using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Globalization;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace web
{
    public partial class Apply : System.Web.UI.Page
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        protected void Page_Init(object sender, EventArgs e)
        {
            // 在初始化階段重建設備控件
            if (Session["Equipments"] != null)
            {
                var equipments = (List<KeyValuePair<string, string>>)Session["Equipments"];
                RecreateEquipControls(equipments);
            }
        }
        private void RecreateEquipControls(List<KeyValuePair<string, string>> equipments)
        {
            foreach (var equip in equipments)
            {
                string equipId = equip.Key;
                string equipName = equip.Value;

                // 動態生成設備 CheckBox
                CheckBox cbEquip = new CheckBox
                {
                    ID = $"cbEquip_{equipId}",
                    Text = equipName
                };

                phEquip.Controls.Add(cbEquip);
                phEquip.Controls.Add(new Literal { Text = "<br />" });
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 確保 Session 中有資料
                if (Session["userid"] == null || Session["AppliedSlots"] == null)
                {
                    Response.Redirect("/User.aspx"); // 返回申請頁面
                    return;
                }

                // 查詢可用設備並填充 CheckBoxList
                LoadEquipments();

                // 取得申請資料
                var appliedSlots = (List<Dictionary<string, object>>)Session["AppliedSlots"];
                lblApplicant.Text = Session["userid"].ToString();

                var roomId = new HashSet<string>();
                var periods = new List<string>();
                var dates = new HashSet<string>();

                foreach (var slot in appliedSlots)
                {
                    roomId.Add(slot["roomid"].ToString());
                    periods.Add(slot["using_hour"].ToString());
                    DateTime usingDate = (DateTime)slot["using_date"];
                    int usingDay = (int)slot["using_day"];

                    dates.Add($"{usingDate:yyyy/MM/dd} ({GetChineseDayOfWeek(usingDay)})");
                }

                lblRoom.Text = string.Join(", ", roomId);
                periods.Sort(ComparePeriods);
                lblPeriod.Text = string.Join("、", periods);
                lblDate.Text = string.Join("; ", dates);
            }
        }

        private void LoadEquipments()
        {
            List<KeyValuePair<string, string>> equipments = new List<KeyValuePair<string, string>>();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT equipid, equipname FROM Equip WHERE borrowed = FALSE";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string equipId = reader["equipid"].ToString();
                            string equipName = reader["equipname"].ToString();

                            equipments.Add(new KeyValuePair<string, string>(equipId, equipName));
                        }
                    }
                }
            }

            // 保存設備清單到 Session
            Session["Equipments"] = equipments;

            // 初始化控件
            RecreateEquipControls(equipments);
        }
        /*
        private void ReloadEquipments()
        {
            foreach (Control control in phEquip.Controls)
            {
                if (control is CheckBox cb)
                {
                    // 從請求中恢復勾選狀態
                    string checkboxId = cb.ID;
                    if (Request.Form[checkboxId] != null)
                    {
                        cb.Checked = true;
                    }
                }
            }
        }
        */
        private string GetChineseDayOfWeek(int day)
        {
            string[] chineseDays = { "星期一", "星期二", "星期三", "星期四", "星期五" };
            if (day >= 1 && day <= 5)
            {
                return chineseDays[day - 1];
            }
            return "未知";
        }
        
        private int ComparePeriods(string x, string y)
        {
            string[] order = { "A", "1", "2", "3", "4", "B", "5", "6", "7", "8", "9" };
            int indexX = Array.IndexOf(order, x);
            int indexY = Array.IndexOf(order, y);

            return indexX.CompareTo(indexY);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (phEquip.Controls.Count == 0)
            {
                lblResult.Visible = true;
                lblResult.ForeColor = System.Drawing.Color.Red;
                lblResult.Text = "未加載設備清單";
                return;
            }

            var appliedSlots = (List<Dictionary<string, object>>)Session["AppliedSlots"];
            string userId = lblApplicant.Text;
            string roomId = lblRoom.Text;
            string officeId = "EC5011"; // 固定的系辦代號
            int applyId = -1; // 保存查詢到的 applyid

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // 合併同一天的所有節次
                var groupedSlots = new Dictionary<DateTime, string>();
                foreach (var slot in appliedSlots)
                {
                    DateTime usingDate = (DateTime)slot["using_date"];
                    string usingHour = slot["using_hour"].ToString();

                    if (!groupedSlots.ContainsKey(usingDate))
                    {
                        groupedSlots[usingDate] = usingHour;
                    }
                    else
                    {
                        groupedSlots[usingDate] += usingHour;
                    }
                }

                // 插入到 Apply 表
                foreach (var slot in groupedSlots)
                {
                    DateTime usingDate = slot.Key;
                    string combinedHours = slot.Value;
                    int usingDay = (int)usingDate.DayOfWeek;
                    if (usingDay == 0 || usingDay == 6) usingDay = 0; // 周日或周六，不合法

                    string purpose = txtPurpose.Text.Replace("'", "''"); // 防止 SQL Injection
                    string insertQuery = @"
                INSERT INTO Apply (applier, roomid, using_hour, using_day, using_date, purpose, duedate, officeid, returned) 
                VALUES ('" + userId + "', '" + roomId + "', '"
                        + combinedHours + "', " + usingDay + ", #" + usingDate.ToString("yyyy-MM-dd") + "#, '"
                        + purpose + "', #" + usingDate.ToString("yyyy-MM-dd") + "#, '" + officeId + "', FALSE)";

                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected != 1)
                        {
                            lblResult.Visible = true;
                            lblResult.ForeColor = System.Drawing.Color.Red;
                            lblResult.Text = "申請失敗: INSERT指令異常";
                            return;
                        }
                    }

                    // 查詢 applyid
                    string selectQuery = @"
                SELECT TOP 1 applyid 
                FROM Apply 
                WHERE applier = '" + userId + @"' 
                AND roomid = '" + roomId + @"' 
                AND using_date = #" + usingDate.ToString("yyyy-MM-dd") + @"# 
                AND using_hour = '" + combinedHours + @"' 
                ORDER BY applyid DESC";

                    using (OleDbCommand cmd = new OleDbCommand(selectQuery, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                applyId = Convert.ToInt32(reader["applyid"]);
                            }
                        }
                    }

                    if (applyId == -1)
                    {
                        lblResult.Visible = true;
                        lblResult.ForeColor = System.Drawing.Color.Red;
                        lblResult.Text = "申請失敗: 無法取得 Apply ID";
                        return;
                    }
                }

                bool equipmentInsertFailed = false;

                foreach (Control control in phEquip.Controls)
                {
                    if (control is CheckBox cb && cb.Checked)
                    {
                        string equipId = cb.ID.Replace("cbEquip_", "");

                        // 插入到 Apply_equip 表
                        string equipInsertQuery = @"
                INSERT INTO Apply_equip (applyid, equipid) 
                VALUES (" + applyId + ", '" + equipId + "')";

                        using (OleDbCommand cmd = new OleDbCommand(equipInsertQuery, conn))
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected != 1)
                            {
                                equipmentInsertFailed = true;
                            }
                        }

                        // 更新 Equip 表的狀態
                        string updateEquipQuery = @"
                UPDATE Equip 
                SET borrowed = TRUE 
                WHERE equipid = '" + equipId + "'";

                        using (OleDbCommand cmd = new OleDbCommand(updateEquipQuery, conn))
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected != 1)
                            {
                                equipmentInsertFailed = true;
                            }
                        }
                    }
                }

                if (equipmentInsertFailed)
                {
                    lblResult.Visible = true;
                    lblResult.ForeColor = System.Drawing.Color.Red;
                    lblResult.Text = "申請成功但借用設備失敗";
                }
                else
                {
                    lblResult.Visible = true;
                    lblResult.ForeColor = System.Drawing.Color.Black;
                    lblResult.Text = "申請成功";
                }
            }

        }

    }
}