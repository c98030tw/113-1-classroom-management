using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace web
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label3.Visible = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == "" || TextBox2.Text == "")
            {
                Label3.ForeColor = System.Drawing.Color.Red;
                Label3.Text = "未輸入帳號或密碼";
                Label3.Visible = true;
            }
            else
            {
                try
                {
                    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
                    string userQuery = "SELECT * FROM Member WHERE userid = '" + TextBox1.Text + "'";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();

                        OleDbDataAdapter adapter = new OleDbDataAdapter(userQuery, connection);

                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count == 1) // 使用者存在 --> 註冊失敗
                        {
                            Label3.ForeColor = System.Drawing.Color.Red;
                            Label3.Text = "使用者已經存在";
                            Label3.Visible = true;

                            TextBox1.Text = "";
                        }
                        else // 使用者不存在 --> 註冊
                        {
                            // 確認帳號格式 (第1碼為英文字母，後9碼是數字)
                            if (TextBox1.Text.Length != 10 || !Regex.IsMatch(TextBox1.Text, @"^[a-zA-Z][0-9]{9}$"))
                            {
                                Label3.ForeColor = System.Drawing.Color.Red;
                                Label3.Text = "帳號格式錯誤";
                                Label3.Visible = true;
                            }
                            // 確認密碼格式 (全為英文字母或數字，不含其他符號)
                            else if (!Regex.IsMatch(TextBox2.Text, @"[a-zA-Z0-9]{1,}$"))
                            {
                                Label3.ForeColor = System.Drawing.Color.Red;
                                Label3.Text = "密碼格式錯誤(限使用英文字母或數字)";
                                Label3.Visible = true;
                            }
                            // 確認連絡電話格式 (10碼數字)
                            else if (TextBox5.Text != "" && !Regex.IsMatch(TextBox5.Text, @"[0-9]{10}$"))
                            {
                                Label3.ForeColor = System.Drawing.Color.Red;
                                Label3.Text = "連絡電話格式錯誤(應為10碼數字)";
                                Label3.Visible = true;
                            }
                            // 確認信箱格式 (英數字@英文字母或點)
                            else if (TextBox6.Text != "" && !Regex.IsMatch(TextBox6.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                            {
                                Label3.ForeColor = System.Drawing.Color.Red;
                                Label3.Text = "信箱格式錯誤";
                                Label3.Visible = true;
                            }
                            // insert
                            else
                            {
                                // 帳號第1碼固定改成大寫字母
                                string Userid = char.ToUpper(TextBox1.Text[0]) + TextBox1.Text.Substring(1);
                                string insertUser = "INSERT INTO Member(userid, username, officeid, email, phone, pw, role) " +
                                                    "VALUES(@userid, @username, @officeid, @email, @phone, @pw, 1)";
                                OleDbCommand command = new OleDbCommand(insertUser, connection);
                                command.Parameters.AddWithValue("@userid", TextBox1.Text.ToUpper());
                                command.Parameters.AddWithValue("@username", TextBox3.Text);
                                command.Parameters.AddWithValue("@officeid", TextBox4.Text);
                                command.Parameters.AddWithValue("@email", TextBox6.Text);
                                command.Parameters.AddWithValue("@phone", TextBox5.Text);
                                command.Parameters.AddWithValue("@pw", TextBox2.Text);

                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected == 1)
                                {
                                    // insert成功，印出提示訊息
                                    Label3.ForeColor = System.Drawing.Color.Black;
                                    Label3.Text = "註冊成功";
                                    Label3.Visible = true;
                                }
                                else
                                {
                                    // insert失敗，印出提示訊息
                                    Label3.ForeColor = System.Drawing.Color.Red;
                                    Label3.Text = "!!註冊失敗!!";
                                    Label3.Visible = true;
                                }

                            }

                            connection.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Label3.ForeColor = System.Drawing.Color.Red;
                    Label3.Text = "錯誤訊息: " + ex.Message;
                    Label3.Visible = true;
                }
            }
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }
        protected void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }
        protected void TextBox4_TextChanged(object sender, EventArgs e)
        {

        }
        protected void TextBox5_TextChanged(object sender, EventArgs e)
        {

        }
        protected void TextBox6_TextChanged(object sender, EventArgs e)
        {

        }

    }
}