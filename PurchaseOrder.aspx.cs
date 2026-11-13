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
    public partial class PurchaseOrder : System.Web.UI.Page
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
                "select p.PO_Id, p.PO_DateCreated, p.PO_TotalAmount, p.PO_Status, s.Supp_Name " +
                "from PO_Tbl p inner join Supplier_Tbl s on p.PO_SupplierId = s.Supp_Id " +
                "order by p.PO_Id desc",
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
                Response.Redirect("Edt_PurchaseOrder.aspx?id=" + e.CommandArgument);
            }
            else
            {
                getCon();

                cmd = new SqlCommand("select count(*) from BackOrder_Tbl where BO_PO_Id = '" + e.CommandArgument + "'", con);

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    Response.Write("<script>alert('Delete the Back Order of this Purchase Order first')</script>");
                    return;
                }

                // remove the stock this PO added when it was received
                cmd = new SqlCommand("delete from Stock_Tbl where Stock_Ref = 'RCV-" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("delete from PO_Tbl where PO_Id = '" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                fillGrid();
                Response.Write("<script>alert('Purchase Order Deleted Successfully')</script>");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["admin"] = null;
            Response.Redirect("login.aspx");
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Add_PurchaseOrder.aspx");
        }
    }
}
