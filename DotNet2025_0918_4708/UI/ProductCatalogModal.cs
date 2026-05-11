// =============================================================================
// ProductCatalogModal.cs  –  Full-screen-ish modal for cashier product catalog.
// Tile/card grid view.  Fires ProductSelected event and closes.
// No BL calls (receives product list from caller).
// =============================================================================

using BO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public class ProductCatalogModal : Form
    {
        public event EventHandler<Product>? ProductSelected;

        private SearchBox searchBox;
        private FilterComboBox cmbCategory;
        private FlowLayoutPanel flpTiles;
        private readonly List<Product> _allProducts;

        public ProductCatalogModal(List<Product> products)
        {
            _allProducts = products;
            BuildLayout();
            LoadTiles();
        }

        private void BuildLayout()
        {
            Text            = "Product Catalog";
            ClientSize      = new Size(980, 660);
            BackColor       = DS.ModalBg;
            ForeColor       = DS.TextPrimary;
            Font            = DS.BodyFont;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;

            // ── Top bar ───────────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 72,
                BackColor = DS.Surface,
                Padding   = new Padding(20, 16, 20, 0),
            };
            pnlTop.Paint += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(DS.Hairline, 1);
                e.Graphics.DrawLine(pen, 0, 71, pnlTop.Width, 71);
            };

            var lblTitle = new Label
            {
                Text      = "Choose a Product",
                ForeColor = DS.Accent,
                Font      = DS.SectionFont,
                AutoSize  = true,
                Location  = new Point(20, 20),
            };

            searchBox = new SearchBox { Width = 260, Location = new Point(300, 18) };
            searchBox.Input.TextChanged += (s, e) => LoadTiles();

            cmbCategory = new FilterComboBox { Width = 160, Location = new Point(580, 20) };
            cmbCategory.Items.Add("All");
            foreach (var cat in Enum.GetNames(typeof(Category)))
                cmbCategory.Items.Add(cat);
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += (s, e) => LoadTiles();

            var btnClose = new GlowButton
            {
                Text      = "✕",
                Size      = new Size(40, 40),
                Location  = new Point(920, 16),
                BackColor = Color.FromArgb(60, 70, 90),
                Font      = DS.SectionFont,
            };
            btnClose.Click += (s, e) => this.Close();

            pnlTop.Controls.AddRange(new Control[] { lblTitle, searchBox, cmbCategory, btnClose });

            // ── Tile grid ─────────────────────────────────────────────────
            flpTiles = new FlowLayoutPanel
            {
                Dock         = DockStyle.Fill,
                BackColor    = Color.Transparent,
                AutoScroll   = true,
                WrapContents = true,
                FlowDirection= FlowDirection.LeftToRight,
                Padding      = new Padding(20),
            };

            Controls.Add(flpTiles);
            Controls.Add(pnlTop);
        }

        private void LoadTiles()
        {
            var filter   = searchBox.Input.Text;
            var catIndex = cmbCategory.SelectedIndex;

            flpTiles.SuspendLayout();
            flpTiles.Controls.Clear();

            IEnumerable<Product> products = _allProducts;

            if (catIndex > 0 && Enum.TryParse<Category>(cmbCategory.SelectedItem?.ToString(), out var cat))
                products = products.Where(p => p.Category == cat);

            if (!string.IsNullOrWhiteSpace(filter))
                products = products.Where(p =>
                    p.ProductName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    p.Id.ToString().Contains(filter));

            foreach (var product in products)
            {
                var tile = new ProductPickerTile(product);
                tile.Selected += (s, e) =>
                {
                    ProductSelected?.Invoke(this, ((ProductPickerTile)s!).Product);
                    this.Close();
                };
                flpTiles.Controls.Add(tile);
            }

            flpTiles.ResumeLayout();
        }
    }
}
