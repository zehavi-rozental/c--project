using BlApi;
using BO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class Form1 : Form
    {
        private readonly IBl bl = BlApi.Factory.Get();
        private Order currentOrder = new();

        private TabControl tabControl = null!;
        
        // Admin Tabs
        private TabPage tabCustomers = null!;
        private TabPage tabProducts = null!;
        private TabPage tabSales = null!;
        private TabPage tabPOS = null!;

        // Customers
        private DataGridView dgvCustomers = null!;
        private TextBox txtCustomerFilter = null!;
        private TextBox txtCustomerId = null!;
        private TextBox txtCustomerName = null!;
        private TextBox txtCustomerAddress = null!;
        private TextBox txtCustomerPhone = null!;

        // Products
        private DataGridView dgvProducts = null!;
        private TextBox txtProductFilter = null!;
        private ComboBox cmbProductCategoryFilter = null!;
        private TextBox txtProductId = null!;
        private TextBox txtProductName = null!;
        private ComboBox cmbProductCategory = null!;
        private TextBox txtProductPrice = null!;
        private TextBox txtProductAmount = null!;

        // Sales
        private DataGridView dgvSales = null!;
        private TextBox txtSaleFilter = null!;
        private TextBox txtSaleId = null!;
        private TextBox txtSaleProductId = null!;
        private TextBox txtSaleAmountRequired = null!;
        private TextBox txtSaleTotalPrice = null!;
        private CheckBox chkSaleClubMembers = null!;
        private DateTimePicker dtpSaleStart = null!;
        private DateTimePicker dtpSaleEnd = null!;

        // POS
        private FlowLayoutPanel flpOrderProducts = null!;
        private TextBox txtOrderProductSearch = null!;
        private ComboBox cmbOrderProducts = null!;
        private NumericUpDown numOrderAmount = null!;
        private CheckBox chkPreferredCustomer = null!;
        private FlowLayoutPanel flpOrderItems = null!;
        private Label lblOrderTotal = null!;
        private Button btnOrderAdd = null!;
        private Button btnOrderClear = null!;
        private Button btnOrderPlace = null!;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
            LoadAllData();
        }

        private void InitializeUI()
        {
            ModernUIFactory.StyleForm(this);
            ClientSize = new Size(1400, 900);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIFactory.AppBackground,
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.BodyFont,
            };

            tabCustomers = new TabPage("👥 Customers") { BackColor = ModernUIFactory.AppBackground };
            tabProducts = new TabPage("📦 Products") { BackColor = ModernUIFactory.AppBackground };
            tabSales = new TabPage("🎁 Promotions") { BackColor = ModernUIFactory.AppBackground };
            tabPOS = new TabPage("💳 POS") { BackColor = ModernUIFactory.AppBackground };

            tabControl.TabPages.AddRange(new TabPage[] { tabCustomers, tabProducts, tabSales, tabPOS });
            Controls.Add(tabControl);

            CreateCustomersTab();
            CreateProductsTab();
            CreateSalesTab();
            CreatePOSTab();
        }

        private void CreateCustomersTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20) };

            var header = ModernUIFactory.CreateTopHero("Customer Management", "Manage all customer profiles in the system");
            header.Location = new Point(0, 0);

            var searchPanel = new Panel { Location = new Point(0, 170), Size = new Size(1360, 50), BackColor = Color.Transparent };
            txtCustomerFilter = ModernUIFactory.CreateInputBox("Search by name or ID");
            txtCustomerFilter.Size = new Size(250, 40);
            var btnFilterCustomers = ModernUIFactory.CreateAccentButton("Search", (s, e) => LoadCustomers());
            btnFilterCustomers.Location = new Point(270, 0);

            searchPanel.Controls.AddRange(new Control[] { txtCustomerFilter, btnFilterCustomers });

            dgvCustomers = CreateDataGrid();
            dgvCustomers.Location = new Point(0, 230);
            dgvCustomers.Size = new Size(900, 600);
            dgvCustomers.CellClick += (s, e) => PopulateCustomerForm();

            var formPanel = ModernUIFactory.CreatePanelBox(new Point(920, 230), new Size(440, 600));
            formPanel.Controls.Add(new Label
            {
                Text = "Customer Details",
                ForeColor = ModernUIFactory.Accent,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 10),
            });

            txtCustomerId = ModernUIFactory.CreateInputBox("ID");
            txtCustomerName = ModernUIFactory.CreateInputBox("Name");
            txtCustomerAddress = ModernUIFactory.CreateInputBox("Address");
            txtCustomerPhone = ModernUIFactory.CreateInputBox("Phone");

            txtCustomerId.Location = new Point(16, 50);
            txtCustomerName.Location = new Point(16, 110);
            txtCustomerAddress.Location = new Point(16, 170);
            txtCustomerPhone.Location = new Point(16, 230);

            var btnCreateCust = ModernUIFactory.CreateAccentButton("Create", (s, e) => ExecuteCustomerAction(CustomerAction.Create));
            var btnUpdateCust = ModernUIFactory.CreateAccentButton("Update", (s, e) => ExecuteCustomerAction(CustomerAction.Update));
            var btnDeleteCust = ModernUIFactory.CreateAccentButton("Delete", (s, e) => ExecuteCustomerAction(CustomerAction.Delete));

            btnCreateCust.Location = new Point(16, 300);
            btnUpdateCust.Location = new Point(16, 360);
            btnDeleteCust.Location = new Point(220, 360);

            formPanel.Controls.AddRange(new Control[] { txtCustomerId, txtCustomerName, txtCustomerAddress, txtCustomerPhone, btnCreateCust, btnUpdateCust, btnDeleteCust });

            panel.Controls.AddRange(new Control[] { header, searchPanel, dgvCustomers, formPanel });
            tabCustomers.Controls.Add(panel);
        }

        private void CreateProductsTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20) };

            var header = ModernUIFactory.CreateTopHero("Product Catalog", "Manage all smart home products in inventory");
            header.Location = new Point(0, 0);

            var searchPanel = new Panel { Location = new Point(0, 170), Size = new Size(1360, 50), BackColor = Color.Transparent };
            txtProductFilter = ModernUIFactory.CreateInputBox("Search by name or ID");
            txtProductFilter.Size = new Size(250, 40);
            cmbProductCategoryFilter = new ComboBox
            {
                Location = new Point(270, 0),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(16, 22, 38),
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.BodyFont,
            };
            cmbProductCategoryFilter.Items.Add("All Categories");
            cmbProductCategoryFilter.Items.AddRange(Enum.GetNames(typeof(Category)));
            cmbProductCategoryFilter.SelectedIndex = 0;

            var btnFilterProducts = ModernUIFactory.CreateAccentButton("Search", (s, e) => LoadProducts());
            btnFilterProducts.Location = new Point(480, 0);

            searchPanel.Controls.AddRange(new Control[] { txtProductFilter, cmbProductCategoryFilter, btnFilterProducts });

            dgvProducts = CreateDataGrid();
            dgvProducts.Location = new Point(0, 230);
            dgvProducts.Size = new Size(900, 600);
            dgvProducts.CellClick += (s, e) => PopulateProductForm();

            var formPanel = ModernUIFactory.CreatePanelBox(new Point(920, 230), new Size(440, 600));
            formPanel.Controls.Add(new Label
            {
                Text = "Product Details",
                ForeColor = ModernUIFactory.Accent,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 10),
            });

            txtProductId = ModernUIFactory.CreateInputBox("ID");
            txtProductName = ModernUIFactory.CreateInputBox("Name");
            cmbProductCategory = new ComboBox
            {
                Location = new Point(16, 170),
                Size = new Size(408, 36),
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(16, 22, 38),
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.BodyFont,
            };
            cmbProductCategory.Items.AddRange(Enum.GetNames(typeof(Category)));
            cmbProductCategory.SelectedIndex = 0;

            txtProductPrice = ModernUIFactory.CreateInputBox("Price");
            txtProductAmount = ModernUIFactory.CreateInputBox("Stock Quantity");

            txtProductId.Location = new Point(16, 50);
            txtProductName.Location = new Point(16, 110);
            txtProductPrice.Location = new Point(16, 230);
            txtProductAmount.Location = new Point(16, 290);

            var btnCreateProd = ModernUIFactory.CreateAccentButton("Create", (s, e) => ExecuteProductAction(ProductAction.Create));
            var btnUpdateProd = ModernUIFactory.CreateAccentButton("Update", (s, e) => ExecuteProductAction(ProductAction.Update));
            var btnDeleteProd = ModernUIFactory.CreateAccentButton("Delete", (s, e) => ExecuteProductAction(ProductAction.Delete));

            btnCreateProd.Location = new Point(16, 360);
            btnUpdateProd.Location = new Point(16, 420);
            btnDeleteProd.Location = new Point(220, 420);

            formPanel.Controls.AddRange(new Control[] { txtProductId, txtProductName, cmbProductCategory, txtProductPrice, txtProductAmount, btnCreateProd, btnUpdateProd, btnDeleteProd });

            panel.Controls.AddRange(new Control[] { header, searchPanel, dgvProducts, formPanel });
            tabProducts.Controls.Add(panel);
        }

        private void CreateSalesTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20) };

            var header = ModernUIFactory.CreateTopHero("Promotions & Sales", "Manage special offers and discounts");
            header.Location = new Point(0, 0);

            var searchPanel = new Panel { Location = new Point(0, 170), Size = new Size(1360, 50), BackColor = Color.Transparent };
            txtSaleFilter = ModernUIFactory.CreateInputBox("Search by sale ID or product ID");
            txtSaleFilter.Size = new Size(250, 40);
            var btnFilterSales = ModernUIFactory.CreateAccentButton("Search", (s, e) => LoadSales());
            btnFilterSales.Location = new Point(270, 0);

            searchPanel.Controls.AddRange(new Control[] { txtSaleFilter, btnFilterSales });

            dgvSales = CreateDataGrid();
            dgvSales.Location = new Point(0, 230);
            dgvSales.Size = new Size(900, 600);
            dgvSales.CellClick += (s, e) => PopulateSaleForm();

            var formPanel = ModernUIFactory.CreatePanelBox(new Point(920, 230), new Size(440, 600));
            formPanel.Controls.Add(new Label
            {
                Text = "Promotion Details",
                ForeColor = ModernUIFactory.Accent,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 10),
            });

            txtSaleId = ModernUIFactory.CreateInputBox("Sale ID");
            txtSaleProductId = ModernUIFactory.CreateInputBox("Product ID");
            txtSaleAmountRequired = ModernUIFactory.CreateInputBox("Min Quantity");
            txtSaleTotalPrice = ModernUIFactory.CreateInputBox("Sale Price");

            txtSaleId.Location = new Point(16, 50);
            txtSaleProductId.Location = new Point(16, 110);
            txtSaleAmountRequired.Location = new Point(16, 170);
            txtSaleTotalPrice.Location = new Point(16, 230);

            chkSaleClubMembers = new CheckBox
            {
                Text = "Club Members Only",
                ForeColor = ModernUIFactory.TextSecondary,
                BackColor = Color.FromArgb(14, 22, 44),
                Location = new Point(16, 290),
                AutoSize = true,
            };

            dtpSaleStart = new DateTimePicker
            {
                Location = new Point(16, 330),
                Size = new Size(408, 30),
                Format = DateTimePickerFormat.Short,
                BackColor = Color.FromArgb(16, 22, 38),
                ForeColor = Color.WhiteSmoke,
            };
            dtpSaleEnd = new DateTimePicker
            {
                Location = new Point(16, 370),
                Size = new Size(408, 30),
                Format = DateTimePickerFormat.Short,
                BackColor = Color.FromArgb(16, 22, 38),
                ForeColor = Color.WhiteSmoke,
            };

            var btnCreateSale = ModernUIFactory.CreateAccentButton("Create", (s, e) => ExecuteSaleAction(SaleAction.Create));
            var btnUpdateSale = ModernUIFactory.CreateAccentButton("Update", (s, e) => ExecuteSaleAction(SaleAction.Update));
            var btnDeleteSale = ModernUIFactory.CreateAccentButton("Delete", (s, e) => ExecuteSaleAction(SaleAction.Delete));

            btnCreateSale.Location = new Point(16, 420);
            btnUpdateSale.Location = new Point(16, 480);
            btnDeleteSale.Location = new Point(220, 480);

            formPanel.Controls.AddRange(new Control[] { txtSaleId, txtSaleProductId, txtSaleAmountRequired, txtSaleTotalPrice, chkSaleClubMembers, dtpSaleStart, dtpSaleEnd, btnCreateSale, btnUpdateSale, btnDeleteSale });

            panel.Controls.AddRange(new Control[] { header, searchPanel, dgvSales, formPanel });
            tabSales.Controls.Add(panel);
        }

        private void CreatePOSTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20) };

            var header = ModernUIFactory.CreateTopHero("Point of Sale - Order Builder", "Build and complete customer orders");
            header.Location = new Point(0, 0);

            var searchPanel = new Panel { Location = new Point(0, 170), Size = new Size(500, 100), BackColor = Color.Transparent };
            txtOrderProductSearch = ModernUIFactory.CreateInputBox("Search product...");
            txtOrderProductSearch.Size = new Size(250, 40);
            txtOrderProductSearch.Location = new Point(0, 0);

            var lblOrLabel = new Label { Text = "OR", ForeColor = ModernUIFactory.TextSecondary, AutoSize = true, Location = new Point(0, 50) };

            cmbOrderProducts = new ComboBox
            {
                Location = new Point(0, 70),
                Size = new Size(250, 36),
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(16, 22, 38),
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.BodyFont,
            };

            searchPanel.Controls.AddRange(new Control[] { txtOrderProductSearch, lblOrLabel, cmbOrderProducts });

            var addPanel = new Panel { Location = new Point(520, 170), Size = new Size(350, 100), BackColor = Color.Transparent };
            var lblAmount = new Label { Text = "Quantity:", ForeColor = ModernUIFactory.TextPrimary, AutoSize = true, Location = new Point(0, 0) };
            numOrderAmount = new NumericUpDown
            {
                Location = new Point(0, 25),
                Size = new Size(100, 36),
                Minimum = 1,
                Value = 1,
                BackColor = Color.FromArgb(16, 22, 38),
                ForeColor = ModernUIFactory.TextPrimary,
            };

            chkPreferredCustomer = new CheckBox
            {
                Text = "Preferred Customer (10% discount)",
                ForeColor = ModernUIFactory.TextSecondary,
                BackColor = Color.FromArgb(14, 22, 44),
                Location = new Point(0, 70),
                AutoSize = true,
            };

            btnOrderAdd = ModernUIFactory.CreateAccentButton("Add to Order", (s, e) => OnAddToOrderClicked());
            btnOrderAdd.Location = new Point(200, 20);

            addPanel.Controls.AddRange(new Control[] { lblAmount, numOrderAmount, chkPreferredCustomer, btnOrderAdd });

            flpOrderItems = ModernUIFactory.CreateCardsPanel(new Point(0, 290), new Size(750, 560));

            var summaryPanel = ModernUIFactory.CreatePanelBox(new Point(770, 290), new Size(600, 560));
            summaryPanel.Controls.Add(new Label
            {
                Text = "Order Summary",
                ForeColor = ModernUIFactory.Accent,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 14),
            });

            lblOrderTotal = new Label
            {
                Text = "Total: ₪0.00",
                ForeColor = ModernUIFactory.TextPrimary,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 60),
            };

            var btnClearOrder = ModernUIFactory.CreateAccentButton("Clear Order", (s, e) => ResetOrder());
            btnClearOrder.Size = new Size(200, 45);
            btnClearOrder.Location = new Point(16, 140);
            btnClearOrder.BackColor = Color.FromArgb(150, 150, 150);

            btnOrderPlace = ModernUIFactory.CreateAccentButton("Complete Order (DoOrder)", (s, e) => OnOrderPlaceClicked());
            btnOrderPlace.Size = new Size(200, 45);
            btnOrderPlace.Location = new Point(16, 200);
            btnOrderPlace.BackColor = Color.FromArgb(0, 180, 150);

            summaryPanel.Controls.AddRange(new Control[] { lblOrderTotal, btnClearOrder, btnOrderPlace });

            panel.Controls.AddRange(new Control[] { header, searchPanel, addPanel, flpOrderItems, summaryPanel });
            tabPOS.Controls.Add(panel);
        }

        private DataGridView CreateDataGrid()
        {
            var dgv = new DataGridView
            {
                BackgroundColor = Color.FromArgb(16, 22, 38),
                ForeColor = ModernUIFactory.TextPrimary,
                GridColor = ModernUIFactory.CardBorder,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(14, 22, 44),
                    ForeColor = ModernUIFactory.Accent,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                }
            };
            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(16, 25, 44),
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.BodyFont,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
            };
            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(20, 30, 50),
            };
            return dgv;
        }

        private void LoadAllData()
        {
            LoadCustomers();
            LoadProducts();
            LoadSales();
        }

        private void LoadCustomers()
        {
            try
            {
                var query = txtCustomerFilter.Text.Trim().ToLower();
                var list = bl.Client.ReadAll();
                if (!string.IsNullOrEmpty(query))
                {
                    list = list.Where(c => c.Name.ToLower().Contains(query) || c.Id.ToString() == query).ToList();
                }
                dgvCustomers.DataSource = list.ToList();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void LoadProducts()
        {
            try
            {
                var query = txtProductFilter.Text.Trim().ToLower();
                var list = bl.Product.ReadAll().ToList();

                if (cmbProductCategoryFilter.SelectedIndex > 0)
                {
                    var categoryText = cmbProductCategoryFilter.SelectedItem?.ToString() ?? string.Empty;
                    if (Enum.TryParse<Category>(categoryText, out var category))
                    {
                        list = list.Where(p => p.Category == category).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(query))
                {
                    list = list.Where(p => p.ProductName.ToLower().Contains(query) || p.Id.ToString() == query).ToList();
                }

                dgvProducts.DataSource = list;

                cmbOrderProducts.DataSource = bl.Product.ReadAll().ToList();
                cmbOrderProducts.DisplayMember = "ProductName";
                cmbOrderProducts.ValueMember = "Id";
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void LoadSales()
        {
            try
            {
                var query = txtSaleFilter.Text.Trim().ToLower();
                var list = bl.Sale.ReadAll();
                if (!string.IsNullOrEmpty(query))
                {
                    list = list.Where(s => s.ProductId.ToString() == query || s.Id.ToString() == query).ToList();
                }
                dgvSales.DataSource = list.ToList();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void LoadOrderItems()
        {
            try
            {
                bl.Order.CalcTotalPrice(currentOrder);
                flpOrderItems.Controls.Clear();
                if (currentOrder.Items != null)
                {
                    foreach (var item in currentOrder.Items)
                    {
                        var tile = ModernUIFactory.CreateOrderItemCard(item);
                        flpOrderItems.Controls.Add(tile);
                    }
                }
                lblOrderTotal.Text = $"Total: ₪{currentOrder.TotalPrice:N2}";
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void PopulateCustomerForm()
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is Client client)
            {
                txtCustomerId.Text = client.Id.ToString();
                txtCustomerName.Text = client.Name;
                txtCustomerAddress.Text = client.Address;
                txtCustomerPhone.Text = client.PhoneNumber;
            }
        }

        private void PopulateProductForm()
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is Product product)
            {
                txtProductId.Text = product.Id.ToString();
                txtProductName.Text = product.ProductName;
                cmbProductCategory.SelectedItem = product.Category.ToString();
                txtProductPrice.Text = product.Price.ToString();
                txtProductAmount.Text = product.Ammount.ToString();
            }
        }

        private void PopulateSaleForm()
        {
            if (dgvSales.CurrentRow?.DataBoundItem is Sale sale)
            {
                txtSaleId.Text = sale.Id.ToString();
                txtSaleProductId.Text = sale.ProductId.ToString();
                txtSaleAmountRequired.Text = sale.AmountRequired.ToString();
                txtSaleTotalPrice.Text = sale.TotalPrice.ToString();
                chkSaleClubMembers.Checked = sale.ClubMembers;
                dtpSaleStart.Value = sale.StartDate;
                dtpSaleEnd.Value = sale.EndDate;
            }
        }

        private void ExecuteCustomerAction(CustomerAction action)
        {
            try
            {
                var client = new Client
                {
                    Id = int.TryParse(txtCustomerId.Text, out var id) ? id : 0,
                    Name = txtCustomerName.Text,
                    Address = txtCustomerAddress.Text,
                    PhoneNumber = txtCustomerPhone.Text,
                };

                switch (action)
                {
                    case CustomerAction.Create:
                        bl.Client.Create(client);
                        ShowInfo("Customer created successfully");
                        break;
                    case CustomerAction.Update:
                        bl.Client.Update(client);
                        ShowInfo("Customer updated successfully");
                        break;
                    case CustomerAction.Delete:
                        bl.Client.Delete(client.Id);
                        ShowInfo("Customer deleted successfully");
                        break;
                }
                LoadCustomers();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ExecuteProductAction(ProductAction action)
        {
            try
            {
                var product = new Product
                {
                    Id = int.TryParse(txtProductId.Text, out var id) ? id : 0,
                    ProductName = txtProductName.Text,
                    Category = Enum.TryParse<Category>(cmbProductCategory.SelectedItem?.ToString(), out var cat) ? cat : Category.LIGHTING,
                    Price = double.TryParse(txtProductPrice.Text, out var price) ? price : 0,
                    Ammount = int.TryParse(txtProductAmount.Text, out var amt) ? amt : 0,
                };

                switch (action)
                {
                    case ProductAction.Create:
                        bl.Product.Create(product);
                        ShowInfo("Product created successfully");
                        break;
                    case ProductAction.Update:
                        bl.Product.Update(product);
                        ShowInfo("Product updated successfully");
                        break;
                    case ProductAction.Delete:
                        bl.Product.Delete(product.Id);
                        ShowInfo("Product deleted successfully");
                        break;
                }
                LoadProducts();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ExecuteSaleAction(SaleAction action)
        {
            try
            {
                var sale = new Sale
                {
                    Id = int.TryParse(txtSaleId.Text, out var id) ? id : 0,
                    ProductId = int.TryParse(txtSaleProductId.Text, out var pid) ? pid : 0,
                    AmountRequired = int.TryParse(txtSaleAmountRequired.Text, out var amt) ? amt : 0,
                    TotalPrice = double.TryParse(txtSaleTotalPrice.Text, out var price) ? price : 0,
                    ClubMembers = chkSaleClubMembers.Checked,
                    StartDate = dtpSaleStart.Value,
                    EndDate = dtpSaleEnd.Value,
                };

                switch (action)
                {
                    case SaleAction.Create:
                        bl.Sale.Create(sale);
                        ShowInfo("Promotion created successfully");
                        break;
                    case SaleAction.Update:
                        bl.Sale.Update(sale);
                        ShowInfo("Promotion updated successfully");
                        break;
                    case SaleAction.Delete:
                        bl.Sale.Delete(sale.Id);
                        ShowInfo("Promotion deleted successfully");
                        break;
                }
                LoadSales();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void OnAddToOrderClicked()
        {
            try
            {
                if (cmbOrderProducts.SelectedItem is Product product)
                {
                    int amount = (int)numOrderAmount.Value;
                    bl.Order.AddProductToOrder(currentOrder, product.Id, amount);
                    currentOrder.IsPreferredCustomer = chkPreferredCustomer.Checked;
                    bl.Order.CalcTotalPrice(currentOrder);
                    LoadOrderItems();
                    ShowInfo($"{product.ProductName} added to order");
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void OnOrderPlaceClicked()
        {
            try
            {
                if (currentOrder.Items == null || !currentOrder.Items.Any())
                {
                    ShowError(new InvalidOperationException("Add products before completing the order"));
                    return;
                }
                bl.Order.DoOrder(currentOrder);
                ShowInfo("Order completed successfully");
                ResetOrder();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ResetOrder()
        {
            currentOrder = new Order { IsPreferredCustomer = chkPreferredCustomer?.Checked ?? false };
            LoadOrderItems();
            if (cmbOrderProducts?.Items.Count > 0)
            {
                cmbOrderProducts.SelectedIndex = 0;
            }
            numOrderAmount.Value = 1;
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private enum CustomerAction { Create, Update, Delete }
        private enum ProductAction { Create, Update, Delete }
        private enum SaleAction { Create, Update, Delete }
    }
}
