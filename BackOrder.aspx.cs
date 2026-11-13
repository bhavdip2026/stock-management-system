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
    public partial class BackOrder : System.Web.UI.Page
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
                "select b.BO_Id, b.BO_DateCreated, b.BO_PO_Id, b.BO_TotalAmount, b.BO_Status, s.Supp_Name " +
                "from BackOrder_Tbl b " +
                "inner join PO_Tbl p on b.BO_PO_Id = p.PO_Id " +
                "inner join Supplier_Tbl s on p.PO_SupplierId = s.Supp_Id " +
                "order by b.BO_Id desc",
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
                Response.Redirect("Edt_BackOrder.aspx?id=" + e.CommandArgument);
            }
            else
            {
                getCon();

                // remove the stock this back order added (if it was received)
                cmd = new SqlCommand("delete from Stock_Tbl where Stock_Ref = 'BO-" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("delete from BackOrder_Tbl where BO_Id = '" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                fillGrid();
                Response.Write("<script>alert('Back Order Deleted Successfully')</script>");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["admin"] = null;
            Response.Redirect("login.aspx");
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Add_BackOrder.aspx");
        }
    }
}
