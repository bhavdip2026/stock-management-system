using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Return : System.Web.UI.Page
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

        void fillGrid()
        {
            getCon();
            da = new SqlDataAdapter(
                "select r.RET_Id, r.RET_DateCreated, r.RET_TotalAmount, s.Supp_Name " +
                "from Return_Tbl r inner join Supplier_Tbl s on r.RET_SupplierId = s.Supp_Id " +
                "order by r.RET_Id desc",
                con
            );
            ds = new DataSet();
            da.Fill(ds);
            GridView1.DataSource = ds;
            GridView1.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack)
            {
                fillGrid();
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "cmd_edt")
            {
                Response.Redirect("Edt_Return.aspx?id=" + e.CommandArgument);
            }
            else
            {
                getCon();

                // give the returned items back to stock
                cmd = new SqlCommand("delete from Stock_Tbl where Stock_Ref = 'RET-" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("delete from Return_Tbl where RET_Id = '" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                fillGrid();
                Response.Write("<script>alert('Return Deleted Successfully')</script>");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["admin"] = null;
            Response.Redirect("login.aspx");
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Add_Return.aspx");
        }
    }
}
