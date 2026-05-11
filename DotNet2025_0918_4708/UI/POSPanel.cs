// =============================================================================
// POSPanel.cs  –  Cashier POS Order Builder main screen.
// Left: product selection + cart. Right: order summary + total widget + DoOrder.
// BL HOOKS annotated with "// BL:"
// =============================================================================

using BlApi;
using BO;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public class POSPanel : Panel
    {
        private readonly IBl _bl;
        private Order _currentOrder = new();

        // ── Layout ────────────────────────────────────────────────────────
        private Panel pnlHeader;
        private Panel pnlLeft;
        private Panel pnlRight;
        private Panel pnlCartHeader;
        private FlowLayoutPanel flpCartItems;
        private FlowLayoutPanel flpProductGrid;
        private Panel pnlSummary;

        // ── Left: add product area ─────────────────────────────────────────
        private SearchBox productSearch;
        private GlowButton btnOpenCatalog;
        private Label lblSelectedProduct;
        private NumericUpDown numQty;
        private CheckBox chkPreferred;
        private GlowButton btnAddToOrder;
        private Label lblOrDivider;

        // ── Right summary ──────────────────────────────────────────────────
        private TotalWidget totalWidget;
        private Label lblPromosApplied;
        private GlowButton btnDoOrder;
        private GlowButton btnClearOrder;

        // ── Selected product from catalog ─────────────────────────────────
        private Product? _selectedProduct;

        public POSPanel(IBl bl)
        {
            _bl       = bl;
            BackColor = DS.Background;
            BuildLayout();
            RefreshCart();
        }

        // ── Layout ────────────────────────────────────────────────────────

        private void BuildLayout()
        {
            pnlHeader = PanelHelper.MakeHeader("Point of Sale",
                "Build orders — select products, apply promotions, and complete sales  ·  Center District");
            pnlHeader.Dock   = DockStyle.Top;
            pnlHeader.Height = 90;

            // Left column
            pnlLeft = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 780,
                BackColor = Color.Transparent,
                Padding   = new Padding(20, 10, 10, 10),
            };
            BuildLeftArea();

            // Right column
            pnlRight = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding   = new Padding(10, 10, 20, 10),
            };
            BuildRightArea();

            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            Controls.Add(pnlHeader);
        }

        private void BuildLeftArea()
        {
            // ── Product selection bar ──────────────────────────────────────
            var pnlSelectBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 130,
                BackColor = DS.CardBg,
                Padding   = new Padding(16),
            };
            pnlSelectBar.Paint += PaintBorderPanel;

            var lblSelectTitle = new Label
            {
                Text      = "Select Product",
                ForeColor = DS.Accent,
                Font      = DS.BodyBold,
                AutoSize  = true,
                Location  = new Point(16, 12),
            };

            productSearch = new SearchBox
            {
                Location = new Point(16, 38),
                Width    = 260,
            };
            productSearch.Input.TextChanged += OnProductSearchChanged;
            productSearch.Input.KeyDown += OnProductSearchKeyDown;

            lblOrDivider = new Label
            {
                Text      = "or",
                ForeColor = DS.TextHint,
                Font      = DS.CaptionFont,
                AutoSize  = true,
                Location  = new Point(284, 48),
            };

            btnOpenCatalog = new GlowButton
            {
                Text     = "Browse Catalog",
                Size     = new Size(150, 38),
                Location = new Point(304, 38),
            };
            btnOpenCatalog.Click += (s, e) => OpenProductCatalog();

            lblSelectedProduct = new Label
            {
                Text      = "No product selected",
                ForeColor = DS.TextHint,
                Font      = DS.CaptionFont,
                AutoSize  = false,
                Size      = new Size(680, 48),
                Location  = new Point(16, 88),
            };

            var lblQty = new Label
            {
                Text      = "Qty:",
                ForeColor = DS.TextMuted,
                Font      = DS.CaptionFont,
                AutoSize  = true,
                Location  = new Point(460, 44),
            };

            numQty = new NumericUpDown
            {
                Minimum   = 1,
                Maximum   = 999,
                Value     = 1,
                Location  = new Point(490, 40),
                Size      = new Size(70, 30),
                BackColor = DS.CardBg,
                ForeColor = DS.TextPrimary,
                Font      = DS.BodyFont,
            };

            chkPreferred = new CheckBox
            {
                Text      = "Preferred customer",
                ForeColor = DS.TextMuted,
                BackColor = Color.Transparent,
                AutoSize  = true,
                Location  = new Point(460, 82),
                Font      = DS.CaptionFont,
            };
            chkPreferred.CheckedChanged += (s, e) =>
            {
                // BL: recalculate order on preference change
                _currentOrder.IsPreferredCustomer = chkPreferred.Checked;
                RecalcOrder();
            };

            btnAddToOrder = new GlowButton
            {
                Text     = "+ Add to Order",
                Size     = new Size(160, 38),
                Location = new Point(570, 38),
            };
            btnAddToOrder.Click += OnAddToOrderClicked;

            pnlSelectBar.Controls.AddRange(new Control[]
            { lblSelectTitle, productSearch, lblOrDivider, btnOpenCatalog,
              lblSelectedProduct, lblQty, numQty, chkPreferred, btnAddToOrder });

            // ── Compact product grid (quick-add tiles) ──────────────
            flpProductGrid = new FlowLayoutPanel
            {
                Dock         = DockStyle.Top,
                Height       = 360,
                BackColor    = Color.Transparent,
                AutoScroll   = true,
                WrapContents = true,
                FlowDirection= FlowDirection.LeftToRight,
                Padding      = new Padding(8),
            };

            try
            {
                var products = _bl.Product.ReadAll().ToList();
                // Limit initial tiles to avoid overcrowding
                foreach (var p in products.Take(120))
                {
                    var tile = new ProductPickerTile(p);
                    tile.Selected += (s, e) =>
                    {
                        _selectedProduct = p;
                        productSearch.Input.Text = p.ProductName;
                        UpdateSelectedLabel();
                    };
                    flpProductGrid.Controls.Add(tile);
                }
            }
            catch { /* safe-fail: grid stays empty if BL unavailable */ }

            // ── Cart list ──────────────────────────────────────────────────
            pnlCartHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 40,
                BackColor = Color.Transparent,
            };
            pnlCartHeader.Paint += (s, e) =>
            {
                using var headerBrush = new SolidBrush(DS.TextMuted);
                e.Graphics.DrawString("Shopping Cart", DS.BodyBold, headerBrush, new PointF(4, 10));
                using var line = new System.Drawing.Pen(DS.Hairline, 1);
                e.Graphics.DrawLine(line, 0, pnlCartHeader.Height - 1, pnlCartHeader.Width, pnlCartHeader.Height - 1);
            };

            flpCartItems = new FlowLayoutPanel
            {
                Dock         = DockStyle.Fill,
                BackColor    = Color.Transparent,
                AutoScroll   = true,
                WrapContents = false,
                FlowDirection= FlowDirection.TopDown,
                Padding      = new Padding(0, 8, 0, 0),
            };

            pnlLeft.Controls.Add(flpCartItems);
            pnlLeft.Controls.Add(pnlCartHeader);
            pnlLeft.Controls.Add(flpProductGrid);
            pnlLeft.Controls.Add(pnlSelectBar);
        }

        private void BuildRightArea()
        {
            pnlSummary = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = DS.CardBg,
                Padding   = new Padding(24),
            };
            pnlSummary.Paint += PaintBorderPanel;

            var lblSummaryTitle = new Label
            {
                Text      = "Order Summary",
                ForeColor = DS.Accent,
                Font      = DS.SectionFont,
                AutoSize  = true,
                Location  = new Point(24, 20),
            };

            totalWidget = new TotalWidget
            {
                Location = new Point(24, 60),
            };

            lblPromosApplied = new Label
            {
                Text      = "No promotions applied",
                ForeColor = DS.TextHint,
                Font      = DS.CaptionFont,
                AutoSize  = false,
                Size      = new Size(340, 60),
                Location  = new Point(24, 198),
            };

            var divider = new Panel
            {
                Location  = new Point(24, 270),
                Size      = new Size(340, 1),
                BackColor = DS.Hairline,
            };

            btnDoOrder = new GlowButton
            {
                Text      = "✓  COMPLETE ORDER",
                Size      = new Size(340, 64),
                Location  = new Point(24, 284),
                BackColor = DS.Success,
                Font      = DS.SectionFont,
            };
            btnDoOrder.Click += OnDoOrderClicked;

            btnClearOrder = new GlowButton
            {
                Text      = "Clear Order",
                Size      = new Size(340, 40),
                Location  = new Point(24, 362),
                BackColor = Color.FromArgb(60, 70, 90),
            };
            btnClearOrder.Click += (s, e) => ResetOrder();

            pnlSummary.Controls.AddRange(new Control[]
            { lblSummaryTitle, totalWidget, lblPromosApplied, divider, btnDoOrder, btnClearOrder });

            pnlRight.Controls.Add(pnlSummary);
        }

        // ── Product Search (type-to-filter) ───────────────────────────────

        private void OnProductSearchChanged(object? sender, EventArgs e)
        {
            var text = productSearch.Input.Text;
            if (string.IsNullOrWhiteSpace(text)) { _selectedProduct = null; UpdateSelectedLabel(); return; }

            try
            {
                // BL: bl.Product.ReadAll() with filter
                var match = _bl.Product.ReadAll()
                    .FirstOrDefault(p => p.ProductName.Contains(text, StringComparison.OrdinalIgnoreCase)
                                     || p.Id.ToString() == text);
                _selectedProduct = match;
                UpdateSelectedLabel();
            }
            catch { _selectedProduct = null; UpdateSelectedLabel(); }
        }

        private void UpdateSelectedLabel()
        {
            if (_selectedProduct == null)
            {
                lblSelectedProduct.Text = "No product selected";
                lblSelectedProduct.ForeColor = DS.TextHint;
                return;
            }

            // Basic details
            var lines = new System.Text.StringBuilder();
            lines.Append($"✓  {_selectedProduct.ProductName}  ·  ₪{_selectedProduct.Price:N2}  ·  Stock: {_selectedProduct.Ammount}");
            lines.AppendLine();
            lines.Append($"ID: {_selectedProduct.Id}  ·  Category: {DS.CategoryLabel(_selectedProduct.Category)}");

            try
            {
                // Show available promotions for the selected quantity and preference
                var temp = new BO.ProductInOrder
                {
                    ProductId = _selectedProduct.Id,
                    ProductName = _selectedProduct.ProductName,
                    BasePrice = _selectedProduct.Price,
                    Amount = (int)numQty.Value,
                };
                _bl.Product.GetValidSales(temp, chkPreferred.Checked);

                if (temp.Sales != null && temp.Sales.Any())
                {
                    var promoLines = string.Join(", ", temp.Sales.Select(s => $"{s.AmountRequired} → ₪{s.SalePrice:N2}"));
                    lines.AppendLine();
                    lines.Append($"Promos: {promoLines}");
                }
            }
            catch { /* ignore BL lookup errors for display */ }

            lblSelectedProduct.Text = lines.ToString();
            lblSelectedProduct.ForeColor = DS.Accent;
        }

        private void OnProductSearchKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.Handled = true;
            e.SuppressKeyPress = true;

            var text = productSearch.Input.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            try
            {
                // Try by id first
                BO.Product? found = null;
                if (int.TryParse(text, out var id))
                {
                    found = _bl.Product.Read(id);
                }

                // Fallback by exact or contains name
                if (found == null)
                {
                    found = _bl.Product.ReadAll()
                        .FirstOrDefault(p => p.ProductName.Equals(text, StringComparison.OrdinalIgnoreCase)
                                          || p.ProductName.Contains(text, StringComparison.OrdinalIgnoreCase));
                }

                if (found == null)
                {
                    MessageBox.Show("Product not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _selectedProduct = found;
                UpdateSelectedLabel();

                // Add default qty to order for quick POS flow
                var qty = (int)numQty.Value;
                _bl.Order.AddProductToOrder(_currentOrder, _selectedProduct.Id, qty);
                _currentOrder.IsPreferredCustomer = chkPreferred.Checked;
                RecalcOrder();
                numQty.Value = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── Catalog modal ─────────────────────────────────────────────────

        private void OpenProductCatalog()
        {
            try
            {
                // BL: bl.Product.ReadAll()
                var products = _bl.Product.ReadAll().ToList();
                var catalog  = new ProductCatalogModal(products);
                catalog.ProductSelected += (s, product) =>
                {
                    _selectedProduct = product;
                    productSearch.Input.Text = product.ProductName;
                    UpdateSelectedLabel();
                };
                catalog.ShowDialog(this);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        // ── Add to order ──────────────────────────────────────────────────

        private void OnAddToOrderClicked(object? sender, EventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product first.", "No Product", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var qty = (int)numQty.Value;
                // BL: bl.Order.AddProductToOrder(order, productId, qty)
                _bl.Order.AddProductToOrder(_currentOrder, _selectedProduct.Id, qty);
                _currentOrder.IsPreferredCustomer = chkPreferred.Checked;
                RecalcOrder();
                numQty.Value = 1;
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Add Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        // ── Remove from order ─────────────────────────────────────────────

        private void RemoveItem(ProductInOrder item)
        {
            // BL: AddProductToOrder with negative delta to remove
            try
            {
                _bl.Order.AddProductToOrder(_currentOrder, item.ProductId, -item.Amount);
                RecalcOrder();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Remove Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        // ── Recalc & refresh cart ─────────────────────────────────────────

        private void RecalcOrder()
        {
            // BL: bl.Order.CalcTotalPrice(order)
            _bl.Order.CalcTotalPrice(_currentOrder);
            RefreshCart();
        }

        private void RefreshCart()
        {
            flpCartItems.SuspendLayout();
            flpCartItems.Controls.Clear();

            foreach (var item in _currentOrder.Items)
            {
                var card = new OrderItemCard(item);
                card.Width        = flpCartItems.Width - 32;
                card.RemoveClicked += (s, e) => RemoveItem(item);
                flpCartItems.Controls.Add(card);
            }

            flpCartItems.ResumeLayout();

            // Update total widget
            totalWidget.Total = _currentOrder.TotalPrice;

            // Promo summary
            var promoItems = _currentOrder.Items.Where(i => i.Sales?.Count > 0).ToList();
            if (promoItems.Count > 0)
            {
                var names = string.Join(", ", promoItems.Select(i => i.ProductName));
                lblPromosApplied.Text      = $"🏷 Promos applied on: {names}";
                lblPromosApplied.ForeColor = DS.Warning;
            }
            else
            {
                lblPromosApplied.Text      = "No promotions applied";
                lblPromosApplied.ForeColor = DS.TextHint;
            }
        }

        // ── DoOrder ───────────────────────────────────────────────────────

        private void OnDoOrderClicked(object? sender, EventArgs e)
        {
            if (!_currentOrder.Items.Any())
            {
                MessageBox.Show("Cart is empty. Add products before completing the order.",
                    "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Complete order?\nTotal: ₪{_currentOrder.TotalPrice:N2}",
                "Confirm Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // BL: bl.Order.DoOrder(order)
                _bl.Order.DoOrder(_currentOrder);
                MessageBox.Show(
                    $"✓ Order completed successfully!\nTotal charged: ₪{_currentOrder.TotalPrice:N2}",
                    "Order Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetOrder();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Order Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void ResetOrder()
        {
            _currentOrder              = new Order { IsPreferredCustomer = chkPreferred.Checked };
            _selectedProduct           = null;
            productSearch.Input.Text   = "";
            numQty.Value               = 1;
            UpdateSelectedLabel();
            RefreshCart();
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static void PaintBorderPanel(object? sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (sender is not Panel panel) return;
            using var pen = new System.Drawing.Pen(DS.Hairline, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Control c in flpCartItems.Controls)
                c.Width = flpCartItems.Width - 32;
        }
    }
}
