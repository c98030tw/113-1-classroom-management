using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.OleDb;

namespace web
{
    public partial class User : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        private DateTime CurrentWeekStart
        {
            get => ViewState["CurrentWeekStart"] != null ? (DateTime)ViewState["CurrentWeekStart"] : DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            set => ViewState["CurrentWeekStart"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("/Default.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    LoadRooms();
                }
                    BindGrid();
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            // 檢查是否有勾選節次
            if (SelectedSlots.Count == 0)
            {
                // 無勾選節次，顯示警告標籤
                lblWarning.Visible = true;
                return;
            }

            // 有勾選節次，記錄到 Session 並跳轉
            List<Dictionary<string, object>> appliedSlots = new List<Dictionary<string, object>>();

            foreach (var day in SelectedSlots)
            {
                foreach (var period in day.Value)
                {
                    appliedSlots.Add(new Dictionary<string, object>
                    {
                        { "roomid", ddlRooms.SelectedValue }, // 教室編號
                        { "using_hour", period },             // 節次
                        { "using_date", CurrentWeekStart.AddDays(int.Parse(day.Key.Replace("星期", "")) - 1) }, // 日期
                        { "using_day", int.Parse(day.Key.Replace("星期", "")) } // 星期
                    });
                }
            }

            // 將資料存入 Session
            Session["AppliedSlots"] = appliedSlots;

            // 跳轉到 Apply.aspx
            Response.Redirect("/Apply.aspx");
        }


        protected void ButtonProfile_Click(object sender, EventArgs e)
        {
            Response.Redirect("/EditProfile.aspx");
        }

        private void LoadRooms()
        {
            ddlRooms.DataSource = GetRoomsFromDB();
            ddlRooms.DataTextField = "roomname";
            ddlRooms.DataValueField = "roomid";
            ddlRooms.DataBind();
        }

        private void BindGrid()
        {
            if (ddlRooms.SelectedValue == "0")
            {
                gvSchedule.DataSource = null;
                gvSchedule.DataBind();
                return;
            }

            DataTable schedule = GetScheduleForRoom(ddlRooms.SelectedValue, CurrentWeekStart);
            GenerateDynamicGrid(schedule);
        }

        private void GenerateDynamicGrid(DataTable schedule)
        {
            gvSchedule.Columns.Clear();

            // 添加節次欄 (固定列)
            gvSchedule.Columns.Add(new BoundField { HeaderText = "", DataField = "Period" });

            // 添加星期一到星期五的表頭，包含日期
            for (int i = 1; i <= 5; i++) // 星期一到五
            {
                string headerText = $"星期{GetChineseDay(i)}<br/>{CurrentWeekStart.AddDays(i - 1):MM/dd}";
                gvSchedule.Columns.Add(new TemplateField
                {
                    HeaderText = headerText,
                    ItemTemplate = new DynamicCheckBoxTemplate(i)
                });
            }

            gvSchedule.DataSource = schedule;
            gvSchedule.DataBind();
        }

        protected void ddlRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 獲取選中的教室
            string selectedRoomId = ddlRooms.SelectedValue;

            // 如果未選擇教室，直接返回
            if (selectedRoomId == "--請選擇教室--")
            {
                return;
            }

            // 清空已選節次
            ClearSelections();

            // 重新綁定表格
            BindGrid();
        }

        protected void btnPrevWeek_Click(object sender, EventArgs e)
        {
            // 切換到上一週
            CurrentWeekStart = CurrentWeekStart.AddDays(-7);

            // 清空已選節次
            ClearSelections();

            // 重新綁定表格
            BindGrid();
        }

        protected void btnNextWeek_Click(object sender, EventArgs e)
        {
            // 切換到下一週
            CurrentWeekStart = CurrentWeekStart.AddDays(7);

            // 清空已選節次
            ClearSelections();

            // 重新綁定表格
            BindGrid();
        }

        private void ClearSelections()
        {
            // 清空後端已選記錄
            SelectedSlots.Clear();

            // 清空前端 Label
            lblSelected.Text = "已選節次：";

            // 重新綁定表格以刷新顯示
            gvSchedule.DataBind();
        }

        private DataTable GetRoomsFromDB()
        {
            DataTable table = new DataTable();
            string query = "SELECT roomid, roomname FROM Room";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }

        private DataTable GetScheduleForRoom(string roomId, DateTime weekStart)
        {
            // 初始化 schedule 表格
            DataTable schedule = new DataTable();
            schedule.Columns.Add("Period", typeof(string));
            for (int i = 1; i <= 5; i++) // 星期一到五
            {
                schedule.Columns.Add($"Day{i}", typeof(string));
            }

            string query_schedule = @"
            SELECT available_date, available_hour 
            FROM Room_time 
            WHERE roomid = '" + roomId + @"' 
            AND available_date BETWEEN #" + weekStart.ToString("yyyy/MM/dd") + @"#
                               AND #" + weekStart.AddDays(4).ToString("yyyy/MM/dd") + "#";

            string query_apply = @"
            SELECT applier, roomid, using_hour, using_day 
            FROM Apply 
            WHERE returned = FALSE 
            AND roomid = '" + roomId + @"'
            AND using_date BETWEEN #" + weekStart.ToString("yyyy/MM/dd") + @"#
                           AND #" + weekStart.AddDays(4).ToString("yyyy/MM/dd") + "#";

            Dictionary<string, string> applyData = new Dictionary<string, string>(); // 儲存預借信息
            Dictionary<string, string> roomTimeData = new Dictionary<string, string>(); // 儲存可借用數據

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // 查詢申請數據
                using (OleDbCommand cmdApply = new OleDbCommand(query_apply, conn))
                {
                    using (OleDbDataReader reader = cmdApply.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string applier = reader["applier"].ToString();
                            string usingHour = reader["using_hour"].ToString();
                            string usingDay = $"Day{reader["using_day"]}";

                            // 保存格式：usingDay + 節次 -> 使用者編號
                            foreach (char period in usingHour)
                            {
                                string key = $"{usingDay}_{period}";
                                applyData[key] = applier;
                            }
                        }
                    }
                }

                // 預先加載可借用數據到字典
                using (OleDbCommand cmdSchedule = new OleDbCommand(query_schedule, conn))
                {
                    cmdSchedule.Parameters.AddWithValue("@roomid", roomId);
                    cmdSchedule.Parameters.AddWithValue("@start_date", weekStart.ToString("yyyy-MM-dd"));
                    cmdSchedule.Parameters.AddWithValue("@end_date", weekStart.AddDays(4).ToString("yyyy-MM-dd"));

                    using (OleDbDataReader reader = cmdSchedule.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string availableDate = reader["available_date"].ToString();
                            string availableHour = reader["available_hour"].ToString();
                            foreach (char period in availableHour)
                            {
                                string key = $"{availableDate}_{period}";
                                roomTimeData[key] = "1"; // 可借用
                            }
                        }
                    }
                }

                // 初始化節次
                string[] periods = { "A", "1", "2", "3", "4", "B", "5", "6", "7", "8", "9" };
                foreach (string period in periods)
                {
                    DataRow row = schedule.NewRow();
                    row["Period"] = period;

                    for (int i = 1; i <= 5; i++) // 星期一到五
                    {
                        string day = $"Day{i}";
                        string dateKey = weekStart.AddDays(i - 1).ToString();
                        string periodKey = $"{dateKey}_{period}";

                        // 判斷是否已被預借
                        if (applyData.ContainsKey($"{day}_{period}"))
                        {
                            row[day] = applyData[$"{day}_{period}"]; // 顯示使用者編號
                        }
                        else if (roomTimeData.ContainsKey(periodKey))
                        {
                            row[day] = "1"; // 可借用
                        }
                        else
                        {
                            row[day] = ""; // 不可借用
                        }
                    }

                    schedule.Rows.Add(row);
                }
            }

            return schedule;
        }


        protected void gvSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowData = (DataRowView)e.Row.DataItem;

                // 最左欄顯示節次與時間範圍
                string period = rowData["Period"].ToString();
                if (PeriodTimeMap.ContainsKey(period))
                {
                    e.Row.Cells[0].Text = $"{period} ( {PeriodTimeMap[period]} )";
                }

                for (int i = 1; i <= 5; i++) // 星期一到五
                {
                    string columnName = $"Day{i}";
                    string available = rowData[columnName]?.ToString();

                    if (available == "1")
                    {
                        string periodKey = rowData["Period"].ToString();

                        // 清空已有控件
                        e.Row.Cells[i].Controls.Clear();

                        // 添加 HiddenField
                        HiddenField hiddenPeriod = new HiddenField
                        {
                            ID = $"hfPeriod_Row{e.Row.RowIndex}_Col{i}",
                            Value = periodKey
                        };
                        e.Row.Cells[i].Controls.Add(hiddenPeriod);

                        // 添加 CheckBox
                        CheckBox cb = new CheckBox
                        {
                            ID = $"cb_Row{e.Row.RowIndex}_Col{i}",
                            AutoPostBack = true
                        };
                        cb.CheckedChanged += CheckBox_CheckedChanged;

                        // 綁定 CheckBox 狀態
                        string key = $"星期{i}";
                        cb.Checked = SelectedSlots.ContainsKey(key) && SelectedSlots[key].Contains(periodKey);

                        e.Row.Cells[i].Controls.Add(cb);
                    }
                    else if (available == "")
                    {
                        e.Row.Cells[i].Text = ""; // 無法借用時清空顯示
                    }
                    else
                    {
                        e.Row.Cells[i].Text = available + " 已預借"; // 已有人預借時顯示預借人id
                    }
                }
            }
        }






        private string GetChineseDay(int day)
        {
            string[] days = { "一", "二", "三", "四", "五" };
            return days[day - 1];
        }

        private Dictionary<string, HashSet<string>> SelectedSlots
        {
            get
            {
                if (ViewState["SelectedSlots"] == null)
                {
                    ViewState["SelectedSlots"] = new Dictionary<string, HashSet<string>>();
                }
                return (Dictionary<string, HashSet<string>>)ViewState["SelectedSlots"];
            }
            set
            {
                ViewState["SelectedSlots"] = value;
            }
        }


        protected void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            GridViewRow currentRow = (GridViewRow)cb.NamingContainer;

            string[] idParts = cb.ID.Split('_');
            string dayColumn = idParts[2][3].ToString(); // 修正為正確的星期編號
            string hiddenFieldId = $"hfPeriod_Row{currentRow.RowIndex}_Col{dayColumn}";
            HiddenField hiddenPeriod = (HiddenField)currentRow.FindControl(hiddenFieldId);

            if (hiddenPeriod == null)
            {
                throw new InvalidOperationException($"HiddenField '{hiddenFieldId}' 不存在。");
            }

            string period = hiddenPeriod.Value;
            string selectedDay = $"星期{dayColumn}";

            // 清空其他日期的選擇
            if (!SelectedSlots.ContainsKey(selectedDay))
            {
                SelectedSlots.Clear(); // 清除所有已選內容
                SelectedSlots[selectedDay] = new HashSet<string>();
            }

            // 更新當前日期的選擇
            if (cb.Checked)
            {
                SelectedSlots[selectedDay].Add(period);
            }
            else
            {
                SelectedSlots[selectedDay].Remove(period);
                if (SelectedSlots[selectedDay].Count == 0)
                {
                    SelectedSlots.Remove(selectedDay);
                }
            }

            // 更新 Label
            UpdateLabel();

            // 強制刷新 GridView 的狀態
            gvSchedule.DataBind();
        }





        private static readonly List<string> PeriodOrder = new List<string>
        {
            "A", "1", "2", "3", "4", "B", "5", "6", "7", "8", "9"
        };

        private void UpdateLabel()
        {
            lblSelected.Text = "已選節次：";
            foreach (var day in SelectedSlots.OrderBy(k => k.Key)) // 日期排序
            {
                string sortedPeriods = string.Join("、", day.Value.OrderBy(v => PeriodOrder.IndexOf(v))); // 節次排序
                lblSelected.Text += $"{day.Key}, 節次: {sortedPeriods}; ";
            }

            lblSelected.Text = lblSelected.Text.TrimEnd(' ', ';');
        }



        private static readonly Dictionary<string, string> PeriodTimeMap = new Dictionary<string, string>
        {
            { "A", "07:00 ~ 07:50" },
            { "1", "08:10 ~ 09:00" },
            { "2", "09:10 ~ 10:00" },
            { "3", "10:10 ~ 11:00" },
            { "4", "11:10 ~ 12:00" },
            { "B", "12:10 ~ 13:00" },
            { "5", "13:10 ~ 14:00" },
            { "6", "14:10 ~ 15:00" },
            { "7", "15:10 ~ 16:00" },
            { "8", "16:10 ~ 17:00" },
            { "9", "17:10 ~ 18:00" }
        };


    }

    public class DynamicCheckBoxTemplate : ITemplate
    {
        private readonly int _dayColumn;

        public DynamicCheckBoxTemplate(int dayColumn)
        {
            _dayColumn = dayColumn;
        }

        public void InstantiateIn(Control container)
        {
            GridViewRow row = container.NamingContainer as GridViewRow;
            int rowIndex = row != null ? row.RowIndex : 0;

            // 清除已有控件
            container.Controls.Clear();

            // 添加 HiddenField
            HiddenField hiddenPeriod = new HiddenField
            {
                ID = $"hfPeriod_Row{rowIndex}_Col{_dayColumn}"
            };
            container.Controls.Add(hiddenPeriod);

            // 添加 CheckBox
            CheckBox cb = new CheckBox
            {
                ID = $"cb_Row{rowIndex}_Col{_dayColumn}",
                AutoPostBack = true
            };

            cb.CheckedChanged += (s, e) =>
            {
                CheckBox senderCb = (CheckBox)s;
                GridViewRow currentRow = (GridViewRow)senderCb.NamingContainer;

                string hiddenFieldId = $"hfPeriod_Row{currentRow.RowIndex}_Col{_dayColumn}";
                HiddenField periodField = (HiddenField)currentRow.FindControl(hiddenFieldId);

                if (periodField == null)
                {
                    throw new InvalidOperationException($"HiddenField '{hiddenFieldId}' 不存在。");
                }

                string period = periodField.Value;
                string day = $"星期{_dayColumn}";
                Label lblSelected = (Label)FindControlRecursive(container.Page, "lblSelected");

                if (lblSelected != null)
                {
                    lblSelected.Text = $"已選節次：{day}, 節次: {period}";
                }
            };

            container.Controls.Add(cb);
        }
    


    private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (Control child in root.Controls)
            {
                Control found = FindControlRecursive(child, id);
                if (found != null) return found;
            }
            return null;
        }
    }




    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }

}