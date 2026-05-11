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
        private readonly List<ProductInOrder> cartItems = new();
        private readonly List<Product> productsCache = new();

        private TabControl tabControl = null!;
        private TabPage tabHome = null!;
        private TabPage tabShop = null!;
        private TabPage tabCart = null!;
        private TabPage tabAdmin = null!;

        private DataGridView dgvCustomers = null!;
        private TextBox txtCustomerFilter = null!;
        private TextBox txtCustomerId = null!;
        private TextBox txtCustomerName = null!;
        private TextBox txtCustomerAddress = null!;
        private TextBox txtCustomerPhone = null!;

        private FlowLayoutPanel flpProductCards = null!;
        private TextBox txtProductFilter = null!;
        private ComboBox cmbProductCategoryFilter = null!;
        private TextBox txtProductId = null!;
        private TextBox txtProductName = null!;
        private ComboBox cmbProductCategory = null!;
        private TextBox txtProductPrice = null!;
        private TextBox txtProductAmount = null!;

        private DataGridView dgvSales = null!;
        private TextBox txtSaleFilter = null!;
        private TextBox txtSaleId = null!;
        private TextBox txtSaleProductId = null!;
        private TextBox txtSaleAmountRequired = null!;
        private TextBox txtSaleTotalPrice = null!;
        private CheckBox chkSaleClubMembers = null!;
        private DateTimePicker dtpSaleStart = null!;
        private DateTimePicker dtpSaleEnd = null!;

        private FlowLayoutPanel flpOrderCards = null!;
        private ComboBox cmbOrderProducts = null!;
        private TextBox txtOrderProductId = null!;
        private NumericUpDown numOrderAmount = null!;
        private CheckBox chkPreferredCustomer = null!;
        private Label lblOrderTotal = null!;
        private Button btnOrderAdd = null!;
        private Button btnOrderPlace = null!;
        private Button btnOrderReset = null!;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
            LoadAllData();
        }

        private void InitializeUI()
        {
            ModernUIFactory.StyleForm(this);
            ClientSize = new Size(1360, 860);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = ModernUIFactory.AppBackground,
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.BodyFont,
            };

            tabHome = new TabPage("Home") { BackColor = ModernUIFactory.AppBackground };
            tabShop = new TabPage("Shop") { BackColor = ModernUIFactory.AppBackground };
            tabCart = new TabPage("Cart") { BackColor = ModernUIFactory.AppBackground };
            tabAdmin = new TabPage("Admin") { BackColor = ModernUIFactory.AppBackground };

            tabControl.TabPages.AddRange(new TabPage[] { tabHome, tabShop, tabCart, tabAdmin });

            Controls.Add(tabControl);

            CreateHomeTab();
            CreateShopTab();
            CreateCartTab();
            CreateAdminTab();
        }

        private void CreateHomeTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(40) };
            var welcome = new Label
            {
                Text = "Welcome to SmartHome Store",
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(20, 20),
            };

            var description = new Label
            {
                Text = "Discover the latest in smart home technology. Browse our catalog of lighting, security, climate control, and audio devices.",
                ForeColor = Color.FromArgb(176, 196, 255),
                Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                MaximumSize = new Size(800, 0),
                Location = new Point(20, 80),
            };

            var btnShopNow = ModernUIFactory.CreateAccentButton("Shop Now", (s, e) => tabControl.SelectedTab = tabShop);
            btnShopNow.Size = new Size(200, 50);
            btnShopNow.Location = new Point(20, 160);

            var features = new Panel
            {
                Location = new Point(20, 240),
                Size = new Size(1200, 400),
                BackColor = Color.Transparent,
            };

            features.Controls.Add(CreateFeatureCard("Smart Lighting", "Control your home's ambiance with intelligent bulbs and switches.", new Point(0, 0)));
            features.Controls.Add(CreateFeatureCard("Home Security", "Protect your family with advanced cameras and sensors.", new Point(400, 0)));
            features.Controls.Add(CreateFeatureCard("Climate Control", "Maintain perfect comfort with smart thermostats.", new Point(800, 0)));
            features.Controls.Add(CreateFeatureCard("Audio Systems", "Immerse yourself in premium sound experiences.", new Point(200, 200)));
            features.Controls.Add(CreateFeatureCard("Easy Setup", "Get started quickly with our user-friendly installation guides.", new Point(600, 200)));

            panel.Controls.AddRange(new Control[] { welcome, description, btnShopNow, features });
            tabHome.Controls.Add(panel);
        }

        private Panel CreateFeatureCard(string title, string description, Point location)
        {
            var card = new Panel
            {
                Size = new Size(380, 160),
                Location = location,
                BackColor = Color.FromArgb(18, 28, 52),
                BorderStyle = BorderStyle.None,
            };
            card.Paint += (sender, args) =>
            {
                using var pen = new Pen(ModernUIFactory.AccentSoft, 2);
                args.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            card.Controls.Add(new Label
            {
                Text = title,
                ForeColor = ModernUIFactory.Accent,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(20, 20),
            });

            card.Controls.Add(new Label
            {
                Text = description,
                ForeColor = ModernUIFactory.TextSecondary,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                MaximumSize = new Size(340, 0),
                Location = new Point(20, 60),
            });

            return card;
        }

        private void CreateShopTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20) };

            var header = ModernUIFactory.CreateTopHero("SmartHome Catalog", "Browse our collection of premium smart home devices");
            header.Location = new Point(0, 0);

            var searchPanel = new Panel
            {
                Location = new Point(0, 180),
                Size = new Size(1320, 60),
                BackColor = Color.Transparent,
            };

            var txtSearch = ModernUIFactory.CreateInputBox("Search products...");
            txtSearch.Location = new Point(0, 10);
            txtSearch.Size = new Size(300, 40);
            txtSearch.TextChanged += (s, e) => FilterProducts();

            var cmbCategory = new ComboBox
            {
                Location = new Point(320, 10),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(16, 22, 38),
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.BodyFont,
            };
            cmbCategory.Items.Add("All Categories");
            cmbCategory.Items.AddRange(Enum.GetNames(typeof(Category)));
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += (s, e) => FilterProducts();

            var btnCart = ModernUIFactory.CreateAccentButton("View Cart", (s, e) => tabControl.SelectedTab = tabCart);
            btnCart.Location = new Point(1100, 10);
            btnCart.Text = $"Cart ({cartItems.Count})";

            searchPanel.Controls.AddRange(new Control[] { txtSearch, cmbCategory, btnCart });

            flpProductCards = ModernUIFactory.CreateCardsPanel(new Point(0, 260), new Size(1320, 520));

            panel.Controls.AddRange(new Control[] { header, searchPanel, flpProductCards });
            tabShop.Controls.Add(panel);
        }

        private void CreateCartTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20) };

            var header = ModernUIFactory.CreateTopHero("Your Shopping Cart", "Review your selected smart home devices");
            header.Location = new Point(0, 0);

            flpOrderCards = ModernUIFactory.CreateCardsPanel(new Point(0, 180), new Size(1000, 500));

            var summaryPanel = ModernUIFactory.CreatePanelBox(new Point(1020, 180), new Size(280, 300));
            summaryPanel.Controls.Add(new Label
            {
                Text = "Order Summary",
                ForeColor = ModernUIFactory.Accent,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(14, 14),
            });

            lblOrderTotal = new Label
            {
                Text = "Total: ₪0.00",
                ForeColor = ModernUIFactory.TextPrimary,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(14, 50),
            };

            var btnCheckout = ModernUIFactory.CreateAccentButton("Checkout", OnCheckoutClicked);
            btnCheckout.Size = new Size(200, 45);
            btnCheckout.Location = new Point(40, 100);

            var btnClearCart = ModernUIFactory.CreateAccentButton("Clear Cart", (s, e) => { cartItems.Clear(); UpdateCartDisplay(); });
            btnClearCart.Size = new Size(200, 45);
            btnClearCart.Location = new Point(40, 160);
            btnClearCart.BackColor = Color.FromArgb(100, 100, 100);

            summaryPanel.Controls.AddRange(new Control[] { lblOrderTotal, btnCheckout, btnClearCart });

            var btnContinueShopping = ModernUIFactory.CreateAccentButton("Continue Shopping", (s, e) => tabControl.SelectedTab = tabShop);
            btnContinueShopping.Location = new Point(0, 700);

            panel.Controls.AddRange(new Control[] { header, flpOrderCards, summaryPanel, btnContinueShopping });
            tabCart.Controls.Add(panel);
        }

        private void CreateAdminTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20) };

            var header = ModernUIFactory.CreateTopHero("Store Administration", "Manage customers, products, promotions and orders");
            header.Location = new Point(0, 0);

            // Add all the admin panels here - simplified for now
            var lblAdmin = new Label
            {
                Text = "Admin features coming soon...",
                ForeColor = ModernUIFactory.TextPrimary,
                Font = ModernUIFactory.SectionFont,
                AutoSize = true,
                Location = new Point(20, 200),
            };

            panel.Controls.AddRange(new Control[] { header, lblAdmin });
            tabAdmin.Controls.Add(panel);
        }

        private void LoadAllData()
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                productsCache.Clear();
                productsCache.AddRange(bl.Product.ReadAll());

                flpProductCards.Controls.Clear();
                foreach (var product in productsCache)
                {
                    var tile = ModernUIFactory.CreateShopProductTile(product, OnAddToCartClicked);
                    flpProductCards.Controls.Add(tile);
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void FilterProducts()
        {
            try
            {
                var query = string.Empty;
                var categoryFilter = Category.LIGHTING; // default, will be overridden

                // Find the search textbox and category combo in shop tab
                foreach (Control ctrl in tabShop.Controls)
                {
                    if (ctrl is Panel panel)
                    {
                        foreach (Control subCtrl in panel.Controls)
                        {
                            if (subCtrl is Panel searchPanel && searchPanel.Controls.Count > 0)
                            {
                                foreach (Control item in searchPanel.Controls)
                                {
                                    if (item is TextBox txt && txt.Tag?.ToString() == "Search products...")
                                    {
                                        query = txt.Text.Trim().ToLower();
                                    }
                                    else if (item is ComboBox cmb && cmb.Items.Contains("All Categories"))
                                    {
                                        if (cmb.SelectedIndex > 0)
                                        {
                                            var categoryText = cmb.SelectedItem?.ToString() ?? string.Empty;
                                            if (Enum.TryParse<Category>(categoryText, out var cat))
                                            {
                                                categoryFilter = cat;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var list = productsCache.AsEnumerable();
                if (!string.IsNullOrEmpty(query))
                {
                    list = list.Where(p => p.ProductName.ToLower().Contains(query) || p.Id.ToString() == query);
                }
                if (categoryFilter != Category.LIGHTING)
                {
                    list = list.Where(p => p.Category == categoryFilter);
                }

                flpProductCards.Controls.Clear();
                foreach (var product in list)
                {
                    var tile = ModernUIFactory.CreateShopProductTile(product, OnAddToCartClicked);
                    flpProductCards.Controls.Add(tile);
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void OnAddToCartClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Product product)
            {
                var existing = cartItems.FirstOrDefault(i => i.ProductId == product.Id);
                if (existing != null)
                {
                    existing.Amount++;
                }
                else
                {
                    cartItems.Add(new ProductInOrder
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Amount = 1,
                        BasePrice = product.Price,
                        FinalPrice = product.Price
                    });
                }
                UpdateCartDisplay();
                MessageBox.Show($"{product.ProductName} added to cart!", "SmartHome Store", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateCartDisplay()
        {
            // Update cart tab and cart button text
            foreach (Control ctrl in tabShop.Controls)
            {
                if (ctrl is Panel panel)
                {
                    foreach (Control subCtrl in panel.Controls)
                    {
                        if (subCtrl is Panel searchPanel)
                        {
                            foreach (Control item in searchPanel.Controls)
                            {
                                if (item is Button btn && btn.Text.Contains("Cart"))
                                {
                                    btn.Text = $"View Cart ({cartItems.Count})";
                                }
                            }
                        }
                    }
                }
            }
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            flpOrderCards.Controls.Clear();
            foreach (var item in cartItems)
            {
                var tile = ModernUIFactory.CreateCartItemTile(item, OnRemoveFromCartClicked);
                flpOrderCards.Controls.Add(tile);
            }
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            var total = cartItems.Sum(i => i.FinalPrice);
            lblOrderTotal.Text = $"Total: ₪{total:N2}";
        }

        private void OnRemoveFromCartClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ProductInOrder item)
            {
                cartItems.Remove(item);
                UpdateCartDisplay();
            }
        }

        private void OnCheckoutClicked(object? sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Your cart is empty!", "SmartHome Store", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Simple checkout - in real app would collect customer info
            var total = cartItems.Sum(i => i.FinalPrice);
            var message = $"Order Total: ₪{total:N2}\n\nItems:\n" +
                         string.Join("\n", cartItems.Select(i => $"{i.ProductName} x{i.Amount} - ₪{i.FinalPrice:N2}"));

            if (MessageBox.Show($"{message}\n\nProceed with checkout?", "Checkout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Here would normally save the order
                cartItems.Clear();
                UpdateCartDisplay();
                MessageBox.Show("Order placed successfully!", "SmartHome Store", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabControl.SelectedTab = tabHome;
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "SmartHome Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "SmartHome", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
