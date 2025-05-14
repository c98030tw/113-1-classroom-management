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
    public partial class Officer : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        
        protected DateTime CurrentWeekStart
        {
            get => (DateTime)(ViewState["CurrentWeekStart"] ?? DateTime.Now.StartOfWeek(DayOfWeek.Monday));
            set => ViewState["CurrentWeekStart"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("/Default");
            }
            else
            {
                if (!IsPostBack)
                {
                    LoadRooms();
                    BindGrid();
                    LoadEquipments();
                    LoadApplications();
                }
            }
        }

        private void LoadRooms()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT roomid, roomname FROM Room";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlRoom.Items.Add(new ListItem(reader["roomname"].ToString(), reader["roomid"].ToString()));
                        }
                    }
                }
            }
        }

        private void LoadEquipments()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT equipid, equipname, borrowed FROM Equip";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dtEquip = new DataTable();
                        dtEquip.Load(reader);

                        gvEquip.DataSource = dtEquip;
                        gvEquip.DataBind();
                    }
                }
            }
        }

        private void LoadApplications()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string queryApply = @"
            SELECT applyid, applier, roomid, using_hour, using_date, purpose, officeid, duedate, returned
            FROM Apply";

                using (OleDbCommand cmdApply = new OleDbCommand(queryApply, conn))
                {
                    using (OleDbDataReader readerApply = cmdApply.ExecuteReader())
                    {
                        DataTable dtApply = new DataTable();
                        dtApply.Columns.Add("applyid", typeof(int));
                        dtApply.Columns.Add("applier", typeof(string));
                        dtApply.Columns.Add("roomid", typeof(string));
                        dtApply.Columns.Add("using_hour", typeof(string));
                        dtApply.Columns.Add("using_date", typeof(DateTime));
                        dtApply.Columns.Add("purpose", typeof(string));
                        dtApply.Columns.Add("EquipNames", typeof(string)); // 動態填充設備名稱
                        dtApply.Columns.Add("officeid", typeof(string));
                        dtApply.Columns.Add("duedate", typeof(DateTime));
                        dtApply.Columns.Add("returned", typeof(bool));

                        while (readerApply.Read())
                        {
                            int applyId = (int)readerApply["applyid"];
                            string applier = readerApply["applier"].ToString();
                            string roomId = readerApply["roomid"].ToString();
                            string usingHour = readerApply["using_hour"].ToString();
                            DateTime usingDate = (DateTime)readerApply["using_date"];
                            string purpose = readerApply["purpose"].ToString();
                            string officeId = readerApply["officeid"].ToString();
                            DateTime dueDate = (DateTime)readerApply["duedate"];
                            bool returned = Convert.ToBoolean(readerApply["returned"]);

                            // 動態查詢設備名稱
                            string equipNames = GetEquipNamesForApply(conn, applyId);

                            dtApply.Rows.Add(applyId, applier, roomId, usingHour, usingDate, purpose, equipNames, officeId, dueDate, returned);
                        }

                        gvApply.DataSource = dtApply;
                        gvApply.DataBind();
                    }
                }
            }
        }

        private string GetEquipNamesForApply(OleDbConnection conn, int applyId)
        {
            string queryEquip = @"
        SELECT E.equipname
        FROM Apply_equip AE
        INNER JOIN Equip E ON AE.equipid = E.equipid
        WHERE AE.applyid = ?";

            using (OleDbCommand cmdEquip = new OleDbCommand(queryEquip, conn))
            {
                cmdEquip.Parameters.AddWithValue("@applyid", applyId);

                using (OleDbDataReader readerEquip = cmdEquip.ExecuteReader())
                {
                    List<string> equipNames = new List<string>();

                    while (readerEquip.Read())
                    {
                        equipNames.Add(readerEquip["equipname"].ToString());
                    }

                    // 將設備名稱用換行分隔
                    return string.Join("<br />", equipNames);
                }
            }
        }


        protected void ddlRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            if (string.IsNullOrEmpty(ddlRoom.SelectedValue)) return;

            string roomId = ddlRoom.SelectedValue;
            DateTime weekStart = CurrentWeekStart;
            DataTable schedule = GetScheduleForRoom(roomId, weekStart);

            // 附上日期
            for (int i = 1; i <= 5; i++)
            {
                DateTime dayDate = weekStart.AddDays(i - 1);
                gvSchedule.Columns[i].HeaderText = $"星期{GetChineseDayOfWeek(i)}<br />{dayDate:MM/dd}";
            }

            gvSchedule.DataSource = schedule;
            gvSchedule.DataBind();
        }

        private string GetChineseDayOfWeek(int dayIndex)
        {
            string[] chineseDays = { "一", "二", "三", "四", "五" };
            return chineseDays[dayIndex - 1];
        }


        private DataTable GetScheduleForRoom(string roomId, DateTime weekStart)
        {
            DataTable schedule = new DataTable();
            schedule.Columns.Add("Period", typeof(string));
            for (int i = 1; i <= 5; i++)
            {
                schedule.Columns.Add($"Day{i}", typeof(string));
            }

            string query_schedule = @"
                SELECT available_date, available_hour 
                FROM Room_time 
                WHERE roomid = ? AND available_date BETWEEN ? AND ?";
            string query_apply = @"
                SELECT applier, using_hour, using_day 
                FROM Apply 
                WHERE returned = FALSE AND roomid = ? AND using_date BETWEEN ? AND ?";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                Dictionary<string, string> applyData = new Dictionary<string, string>();

                using (OleDbCommand cmdApply = new OleDbCommand(query_apply, conn))
                {
                    cmdApply.Parameters.AddWithValue("@roomid", roomId);
                    cmdApply.Parameters.AddWithValue("@start_date", weekStart);
                    cmdApply.Parameters.AddWithValue("@end_date", weekStart.AddDays(4));

                    using (OleDbDataReader reader = cmdApply.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string applier = reader["applier"].ToString();
                            string usingHour = reader["using_hour"].ToString();
                            string usingDay = $"Day{reader["using_day"]}";

                            foreach (char period in usingHour)
                            {
                                applyData[$"{usingDay}_{period}"] = $"{applier} 已預借";
                            }
                        }
                    }
                }

                using (OleDbCommand cmdSchedule = new OleDbCommand(query_schedule, conn))
                {
                    cmdSchedule.Parameters.AddWithValue("@roomid", roomId);
                    cmdSchedule.Parameters.AddWithValue("@start_date", weekStart);
                    cmdSchedule.Parameters.AddWithValue("@end_date", weekStart.AddDays(4));

                    using (OleDbDataReader reader = cmdSchedule.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string availableDate = reader["available_date"].ToString();
                            string availableHour = reader["available_hour"].ToString();
                            DateTime date = DateTime.Parse(availableDate);

                            string dayKey = $"Day{(int)date.DayOfWeek}";
                            foreach (char period in availableHour)
                            {
                                if (!applyData.ContainsKey($"{dayKey}_{period}"))
                                {
                                    applyData[$"{dayKey}_{period}"] = "可借出";
                                }
                            }
                        }
                    }
                }

                string[] periods = { "A", "1", "2", "3", "4", "B", "5", "6", "7", "8", "9" };
                string[] times = {
                    "07:00 ~ 07:50", "08:10 ~ 09:00", "09:10 ~ 10:00", "10:10 ~ 11:00",
                    "11:10 ~ 12:00", "12:10 ~ 13:00", "13:10 ~ 14:00", "14:10 ~ 15:00",
                    "15:10 ~ 16:00", "16:10 ~ 17:00", "17:10 ~ 18:00"
                };

                for (int i = 0; i < periods.Length; i++)
                {
                    DataRow row = schedule.NewRow();
                    row["Period"] = $"{periods[i]} ({times[i]})";

                    for (int j = 1; j <= 5; j++)
                    {
                        string key = $"Day{j}_{periods[i]}";
                        row[$"Day{j}"] = applyData.ContainsKey(key) ? applyData[key] : "";
                    }

                    schedule.Rows.Add(row);
                }
            }

            return schedule;
        }

        protected void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            string username;
            string userQuery = "SELECT username FROM Member WHERE userid = '" + Session["userid"].ToString() + "'";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                OleDbCommand cmd = new OleDbCommand(userQuery, conn);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    username = reader["username"].ToString();
                }
                conn.Close();
            }

            DateTime now = DateTime.Now;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                foreach (GridViewRow row in gvApply.Rows)
                {
                    // 尋找 "已歸還" 的 CheckBox
                    CheckBox cbReturn = row.FindControl("cbReturn") as CheckBox;
                    if (cbReturn != null && cbReturn.Checked)
                    {
                        // 從 DataKeyNames 取得 applyid
                        int applyId = Convert.ToInt32(gvApply.DataKeys[row.RowIndex].Value);

                        // 更新 Apply 表的內容
                        string updateApplyQuery = @"
                    UPDATE Apply 
                    SET officeid = ?, duedate = ?, returned = TRUE 
                    WHERE applyid = ?";

                        using (OleDbCommand cmd = new OleDbCommand(updateApplyQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@officeid", username);
                            cmd.Parameters.AddWithValue("@duedate", now.ToString());
                            cmd.Parameters.AddWithValue("@applyid", applyId);
                            cmd.ExecuteNonQuery();
                        }

                        // 查詢該申請的所有設備
                        string queryEquip = @"
                    SELECT equipid 
                    FROM Apply_equip 
                    WHERE applyid = ?";

                        List<string> equipIds = new List<string>();
                        using (OleDbCommand cmdEquip = new OleDbCommand(queryEquip, conn))
                        {
                            cmdEquip.Parameters.AddWithValue("@applyid", applyId);
                            using (OleDbDataReader reader = cmdEquip.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    equipIds.Add(reader["equipid"].ToString());
                                }
                            }
                        }

                        // 更新 Equip 表的狀態
                        foreach (string equipId in equipIds)
                        {
                            string updateEquipQuery = @"
                        UPDATE Equip 
                        SET borrowed = FALSE 
                        WHERE equipid = ?";

                            using (OleDbCommand cmdUpdateEquip = new OleDbCommand(updateEquipQuery, conn))
                            {
                                cmdUpdateEquip.Parameters.AddWithValue("@equipid", equipId);
                                cmdUpdateEquip.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // 重新加載資料
                LoadApplications();
                LoadEquipments();
                BindGrid();
            }
        }



        protected void btnPrevWeek_Click(object sender, EventArgs e)
        {
            CurrentWeekStart = CurrentWeekStart.AddDays(-7);
            BindGrid();
        }

        protected void btnNextWeek_Click(object sender, EventArgs e)
        {
            CurrentWeekStart = CurrentWeekStart.AddDays(7);
            BindGrid();
        }


        protected void btnChangePw_Click(object sender, EventArgs e)
        {
            Response.Redirect("/ChangePw.aspx");
        }

        
    }


}