<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard | Stock Management System</title>
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
                                <a href="WebForm1.aspx" class="nav-link active">
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
                                <h1 class="m-0">Dashboard</h1>
                            </div>
                        </div>
                    </div>
                </div>

                <section class="content">
                    <div class="container-fluid">

                        <div class="card card-outline card-primary mb-4">
                            <div class="card-body">
                                <h4>
                                    <asp:Label ID="lblWelcome" runat="server"></asp:Label></h4>
                                <p class="text-muted mb-0">Use the menu on the left, or the quick links below, to manage the system.</p>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-3 col-6">
                                <div class="small-box bg-info">
                                    <div class="inner">
                                        <h4>Supplier</h4>
                                        <p>Manage suppliers</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-truck"></i></div>
                                    <a href="Supplier.aspx" class="small-box-footer">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box bg-success">
                                    <div class="inner">
                                        <h4>Item</h4>
                                        <p>Manage items / products</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-box"></i></div>
                                    <a href="Item.aspx" class="small-box-footer">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box bg-warning">
                                    <div class="inner">
                                        <h4>Stocks</h4>
                                        <p>Stock in / out</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-warehouse"></i></div>
                                    <a href="Stocks.aspx" class="small-box-footer">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box bg-danger">
                                    <div class="inner">
                                        <h4>Sales</h4>
                                        <p>Manage sales</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-cash-register"></i></div>
                                    <a href="Sales.aspx" class="small-box-footer">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box bg-secondary">
                                    <div class="inner">
                                        <h4>Purchase Order</h4>
                                        <p>Order from suppliers</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-file-invoice"></i></div>
                                    <a href="PurchaseOrder.aspx" class="small-box-footer">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box bg-primary">
                                    <div class="inner">
                                        <h4>Receiving</h4>
                                        <p>Receive stock</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-dolly"></i></div>
                                    <a href="Receiving.aspx" class="small-box-footer">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box" style="background-color: #6f42c1; color: #fff;">
                                    <div class="inner">
                                        <h4>Back Order</h4>
                                        <p>Manage back orders</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-undo-alt"></i></div>
                                    <a href="BackOrder.aspx" class="small-box-footer" style="color: #fff;">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box" style="background-color: #20c997; color: #fff;">
                                    <div class="inner">
                                        <h4>Return</h4>
                                        <p>Manage returns</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-reply"></i></div>
                                    <a href="Return.aspx" class="small-box-footer" style="color: #fff;">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
                            </div>

                            <div class="col-lg-3 col-6">
                                <div class="small-box bg-dark">
                                    <div class="inner">
                                        <h4>Users</h4>
                                        <p>Manage system users</p>
                                    </div>
                                    <div class="icon"><i class="fas fa-users"></i></div>
                                    <a href="Users.aspx" class="small-box-footer">View List <i class="fas fa-arrow-circle-right"></i></a>
                                </div>
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
