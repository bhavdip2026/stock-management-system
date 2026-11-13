<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edt_Receiving.aspx.cs" Inherits="WebApplication1.Edt_Receiving" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Receiving | Stock Management System</title>
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
                                <a href="Stocks.aspx" class="nav-link">
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
                                <a href="Receiving.aspx" class="nav-link active">
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
                                <h1 class="m-0">Edit Receiving</h1>
                            </div>
                            <div class="col-sm-6 text-right">
                                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-flat btn-default" OnClick="LinkButton1_Click"><i class="fas fa-arrow-left"></i>&nbsp; Back to Receiving List</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>

                <section class="content">
                    <div class="container-fluid">

                        <!-- PO info -->
                        <div class="card card-outline card-primary">
                            <div class="card-header">
                                <h3 class="card-title">Purchase Order</h3>
                            </div>
                            <div class="card-body">
                                <div class="form-group">
                                    <label>Purchase Order</label>
                                    <asp:Label ID="lblpo" runat="server" CssClass="form-control-plaintext d-block font-weight-bold"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <label>Date Created</label>
                                    <asp:Label ID="lbldate" runat="server" CssClass="form-control-plaintext d-block"></asp:Label>
                                </div>
                                <div class="form-group mb-0">
                                    <label>Status</label>
                                    <asp:Label ID="lblstatus" runat="server" CssClass="form-control-plaintext d-block font-weight-bold"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <!-- Receiving quantities -->
                        <div class="card card-outline card-secondary">
                            <div class="card-header">
                                <h3 class="card-title">Received Quantity</h3>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="POI_Id"
                                        CssClass="table table-bordered table-striped" GridLines="None"
                                        HeaderStyle-CssClass="thead-light">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Item Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("Item_Name") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Price">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("POI_Price", "{0:0.00}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Ordered Qty">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblordered" runat="server" Text='<%# Eval("POI_Qty") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Received Qty">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtrecv" runat="server" CssClass="form-control rounded-0" Width="120px" Text='<%# Eval("RecvQty") %>'></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <small class="text-muted">The received quantity can be from 0 up to the ordered quantity - never more. The missing quantity can be handled on the Back Order page.</small>
                            </div>
                            <div class="card-footer">
                                <asp:Button ID="edt_rec_btn" runat="server" Text="Update Receiving" CssClass="btn btn-flat btn-primary" OnClick="edt_rec_btn_Click" />
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
