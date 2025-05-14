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
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label3.Visible = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if(TextBox1.Text == "" || TextBox2.Text == "")
            {
                Label3.Text = "未輸入帳號或密碼";
                Label3.Visible = true;
            }
            else
            {
                // 資料庫中學號2的開頭字母一律使用大寫，但登入時可以輸入小寫
                string Userid = TextBox1.Text.ToUpper();
                string Password = TextBox2.Text;
                try
                    {
                        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
                        string userQuery = "SELECT * FROM Member WHERE userid = '" + Userid + "'";

                        using (OleDbConnection connection = new OleDbConnection(connectionString))
                        {
                            connection.Open();

                            OleDbDataAdapter adapter = new OleDbDataAdapter(userQuery, connection);

                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            connection.Close();

                            if (dt.Rows.Count == 1)
                            {
                                // 使用者存在 --> 確認密碼是否正確
                                DataRow[] userRow = dt.Select("pw = '" + Password + "'");
                                if (userRow.Length == 1) // 使用者帳號密碼都正確 --> 成功登入，跳轉到role對應的頁面
                                {
                                    Session["userid"] = Userid;
                                    if (userRow[0]["role"].ToString() == "1")
                                    {
                                        Response.Redirect("User.aspx");
                                    }
                                    else if (userRow[0]["role"].ToString() == "2")
                                    {
                                        Response.Redirect("Officer.aspx");
                                    }
                                    else if (userRow[0]["role"].ToString() == "0")
                                    {
                                        Response.Redirect("Admin.aspx");
                                    }
                                    // Label3.Text = "驗證成功";
                                    // Label3.Visible = true;
                                }
                                else // 密碼不正確
                                {
                                    Label3.Text = "密碼不正確";
                                    Label3.Visible = true;
                                }
                            }
                            else
                            {
                                // 使用者不存在 --> 登入失敗，跳轉到註冊頁面
                                Label3.Text = "使用者尚未註冊";
                                Label3.Visible = true;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
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
    }
}