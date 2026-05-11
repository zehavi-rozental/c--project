// =============================================================================
// AdminSalesPanel.cs  –  Promotions management panel (card layout).
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
    public class AdminSalesPanel : Panel
    {
        private readonly IBl _bl;

        private Panel pnlHeader;
        private Panel pnlToolbar;
        private FlowLayoutPanel flpCards;
        private Panel pnlEditor;

        private SearchBox searchBox;
        private GlowButton btnAddNew;

        // Editor
        private Label lblEditorTitle;
        private GlowInputPanel inpId;
        private GlowInputPanel inpProductId;
        private GlowInputPanel inpAmountRequired;
        private GlowInputPanel inpTotalPrice;
        private CheckBox chkClubMembers;
        private Label lblStart;
        private DateTimePicker dtpStart;
        private Label lblEnd;
        private DateTimePicker dtpEnd;
        private GlowButton btnSave;
        private GlowButton btnDelete;
        private GlowButton btnCancel;

        private Sale? _editingSale;

        public AdminSalesPanel(IBl bl)
        {
            _bl       = bl;
            BackColor = DS.Background;
            BuildLayout();
            // BL: initial load
            LoadCards();
        }

        private void BuildLayout()
        {
            pnlHeader = PanelHelper.MakeHeader("Promotions & Deals",
                "Manage special offers, bundle deals, and club member discounts");
            pnlHeader.Dock   = DockStyle.Top;
            pnlHeader.Height = 90;

            pnlToolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.Transparent,
                Padding   = new Padding(20, 10, 20, 0),
            };

            searchBox = new SearchBox { Width = 280, Location = new Point(20, 14) };
            searchBox.Input.TextChanged += (s, e) => LoadCards(searchBox.Input.Text);

            btnAddNew = new GlowButton
            {
                Text     = "+ Add Promotion",
                Size     = new Size(165, 38),
                Location = new Point(320, 11),
            };
            btnAddNew.Click += (s, e) => OpenEditor(null);

            pnlToolbar.Controls.AddRange(new Control[] { searchBox, btnAddNew });

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

        private void LoadCards(string filter = "")
        {
            flpCards.SuspendLayout();
            flpCards.Controls.Clear();

            try
            {
                // BL: bl.Sale.ReadAll()
                IEnumerable<Sale> sales = _bl.Sale.ReadAll();

                if (!string.IsNullOrWhiteSpace(filter))
                    sales = sales.Where(s =>
                        s.Id.ToString().Contains(filter) ||
                        s.ProductId.ToString().Contains(filter));

                foreach (var sale in sales)
                {
                    var card = new SaleCard(sale);
                    card.EditClicked   += (s, e) => OpenEditor(((SaleCard)s!).Sale);
                    card.DeleteClicked += (s, e) => DeleteSale(((SaleCard)s!).Sale.Id);
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
                Size      = new Size(400, 620),
                BackColor = DS.CardBg,
            };
            panel.Paint += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(DS.Warning, 1.5f);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                using var glow = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, panel.Width, 4), DS.Warning, Color.Transparent, 0f);
                e.Graphics.FillRectangle(glow, 0, 0, panel.Width, 4);
            };

            lblEditorTitle = new Label
            {
                Text      = "Add Promotion",
                ForeColor = DS.Warning,
                Font      = DS.SectionFont,
                AutoSize  = true,
                Location  = new Point(24, 20),
            };

            inpId             = new GlowInputPanel("Sale ID")           { Location = new Point(24, 70),  Width = 352 };
            inpProductId      = new GlowInputPanel("Product ID")        { Location = new Point(24, 148), Width = 352 };
            inpAmountRequired = new GlowInputPanel("Min Qty Required")  { Location = new Point(24, 226), Width = 352 };
            inpTotalPrice     = new GlowInputPanel("Sale Price (₪)")    { Location = new Point(24, 304), Width = 352 };

            chkClubMembers = new CheckBox
            {
                Text      = "Club members only",
                ForeColor = DS.TextMuted,
                BackColor = DS.CardBg,
                Location  = new Point(24, 384),
                AutoSize  = true,
                Font      = DS.BodyFont,
            };

            lblStart = new Label { Text = "Start Date", ForeColor = DS.TextMuted, Font = DS.CaptionFont,
                                   AutoSize = true, Location = new Point(24, 412) };
            dtpStart = MakeDatePicker(new Point(24, 432));

            lblEnd = new Label   { Text = "End Date",   ForeColor = DS.TextMuted, Font = DS.CaptionFont,
                                   AutoSize = true, Location = new Point(24, 472) };
            dtpEnd = MakeDatePicker(new Point(24, 492));

            btnSave = new GlowButton
            {
                Text     = "SAVE",
                Size     = new Size(352, 48),
                Location = new Point(24, 542),
                BackColor= DS.Warning,
            };
            btnSave.Click += OnSaveClicked;

            btnDelete = new GlowButton
            {
                Text      = "DELETE",
                Size      = new Size(168, 36),
                BackColor = DS.Danger,
                Location  = new Point(24, 578),
                Visible   = false,
            };
            btnDelete.Click += OnDeleteClicked;

            btnCancel = new GlowButton
            {
                Text      = "CANCEL",
                Size      = new Size(168, 36),
                BackColor = DS.Hairline,
                Location  = new Point(208, 578),
            };
            btnCancel.Click += (s, e) => CloseEditor();

            panel.Controls.AddRange(new Control[]
            { lblEditorTitle, inpId, inpProductId, inpAmountRequired, inpTotalPrice,
              chkClubMembers, lblStart, dtpStart, lblEnd, dtpEnd,
              btnSave, btnDelete, btnCancel });
            return panel;
        }

        private static DateTimePicker MakeDatePicker(Point location) => new DateTimePicker
        {
            Location  = location,
            Size      = new Size(352, 30),
            Format    = DateTimePickerFormat.Short,
            BackColor = DS.CardBg,
            ForeColor = DS.TextPrimary,
        };

        private void OpenEditor(Sale? sale)
        {
            _editingSale = sale;

            if (sale == null)
            {
                lblEditorTitle.Text                = "Add Promotion";
                inpId.Input.Text                   = "";
                inpId.Input.ReadOnly               = false;
                inpProductId.Input.Text            = "";
                inpAmountRequired.Input.Text       = "";
                inpTotalPrice.Input.Text           = "";
                chkClubMembers.Checked             = false;
                dtpStart.Value                     = DateTime.Today;
                dtpEnd.Value                       = DateTime.Today.AddMonths(1);
                btnDelete.Visible                  = false;
            }
            else
            {
                lblEditorTitle.Text                = "Edit Promotion";
                inpId.Input.Text                   = sale.Id.ToString();
                inpId.Input.ReadOnly               = true;
                inpProductId.Input.Text            = sale.ProductId.ToString();
                inpAmountRequired.Input.Text       = sale.AmmontRequird.ToString();
                inpTotalPrice.Input.Text           = sale.TotalPrice.ToString();
                chkClubMembers.Checked             = sale.IsClubMembers;
                dtpStart.Value                     = sale.StartSale;
                dtpEnd.Value                       = sale.EndSale;
                btnDelete.Visible                  = true;
            }

            pnlEditor.Location = new Point(Width - 430, 90);
            pnlEditor.BringToFront();
            pnlEditor.Visible = true;
        }

        private void CloseEditor()
        {
            pnlEditor.Visible = false;
            _editingSale      = null;
        }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            try
            {
                var sale = new Sale
                {
                    Id            = int.TryParse(inpId.Input.Text, out var id) ? id : 0,
                    ProductId     = int.TryParse(inpProductId.Input.Text, out var pid) ? pid : 0,
                    AmmontRequird = int.TryParse(inpAmountRequired.Input.Text, out var amt) ? amt : 0,
                    TotalPrice    = double.TryParse(inpTotalPrice.Input.Text, out var price) ? price : 0,
                    IsClubMembers = chkClubMembers.Checked,
                    StartSale     = dtpStart.Value,
                    EndSale       = dtpEnd.Value,
                };

                if (_editingSale == null)
                    // BL: bl.Sale.Create(sale)
                    _bl.Sale.Create(sale);
                else
                    // BL: bl.Sale.Update(sale)
                    _bl.Sale.Update(sale);

                CloseEditor();
                LoadCards(searchBox.Input.Text);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (_editingSale == null) return;
            DeleteSale(_editingSale.Id);
        }

        private void DeleteSale(int id)
        {
            var confirm = MessageBox.Show($"Delete promotion #{id}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // BL: bl.Sale.Delete(id)
                _bl.Sale.Delete(id);
                CloseEditor();
                LoadCards(searchBox.Input.Text);
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
