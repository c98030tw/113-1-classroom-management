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
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                GridviewLoadMember();
                DropDownListLoadMember();
            }
        }

        private void GridviewLoadMember()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
                string query = "SELECT userid, username, pw, role FROM Member";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Session["memberTable"] = dt;

                    if (dt.Rows.Count > 0)
                    {
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        Label2.Visible = false;
                    }
                    else
                    {
                        Label2.Text = "沒有資料可以顯示";
                        Label2.Visible = true;
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Label2.Text = "錯誤訊息: " + ex.Message;
                Label2.Visible = true;
            }
        }

        protected void DropDownList1_Load(object sender, EventArgs e)
        {
            
        }

        private void DropDownListLoadMember()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            string query = "SELECT userid FROM Member";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                connection.Close();

                if (dt.Rows.Count > 0)
                {
                    DropDownList1.DataValueField = "userid";
                    DropDownList1.DataTextField = "userid";
                    DropDownList1.DataSource = dt;
                    DropDownList1.DataBind();
                    DropDownList1.Items.Insert(0, new ListItem("--請選擇--", "-1"));
                    Label2.Visible = false;
                }
                else
                {
                    Label2.Text = "沒有資料可以顯示";
                    Label2.Visible = true;
                }
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeleteTarget.Text = DropDownList1.SelectedItem.Text;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string target = DeleteTarget.Text.ToString();
            if(target != "-1")
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
                string deleteUser = "DELETE * FROM Member WHERE userid = '"+target+"'";
                using(OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(deleteUser, connection);
                    //command.Parameters.AddWithValue("@userid", target);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if(rowsAffected > 0)
                        {
                            Label1.ForeColor = System.Drawing.Color.Black;
                            Label1.Text = "刪除成功";
                            Label1.Visible = true;
                            GridviewLoadMember();
                            DropDownListLoadMember();
                            DeleteTarget.Text = "-1";
                        }
                        else
                        {
                            Label1.ForeColor = System.Drawing.Color.Red;
                            Label1.Text = "刪除失敗";
                            Label1.Visible = true;
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Label1.ForeColor = System.Drawing.Color.Red;
                        Label1.Text = "錯誤訊息: " + ex.Message;
                        Label1.Visible = true;
                    }
                }
            }
            
        }

        protected void Gridview1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GridView1.DataSource = Session["memberTable"];
            GridView1.DataBind();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GridView1.DataSource = Session["memberTable"];
            GridView1.DataBind();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //Retrieve the table from the session object.
            DataTable dt = (DataTable)Session["memberTable"];

            //Update the values.
            GridViewRow row = GridView1.Rows[e.RowIndex];
            //dt.Rows[row.DataItemIndex]["userid"] = ((TextBox)(row.Cells[1].Controls[0])).Text;
            dt.Rows[row.DataItemIndex]["username"] = ((TextBox)(row.Cells[2].Controls[0])).Text;
            dt.Rows[row.DataItemIndex]["pw"] = ((TextBox)(row.Cells[3].Controls[0])).Text;
            dt.Rows[row.DataItemIndex]["role"] = ((TextBox)(row.Cells[4].Controls[0])).Text;

            //Reset the edit index.
            GridView1.EditIndex = -1;

            //Bind data to the GridView control.
            Session["memberTable"] = dt;
            GridView1.DataSource = Session["memberTable"];
            GridView1.DataBind();

            //Update to database
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            string updateUser = "UPDATE Member SET username = '"+ ((TextBox)(row.Cells[2].Controls[0])).Text
                + "', pw = '" + ((TextBox)(row.Cells[3].Controls[0])).Text
                + "', role = " + int.Parse(((TextBox)(row.Cells[4].Controls[0])).Text)
                + " WHERE userid = '" + dt.Rows[row.DataItemIndex]["userid"] + "'";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(updateUser, connection);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Label2.ForeColor = System.Drawing.Color.Black;
                        Label2.Text = "更新成功";
                        Label2.Visible = true;
                        DropDownListLoadMember();
                    }
                    else
                    {
                        Label2.ForeColor = System.Drawing.Color.Red;
                        Label2.Text = "更新失敗 " + command.CommandText;
                        Label2.Visible = true;
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Label2.ForeColor = System.Drawing.Color.Red;
                    Label2.Text = "錯誤訊息: " + ex.Message;
                    Label2.Visible = true;
                }
            }
        }

    }
}