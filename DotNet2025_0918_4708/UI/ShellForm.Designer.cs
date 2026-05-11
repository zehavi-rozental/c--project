// =============================================================================
// ShellForm.Designer.cs  –  Layout ONLY for the main shell window.
// Sidebar, nav buttons, content host panel.
// =============================================================================

using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class ShellForm
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Panel pnlSidebarTop;
        private Label lblAppName;
        private Label lblAppSub;
        private Panel pnlDividerTop;
        private SideNavButton btnNavCustomers;
        private SideNavButton btnNavProducts;
        private SideNavButton btnNavSales;
        private SideNavButton btnNavPOS;
        private Panel pnlSidebarBottom;
        private SideNavButton btnLogout;
        private Label lblRoleIndicator;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text           = "SmartHome Hub";
            this.ClientSize     = new Size(1400, 860);
            this.BackColor      = DS.Background;
            this.ForeColor      = DS.TextPrimary;
            this.Font           = DS.BodyFont;
            this.MinimumSize    = new Size(1200, 700);
            this.StartPosition  = FormStartPosition.CenterScreen;

            // ── Sidebar ───────────────────────────────────────────────────
            pnlSidebar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 230,
                BackColor = DS.Surface,
            };
            pnlSidebar.Paint += (s, e) =>
            {
                // Right hairline border
                using var pen = new System.Drawing.Pen(DS.Hairline, 1);
                e.Graphics.DrawLine(pen, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            // App header area
            pnlSidebarTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 110,
                BackColor = System.Drawing.Color.Transparent,
            };

            lblAppName = new Label
            {
                Text      = "SmartHome",
                ForeColor = DS.TextPrimary,
                Font      = DS.SectionFont,
                AutoSize  = true,
                Location  = new System.Drawing.Point(16, 20),
            };

            lblAppSub = new Label
            {
                Text      = "Hub  ●",
                ForeColor = DS.Accent,
                Font      = DS.SectionFont,
                AutoSize  = true,
                Location  = new System.Drawing.Point(108, 20),
            };

            lblRoleIndicator = new Label
            {
                Text      = _role == UserRole.Admin ? "ADMIN" : "CASHIER",
                ForeColor = _role == UserRole.Admin ? DS.Warning : DS.Accent,
                Font      = DS.CaptionFont,
                AutoSize  = true,
                Location  = new System.Drawing.Point(18, 56),
            };

            pnlDividerTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = DS.Hairline,
            };

            pnlSidebarTop.Controls.AddRange(new Control[] { lblAppName, lblAppSub, lblRoleIndicator });
            pnlSidebar.Controls.Add(pnlSidebarTop);
            pnlSidebar.Controls.Add(pnlDividerTop);

            // Nav buttons
            btnNavCustomers = new SideNavButton("👥", "Customers") { Top = 130 };
            btnNavProducts  = new SideNavButton("📦", "Products")  { Top = 186 };
            btnNavSales     = new SideNavButton("🎁", "Promotions"){ Top = 242 };
            btnNavPOS       = new SideNavButton("💳", "POS Order") { Top = 130 };

            pnlSidebar.Controls.AddRange(new Control[]
            { btnNavCustomers, btnNavProducts, btnNavSales, btnNavPOS });

            // Bottom: logout
            pnlSidebarBottom = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 60,
                BackColor = System.Drawing.Color.Transparent,
            };
            pnlSidebarBottom.Paint += (s, e) =>
            {
                using var pen = new System.Drawing.Pen(DS.Hairline, 1);
                e.Graphics.DrawLine(pen, 0, 0, pnlSidebarBottom.Width, 0);
            };

            btnLogout = new SideNavButton("↩", "Sign Out") { Top = 10 };
            pnlSidebarBottom.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(pnlSidebarBottom);

            // ── Content area ──────────────────────────────────────────────
            pnlContent = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = DS.Background,
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);

            this.ResumeLayout(false);
        }
    }
}
