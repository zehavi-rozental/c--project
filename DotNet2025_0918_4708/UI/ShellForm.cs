// =============================================================================
// ShellForm.cs  –  Main application shell, sidebar, panel host.
// UI Layout is in ShellForm.Designer.cs.
// BL is passed in from LoginForm.
// =============================================================================

using BlApi;
using System;
using System.Windows.Forms;

namespace UI
{
    public partial class ShellForm : Form
    {
        private readonly IBl _bl;
        private readonly UserRole _role;

        // Lazy-loaded panels
        private AdminCustomersPanel? _customersPanel;
        private AdminProductsPanel?  _productsPanel;
        private AdminSalesPanel?     _salesPanel;
        private POSPanel?            _posPanel;

        public ShellForm(IBl bl, UserRole role)
        {
            _bl   = bl;
            _role = role;
            InitializeComponent();
            BuildNavigation();
            ShowDefaultPanel();
        }

        // ── Navigation ────────────────────────────────────────────────────

        private void BuildNavigation()
        {
            // The sidebar buttons are already created in Designer.cs.
            // Here we attach Click handlers based on role.

            if (_role == UserRole.Admin)
            {
                btnNavCustomers.Visible = true;
                btnNavProducts.Visible  = true;
                btnNavSales.Visible     = true;
                btnNavPOS.Visible       = false;

                btnNavCustomers.Click += (s, e) => ShowPanel(NavSection.Customers);
                btnNavProducts.Click  += (s, e) => ShowPanel(NavSection.Products);
                btnNavSales.Click     += (s, e) => ShowPanel(NavSection.Sales);
            }
            else // Cashier
            {
                btnNavCustomers.Visible = false;
                btnNavProducts.Visible  = false;
                btnNavSales.Visible     = false;
                btnNavPOS.Visible       = true;

                btnNavPOS.Click += (s, e) => ShowPanel(NavSection.POS);
            }

            btnLogout.Click += (s, e) =>
            {
                this.Close();
            };
        }

        private void ShowDefaultPanel()
        {
            ShowPanel(_role == UserRole.Admin ? NavSection.Customers : NavSection.POS);
        }

        private void ShowPanel(NavSection section)
        {
            // Update active state on nav buttons
            btnNavCustomers.IsActive = section == NavSection.Customers;
            btnNavProducts.IsActive  = section == NavSection.Products;
            btnNavSales.IsActive     = section == NavSection.Sales;
            btnNavPOS.IsActive       = section == NavSection.POS;

            // Remove existing content
            pnlContent.Controls.Clear();

            Control panel = section switch
            {
                NavSection.Customers => GetCustomersPanel(),
                NavSection.Products  => GetProductsPanel(),
                NavSection.Sales     => GetSalesPanel(),
                NavSection.POS       => GetPOSPanel(),
                _                    => throw new ArgumentOutOfRangeException()
            };

            panel.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(panel);
        }

        // ── Panel factories (lazy init) ───────────────────────────────────

        private AdminCustomersPanel GetCustomersPanel()
        {
            _customersPanel ??= new AdminCustomersPanel(_bl);
            return _customersPanel;
        }

        private AdminProductsPanel GetProductsPanel()
        {
            _productsPanel ??= new AdminProductsPanel(_bl);
            return _productsPanel;
        }

        private AdminSalesPanel GetSalesPanel()
        {
            _salesPanel ??= new AdminSalesPanel(_bl);
            return _salesPanel;
        }

        private POSPanel GetPOSPanel()
        {
            _posPanel ??= new POSPanel(_bl);
            return _posPanel;
        }

        private enum NavSection { Customers, Products, Sales, POS }
    }
}
