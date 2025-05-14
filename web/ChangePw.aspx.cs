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
    public partial class ChangePw : System.Web.UI.Page
    {
        string pw; 
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["userid"] == null)
            {
                Response.Redirect("/Default");
            }
            else
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
                string query = "SELECT pw FROM Member WHERE userid = '" + Session["userid"].ToString() + "'";
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        OleDbCommand command = new OleDbCommand(query, connection);
                        OleDbDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            Label1.Text = Session["userid"].ToString();
                            pw = reader["pw"].ToString();
                        }
                        else
                        {
                            Label1.Text = "reader error";
                        }
                        reader.Close();

                        Label1.Visible = true;

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Label1.Text = "錯誤訊息: " + ex.Message;
                    Label1.Visible = true;
                }
            }
            
        }

        protected void ButtonGoback_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Officer.aspx");
        }


        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            
            if(oldPw.Text == "" || newPw.Text == "" || checkPw.Text =="")
            {
                Label2.ForeColor = System.Drawing.Color.Red;
                Label2.Text = "尚有欄位未填";
                Label2.Visible = true;
            }
            else if(!string.Equals(pw, oldPw.Text) || !string.Equals(newPw.Text, checkPw.Text))
            {
                Label2.ForeColor = System.Drawing.Color.Red;
                Label2.Text = "密碼輸入錯誤";
                Label2.Visible = true;
            }
            else
            {
                string updatePw = "UPDATE Member SET pw = '" + newPw.Text + "' WHERE userid = '" + Label1.Text + "'";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(updatePw, connection);
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 1)
                        {
                            Label2.ForeColor = System.Drawing.Color.Black;
                            Label2.Text = "修改成功";
                        }
                        else
                        {
                            Label2.ForeColor = System.Drawing.Color.Red;
                            Label2.Text = "修改失敗";
                        }

                        Label2.Visible = true;

                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Label2.Text = "錯誤訊息: " + ex.Message;
                        Label2.Visible = true;
                    }
                }
            }

        }

        protected void TextBoxPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}