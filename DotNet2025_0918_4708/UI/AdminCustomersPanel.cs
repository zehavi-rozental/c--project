// =============================================================================
// AdminCustomersPanel.cs  –  Customer management panel (card layout).
// Handles both the card grid view and the side-editor.
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
    public class AdminCustomersPanel : Panel
    {
        private readonly IBl _bl;

        // ── Layout areas ─────────────────────────────────────────────────
        private Panel pnlHeader;
        private Panel pnlToolbar;
        private FlowLayoutPanel flpCards;
        private Panel pnlEditor;
        private Panel pnlEditorScrim;

        // ── Toolbar ───────────────────────────────────────────────────────
        private SearchBox searchBox;
        private GlowButton btnAddNew;

        // ── Editor fields ─────────────────────────────────────────────────
        private Label lblEditorTitle;
        private GlowInputPanel inpId;
        private GlowInputPanel inpName;
        private GlowInputPanel inpEmail;
        private GlowInputPanel inpPassword;
        private GlowInputPanel inpAddress;
        private GlowInputPanel inpPhone;
        private CheckBox chkIsAdmin;
        private GlowButton btnSave;
        private GlowButton btnDelete;
        private GlowButton btnCancel;

        private Client? _editingClient;

        public AdminCustomersPanel(IBl bl)
        {
            _bl       = bl;
            BackColor = DS.Background;
            BuildLayout();
            // BL: initial data load
            LoadCards();
        }

        // ── Build Layout ──────────────────────────────────────────────────

        private void BuildLayout()
        {
            // Header
            pnlHeader = PanelHelper.MakeHeader("Customer Management",
                "View, add, and edit customer profiles");
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 90;

            // Toolbar
            pnlToolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.Transparent,
                Padding   = new Padding(20, 10, 20, 0),
            };

            searchBox = new SearchBox { Width = 280 };
            searchBox.Input.TextChanged += (s, e) => LoadCards(searchBox.Input.Text);
            searchBox.Location = new Point(20, 14);

            btnAddNew = new GlowButton
            {
                Text     = "+ Add Customer",
                Size     = new Size(160, 38),
                Location = new Point(320, 11),
            };
            btnAddNew.Click += (s, e) => OpenEditor(null);

            pnlToolbar.Controls.AddRange(new Control[] { searchBox, btnAddNew });

            // Cards area
            flpCards = new FlowLayoutPanel
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.Transparent,
                AutoScroll  = true,
                WrapContents= true,
                FlowDirection= FlowDirection.LeftToRight,
                Padding     = new Padding(16),
            };

            // Editor panel (overlays right side)
            pnlEditor = BuildEditorPanel();
            pnlEditor.Visible = false;

            Controls.Add(flpCards);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlHeader);
            Controls.Add(pnlEditor);
        }

        // ── Data Loading ──────────────────────────────────────────────────

        private void LoadCards(string filter = "")
        {
            flpCards.SuspendLayout();
            flpCards.Controls.Clear();

            try
            {
                // BL: bl.Client.ReadAll()
                IEnumerable<Client> clients = _bl.Client.ReadAll();

                if (!string.IsNullOrWhiteSpace(filter))
                    clients = clients.Where(c =>
                        c.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                        c.Id.ToString().Contains(filter));

                foreach (var client in clients)
                {
                    var card = new CustomerCard(client);
                    card.EditClicked   += (s, e) => OpenEditor(((CustomerCard)s!).Client);
                    card.DeleteClicked += (s, e) => DeleteClient(((CustomerCard)s!).Client.Id);
                    flpCards.Controls.Add(card);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            flpCards.ResumeLayout();
        }

        // ── Editor ────────────────────────────────────────────────────────

        private Panel BuildEditorPanel()
        {
            var panel = new Panel
            {
                Size      = new Size(400, 680),
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
                Text      = "Add Customer",
                ForeColor = DS.Accent,
                Font      = DS.SectionFont,
                AutoSize  = true,
                Location  = new Point(24, 20),
            };

            inpId      = new GlowInputPanel("Customer ID")    { Location = new Point(24, 70),  Width = 352 };
            inpName    = new GlowInputPanel("Full Name")      { Location = new Point(24, 148), Width = 352 };
            inpEmail    = new GlowInputPanel("Email")          { Location = new Point(24, 226), Width = 352 };
            inpPassword = new GlowInputPanel("Password", true) { Location = new Point(24, 304), Width = 352 };
            inpAddress  = new GlowInputPanel("Address")        { Location = new Point(24, 382), Width = 352 };
            inpPhone    = new GlowInputPanel("Phone Number")   { Location = new Point(24, 460), Width = 352 };
            chkIsAdmin = new CheckBox
            {
                Text      = "Is Admin",
                ForeColor = DS.TextMuted,
                BackColor = Color.Transparent,
                AutoSize  = true,
                Location  = new Point(24, 528),
                Font      = DS.CaptionFont,
            };

            btnSave = new GlowButton
            {
                Text     = "SAVE",
                Size     = new Size(352, 48),
                Location = new Point(24, 390),
            };
            btnSave.Click += OnSaveClicked;

            btnDelete = new GlowButton
            {
                Text      = "DELETE",
                Size      = new Size(168, 40),
                BackColor = DS.Danger,
                Location  = new Point(24, 450),
                Visible   = false,
            };
            btnDelete.Click += OnDeleteClicked;

            btnCancel = new GlowButton
            {
                Text      = "CANCEL",
                Size      = new Size(168, 40),
                BackColor = DS.Hairline,
                Location  = new Point(208, 450),
            };
            btnCancel.Click += (s, e) => CloseEditor();

            panel.Controls.AddRange(new Control[]
            { lblEditorTitle, inpId, inpName, inpAddress, inpPhone, btnSave, btnDelete, btnCancel });
            return panel;
        }

        private void OpenEditor(Client? client)
        {
            _editingClient = client;

            if (client == null)
            {
                lblEditorTitle.Text   = "Add Customer";
                inpId.Input.Text      = "";
                inpId.Input.ReadOnly  = false;
                inpName.Input.Text    = "";
                inpAddress.Input.Text = "";
                inpPhone.Input.Text   = "";
                chkIsAdmin.Checked     = false;
            }
            else
            {
                lblEditorTitle.Text   = "Edit Customer";
                inpId.Input.Text      = client.Id.ToString();
                inpId.Input.ReadOnly  = true;
                inpName.Input.Text    = client.Name;
                inpAddress.Input.Text = client.Address;
                inpPhone.Input.Text   = client.PhoneNumber;
                chkIsAdmin.Checked     = client.IsAdmin;
            }

            // Position editor on the right side
            pnlEditor.Location = new Point(Width - 430, 90);
            pnlEditor.BringToFront();
            pnlEditor.Visible = true;
        }

        private void CloseEditor()
        {
            pnlEditor.Visible = false;
            _editingClient    = null;
        }

        // ── BL Actions ────────────────────────────────────────────────────

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            try
            {
                var client = new Client
                {
                    Id          = int.TryParse(inpId.Input.Text, out var id) ? id : 0,
                    Name        = inpName.Input.Text,
                    Address     = inpAddress.Input.Text,
                    PhoneNumber = inpPhone.Input.Text,
                    Email       = inpEmail.Input.Text,
                    Password    = inpPassword.Input.Text,
                };

                if (_editingClient == null)
                {
                    // New client: assign a default password so the account is usable.
                    if (string.IsNullOrEmpty(client.Password)) client.Password = "1234";
                    _bl.Client.Create(client);
                }
                else
                {
                    // Preserve existing password when left blank in the editor
                    var existing = _bl.Client.Read(_editingClient.Id);
                    if (existing != null)
                    {
                        if (string.IsNullOrEmpty(client.Password)) client.Password = existing.Password;
                    }
                    _bl.Client.Update(client);
                }

                CloseEditor();
                LoadCards(searchBox.Input.Text);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (_editingClient == null) return;
            DeleteClient(_editingClient.Id);
        }

        private void DeleteClient(int id)
        {
            var confirm = MessageBox.Show($"Delete customer #{id}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // BL: bl.Client.Delete(id)
                _bl.Client.Delete(id);
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
