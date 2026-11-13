using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace WebApplication1
{
    public partial class login : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataSet ds;


        string s = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        void getCon()
        {
            con = new SqlConnection(s);
            con.Open();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["admin"] != null)
            {
                Response.Redirect("WebForm1.aspx");
            }
        }


        protected void login_btn_Click(object sender, EventArgs e)
        {
            if (!(string.IsNullOrEmpty(txtusername.Text)) && !(string.IsNullOrEmpty(txtpass.Text)))
            {
                getCon();


                cmd = new SqlCommand("SELECT COUNT(*) FROM Users_Tbl WHERE Username='" + txtusername.Text + "' AND Password='" + txtpass.Text + "'", con);
                int i = Convert.ToInt32(cmd.ExecuteScalar());

                if (i > 0)
                {

                    Session["admin"] = txtusername.Text;
                    Response.Redirect("WebForm1.aspx");
                }
                else
                {
                    Response.Write("<script>alert('Invalid Username or Password')</script>");
                }


            }
        }
    }
}
