// =============================================================================
// AdminProductsPanel.cs  –  Product catalog management panel (card layout).
// BL HOOKS annotated with "// BL:"
// =============================================================================

using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public class AdminProductsPanel : Panel
    {
        private readonly IBl _bl;

        private Panel pnlHeader;
        private Panel pnlToolbar;
        private FlowLayoutPanel flpCards;
        private Panel pnlEditor;

        private SearchBox searchBox;
        private FilterComboBox cmbCategoryFilter;
        private GlowButton btnAddNew;

        // Editor fields
        private Label lblEditorTitle;
        private GlowInputPanel inpId;
        private GlowInputPanel inpName;
        private GlowInputPanel inpPrice;
        private GlowInputPanel inpAmount;
        private Label lblCategory;
        private FilterComboBox cmbCategory;
        private GlowButton btnSave;
        private GlowButton btnDelete;
        private GlowButton btnCancel;

        private Product? _editingProduct;

        public AdminProductsPanel(IBl bl)
        {
            _bl       = bl;
            BackColor = DS.Background;
            BuildLayout();
            // BL: initial data load
            LoadCards();
        }

        private void BuildLayout()
        {
            pnlHeader = PanelHelper.MakeHeader("Product Catalog",
                "Smart home product inventory — add, edit, and manage stock");
            pnlHeader.Dock   = DockStyle.Top;
            pnlHeader.Height = 90;

            pnlToolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.Transparent,
                Padding   = new Padding(20, 10, 20, 0),
            };

            searchBox = new SearchBox { Width = 250, Location = new Point(20, 14) };
            searchBox.Input.TextChanged += (s, e) => LoadCards();

            cmbCategoryFilter = new FilterComboBox
            {
                Width    = 170,
                Location = new Point(290, 14),
            };
            cmbCategoryFilter.Items.Add("All Categories");
            foreach (var cat in Enum.GetNames(typeof(Category)))
                cmbCategoryFilter.Items.Add(cat);
            cmbCategoryFilter.SelectedIndex = 0;
            cmbCategoryFilter.SelectedIndexChanged += (s, e) => LoadCards();

            btnAddNew = new GlowButton
            {
                Text     = "+ Add Product",
                Size     = new Size(150, 38),
                Location = new Point(480, 11),
            };
            btnAddNew.Click += (s, e) => OpenEditor(null);

            pnlToolbar.Controls.AddRange(new Control[] { searchBox, cmbCategoryFilter, btnAddNew });

            flpCards = new FlowLayoutPanel
            {
                Dock         = DockStyle.Fill,
                BackColor    = Color.Transparent,
                AutoScroll   = true,
                WrapContents = true,
                FlowDirection= FlowDirection.LeftToRight,
                Padding      = new Padding(16),
            };

            pnlEditor = BuildEditorPanel();
            pnlEditor.Visible = false;

            Controls.Add(flpCards);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlHeader);
            Controls.Add(pnlEditor);
        }

        private void LoadCards()
        {
            var filter   = searchBox.Input.Text;
            var catIndex = cmbCategoryFilter?.SelectedIndex ?? 0;

            flpCards.SuspendLayout();
            flpCards.Controls.Clear();

            try
            {
                // BL: bl.Product.ReadAll()
                IEnumerable<Product> products = _bl.Product.ReadAll();

                if (catIndex > 0 && Enum.TryParse<Category>(
                        cmbCategoryFilter!.SelectedItem?.ToString(), out var cat))
                    products = products.Where(p => p.Category == cat);

                if (!string.IsNullOrWhiteSpace(filter))
                    products = products.Where(p =>
                        p.ProductName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                        p.Id.ToString().Contains(filter));

                foreach (var product in products)
                {
                    var card = new ProductCard(product);
                    card.EditClicked   += (s, e) => OpenEditor(((ProductCard)s!).Product);
                    card.DeleteClicked += (s, e) => DeleteProduct(((ProductCard)s!).Product.Id);
                    flpCards.Controls.Add(card);
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            flpCards.ResumeLayout();
        }

        private Panel BuildEditorPanel()
        {
            var panel = new Panel
            {
                Size      = new Size(400, 580),
                BackColor = DS.CardBg,
            };
            panel.Paint += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(DS.Accent, 1.5f);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                using var glow = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, panel.Width, 4), DS.Accent, Color.Transparent, 0f);
                e.Graphics.FillRectangle(glow, 0, 0, panel.Width, 4);
            };

            lblEditorTitle = new Label
            {
                Text      = "Add Product",
                ForeColor = DS.Accent,
                Font      = DS.SectionFont,
                AutoSize  = true,
                Location  = new Point(24, 20),
            };

            inpId     = new GlowInputPanel("Product ID")    { Location = new Point(24, 70),  Width = 352 };
            inpName   = new GlowInputPanel("Product Name")  { Location = new Point(24, 148), Width = 352 };
            inpPrice  = new GlowInputPanel("Price (₪)")     { Location = new Point(24, 226), Width = 352 };
            inpAmount = new GlowInputPanel("Stock Quantity") { Location = new Point(24, 304), Width = 352 };

            lblCategory = new Label
            {
                Text      = "Category",
                ForeColor = DS.TextMuted,
                Font      = DS.CaptionFont,
                AutoSize  = true,
                Location  = new Point(24, 386),
            };

            cmbCategory = new FilterComboBox
            {
                Location = new Point(24, 406),
                Width    = 352,
            };
            foreach (var cat in Enum.GetNames(typeof(Category)))
                cmbCategory.Items.Add(cat);
            cmbCategory.SelectedIndex = 0;

            btnSave = new GlowButton
            {
                Text     = "SAVE",
                Size     = new Size(352, 48),
                Location = new Point(24, 460),
            };
            btnSave.Click += OnSaveClicked;

            btnDelete = new GlowButton
            {
                Text      = "DELETE",
                Size      = new Size(168, 40),
                BackColor = DS.Danger,
                Location  = new Point(24, 520),
                Visible   = false,
            };
            btnDelete.Click += OnDeleteClicked;

            btnCancel = new GlowButton
            {
                Text      = "CANCEL",
                Size      = new Size(168, 40),
                BackColor = DS.Hairline,
                Location  = new Point(208, 520),
            };
            btnCancel.Click += (s, e) => CloseEditor();

            panel.Controls.AddRange(new Control[]
            { lblEditorTitle, inpId, inpName, inpPrice, inpAmount,
              lblCategory, cmbCategory, btnSave, btnDelete, btnCancel });
            return panel;
        }

        private void OpenEditor(Product? product)
        {
            _editingProduct = product;

            if (product == null)
            {
                lblEditorTitle.Text    = "Add Product";
                inpId.Input.Text       = "";
                inpId.Input.ReadOnly   = false;
                inpName.Input.Text     = "";
                inpPrice.Input.Text    = "";
                inpAmount.Input.Text   = "";
                cmbCategory.SelectedIndex = 0;
                btnDelete.Visible      = false;
            }
            else
            {
                lblEditorTitle.Text    = "Edit Product";
                inpId.Input.Text       = product.Id.ToString();
                inpId.Input.ReadOnly   = true;
                inpName.Input.Text     = product.ProductName;
                inpPrice.Input.Text    = product.Price.ToString();
                inpAmount.Input.Text   = product.Ammount.ToString();
                cmbCategory.SelectedItem = product.Category.ToString();
                btnDelete.Visible      = true;
            }

            pnlEditor.Location = new Point(Width - 430, 90);
            pnlEditor.BringToFront();
            pnlEditor.Visible = true;
        }

        private void CloseEditor()
        {
            pnlEditor.Visible = false;
            _editingProduct   = null;
        }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            try
            {
                var product = new Product
                {
                    Id          = int.TryParse(inpId.Input.Text, out var id) ? id : 0,
                    ProductName = inpName.Input.Text,
                    Price       = double.TryParse(inpPrice.Input.Text, out var price) ? price : 0,
                    Ammount     = int.TryParse(inpAmount.Input.Text, out var amt) ? amt : 0,
                    Category    = Enum.TryParse<Category>(cmbCategory.SelectedItem?.ToString(), out var cat)
                                  ? cat : Category.LIGHTING,
                };

                if (_editingProduct == null)
                    // BL: bl.Product.Create(product)
                    _bl.Product.Create(product);
                else
                    // BL: bl.Product.Update(product)
                    _bl.Product.Update(product);

                CloseEditor();
                LoadCards();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (_editingProduct == null) return;
            DeleteProduct(_editingProduct.Id);
        }

        private void DeleteProduct(int id)
        {
            var confirm = MessageBox.Show($"Delete product #{id}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // BL: bl.Product.Delete(id)
                _bl.Product.Delete(id);
                CloseEditor();
                LoadCards();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlEditor?.Visible == true)
                pnlEditor.Location = new Point(Width - 430, 90);
        }
    }
}
