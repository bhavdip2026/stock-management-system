<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stocks.aspx.cs" Inherits="WebApplication1.Stocks" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Stocks | Stock Management System</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />
    <link rel="stylesheet" href="css/adminlte.css" />
    <link rel="stylesheet" href="css/custom.css" />
</head>
<body class="hold-transition sidebar-mini layout-fixed">
    <form id="form1" runat="server">
        <div class="wrapper">

            <!-- Top Navbar -->
            <nav class="main-header navbar navbar-expand navbar-white navbar-light">
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <a class="nav-link" data-widget="pushmenu" href="#" role="button"><i class="fas fa-bars"></i></a>
                    </li>
                    <li class="nav-item d-none d-sm-inline-block">
                        <span class="nav-link">Stock Management System</span>
                    </li>
                </ul>
                <ul class="navbar-nav ml-auto">
                    <li class="nav-item">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="nav-link" OnClick="LinkButton1_Click"><i class="fas fa-sign-out-alt"></i>&nbsp; Logout</asp:LinkButton>
                    </li>
                </ul>
            </nav>

            <!-- Sidebar -->
            <aside class="main-sidebar sidebar-dark-primary elevation-4">
                <a href="WebForm1.aspx" class="brand-link">
                    <img src="img/logo.png" alt="Logo" class="brand-image img-circle elevation-3" style="opacity: .8" />
                    <span class="brand-text font-weight-light">SMS Admin</span>
                </a>
                <div class="sidebar">
                    <nav class="mt-2">
                        <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu">
                            <li class="nav-item">
                                <a href="WebForm1.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-tachometer-alt"></i>
                                    <p>Dashboard</p>
                                </a>
                            </li>
                            <li class="nav-header">MAINTENANCE</li>
                            <li class="nav-item">
                                <a href="Supplier.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-truck"></i>
                                    <p>Supplier</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="Item.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-box"></i>
                                    <p>Item</p>
                                </a>
                            </li>
                            <li class="nav-header">INVENTORY</li>
                            <li class="nav-item">
                                <a href="Stocks.aspx" class="nav-link active">
                                    <i class="nav-icon fas fa-warehouse"></i>
                                    <p>Stocks</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="PurchaseOrder.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-file-invoice"></i>
                                    <p>Purchase Order</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="Receiving.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-dolly"></i>
                                    <p>Receiving</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="BackOrder.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-undo-alt"></i>
                                    <p>Back Order</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="Return.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-reply"></i>
                                    <p>Return</p>
                                </a>
                            </li>
                            <li class="nav-header">SALES</li>
                            <li class="nav-item">
                                <a href="Sales.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-cash-register"></i>
                                    <p>Sales</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="ProfitLoss.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-chart-line"></i>
                                    <p>Profit / Loss</p>
                                </a>
                            </li>
                            <li class="nav-header">SYSTEM</li>
                            <li class="nav-item">
                                <a href="Users.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-users"></i>
                                    <p>Users</p>
                                </a>
                            </li>
                        </ul>
                    </nav>
                </div>
            </aside>

            <!-- Content Wrapper -->
            <div class="content-wrapper">
                <div class="content-header">
                    <div class="container-fluid">
                        <div class="row mb-2">
                            <div class="col-sm-6">
                                <h1 class="m-0">Stocks</h1>
                            </div>
                        </div>
                    </div>
                </div>

                <section class="content">
                    <div class="container-fluid">
                        <div class="card card-outline card-primary">
                            <div class="card-header">
                                <h3 class="card-title">List of Stocks</h3>
                            </div>
                            <div class="card-body">

                                <div class="row mb-3">
                                    <div class="col-md-6 offset-md-6">
                                        <div class="input-group">
                                            <asp:TextBox ID="txtsearch" runat="server" CssClass="form-control rounded-0" placeholder="Search item, supplier or description"></asp:TextBox>
                                            <div class="input-group-append">
                                                <asp:LinkButton ID="btn_search" runat="server" CssClass="btn btn-flat btn-primary" OnClick="btn_search_Click"><i class="fas fa-search"></i>&nbsp; Search</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                                        AllowPaging="True" PageSize="10" OnPageIndexChanging="GridView1_PageIndexChanging"
                                        CssClass="table table-bordered table-striped" GridLines="None"
                                        HeaderStyle-CssClass="thead-light" PagerStyle-CssClass="pagination-sm">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Item Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("Item_Name") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Supplier">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("Supp_Name") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Description">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label4" runat="server" Text='<%# Eval("Item_Desc") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Received">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("LastReceived") == DBNull.Value ? "-" : Convert.ToDateTime(Eval("LastReceived")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Available Stocks" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label6" runat="server" CssClass='<%# Convert.ToInt32(Eval("Available")) > 0 ? "badge badge-success" : "badge badge-danger" %>' Text='<%# Eval("Available") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <small class="text-muted">Stock is updated automatically: Receiving adds stock, Sales reduce it.</small>
                            </div>
                        </div>
                    </div>
                </section>
            </div>

            <footer class="main-footer">
                <strong>Stock Management System</strong>
            </footer>
        </div>
    </form>
</body>
</html>
