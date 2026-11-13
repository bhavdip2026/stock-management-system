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
    public partial class Receiving : System.Web.UI.Page
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

        // Every purchase order is a row here (cancelled ones are not shown)
        void fillGrid()
        {
            getCon();
            da = new SqlDataAdapter(
                "select p.PO_Id, p.PO_DateCreated, p.PO_Status, s.Supp_Name, " +
                "(select isnull(sum(POI_Qty), 0) from PO_Items_Tbl where POI_PO_Id = p.PO_Id) as OrderedQty, " +
                "(select isnull(sum(isnull(POI_RecvQty, POI_Qty)), 0) from PO_Items_Tbl where POI_PO_Id = p.PO_Id) as ReceivingQty, " +
                "(select isnull(sum(isnull(POI_RecvQty, POI_Qty) * POI_Price), 0) from PO_Items_Tbl where POI_PO_Id = p.PO_Id) as ReceivingAmount " +
                "from PO_Tbl p inner join Supplier_Tbl s on p.PO_SupplierId = s.Supp_Id " +
                "where p.PO_Status in ('Pending', 'Received') " +
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
                Response.Redirect("Edt_Receiving.aspx?id=" + e.CommandArgument);
            }
            else if (e.CommandName == "cmd_rcv")
            {
                getCon();

                // lock in the quantity that will be received (empty = everything ordered)
                cmd = new SqlCommand(
                    "update PO_Items_Tbl set POI_RecvQty = isnull(POI_RecvQty, POI_Qty) where POI_PO_Id = '" + e.CommandArgument + "'",
                    con
                );
                cmd.ExecuteNonQuery();

                // safety: make sure this PO never adds its stock twice
                cmd = new SqlCommand("delete from Stock_Tbl where Stock_Ref = 'RCV-" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                // stock in for every line with the received quantity
                cmd = new SqlCommand(
                    "insert into Stock_Tbl (Stock_ItemId, Stock_Qty, Stock_Type, Stock_Ref) " +
                    "select POI_ItemId, POI_RecvQty, 'IN', 'RCV-" + e.CommandArgument + "' " +
                    "from PO_Items_Tbl where POI_PO_Id = '" + e.CommandArgument + "' and POI_RecvQty > 0",
                    con
                );
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("update PO_Tbl set PO_Status = 'Received' where PO_Id = '" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();

                fillGrid();
                Response.Write("<script>alert('Received - the items were added to stock')</script>");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["admin"] = null;
            Response.Redirect("login.aspx");
        }
    }
}
