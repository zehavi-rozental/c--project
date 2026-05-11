// =============================================================================
// LoginForm.Designer.cs  –  Visual Layout ONLY
// No BL calls, no event logic here.
// =============================================================================

using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private Panel pnlLeft;
        private Panel pnlRight;
        private Panel pnlLogoWrap;
        private Label lblLogo;
        private Label lblTagline;
        private Label lblVersion;
        private Panel pnlCard;
        private Label lblWelcome;
        private Label lblSubtitle;
        private GlowTextBox txtUsername;
        private GlowTextBox txtPassword;
        private GlowButton btnLogin;
        private Label lblError;
        private Panel pnlDivider;
        private AnimatedLogo logoAnim;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // ── Form ──────────────────────────────────────────────────────
            this.Text           = "SmartHome Hub · Login";
            this.ClientSize     = new Size(1100, 680);
            this.BackColor      = DS.Background;
            this.ForeColor      = DS.TextPrimary;
            this.Font           = DS.BodyFont;
            this.FormBorderStyle= FormBorderStyle.FixedSingle;
            this.MaximizeBox    = false;
            this.StartPosition  = FormStartPosition.CenterScreen;

            // ── Left panel – branding ─────────────────────────────────────
            pnlLeft = new Panel
            {
                Bounds = new Rectangle(0, 0, 500, 680),
                BackColor = DS.Surface,
            };
            pnlLeft.Paint += (s, e) =>
            {
                // Vertical accent stripe
                using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(490, 0), new Point(500, 680),
                    DS.Accent, DS.AccentDim);
                e.Graphics.FillRectangle(brush, 490, 0, 10, 680);

                // Subtle grid pattern
                using var gridPen = new Pen(Color.FromArgb(18, DS.Accent), 1);
                for (int x = 0; x < 490; x += 40)
                    e.Graphics.DrawLine(gridPen, x, 0, x, 680);
                for (int y = 0; y < 680; y += 40)
                    e.Graphics.DrawLine(gridPen, 0, y, 490, y);
            };

            logoAnim = new AnimatedLogo
            {
                Bounds = new Rectangle(60, 80, 200, 200),
            };

            lblLogo = new Label
            {
                Text      = "SmartHome Hub",
                ForeColor = DS.TextPrimary,
                Font      = DS.TitleFont,
                AutoSize  = true,
                Location  = new Point(60, 300),
            };

            lblTagline = new Label
            {
                Text      = "Intelligent spaces.\nPowered by precision.",
                ForeColor = DS.TextMuted,
                Font      = DS.SubtitleFont,
                AutoSize  = true,
                Location  = new Point(62, 360),
            };

            lblVersion = new Label
            {
                Text      = "v4.0  ·  Center District, Israel",
                ForeColor = DS.AccentDim,
                Font      = DS.CaptionFont,
                AutoSize  = true,
                Location  = new Point(62, 620),
            };

            pnlLeft.Controls.AddRange(new Control[] { logoAnim, lblLogo, lblTagline, lblVersion });

            // ── Right panel – login card ───────────────────────────────────
            pnlRight = new Panel
            {
                Bounds    = new Rectangle(500, 0, 600, 680),
                BackColor = DS.Background,
            };

            pnlCard = new Panel
            {
                Bounds    = new Rectangle(80, 100, 440, 480),
                BackColor = DS.CardBg,
            };
            pnlCard.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var pen = new Pen(DS.AccentDim, 1);
                g.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
                // Top accent line
                using var topBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(pnlCard.Width, 0),
                    DS.Accent, Color.Transparent);
                g.FillRectangle(topBrush, 0, 0, pnlCard.Width, 2);
            };

            lblWelcome = new Label
            {
                Text      = "Welcome Back",
                ForeColor = DS.TextPrimary,
                Font      = DS.HeadingFont,
                AutoSize  = true,
                Location  = new Point(36, 36),
            };

            lblSubtitle = new Label
            {
                Text      = "Sign in to continue to your workspace",
                ForeColor = DS.TextMuted,
                Font      = DS.BodyFont,
                AutoSize  = true,
                Location  = new Point(38, 80),
            };

            pnlDivider = new Panel
            {
                Bounds    = new Rectangle(36, 108, 368, 1),
                BackColor = DS.Hairline,
            };

            txtUsername = new GlowTextBox
            {
                PlaceholderText = "Username",
                Bounds          = new Rectangle(36, 130, 368, 48),
            };
            // Pre-fill placeholder manually for the old-style approach
            txtUsername.Text      = "Username";
            txtUsername.ForeColor = DS.TextHint;
            txtUsername.Enter    += txtUsername_Enter;
            txtUsername.Leave    += txtUsername_Leave;

            txtPassword = new GlowTextBox
            {
                PlaceholderText       = "Password",
                Bounds                = new Rectangle(36, 202, 368, 48),
                UseSystemPasswordChar = false,
            };
            txtPassword.Text      = "Password";
            txtPassword.ForeColor = DS.TextHint;
            txtPassword.Enter    += txtPassword_Enter;
            txtPassword.Leave    += txtPassword_Leave;

            lblError = new Label
            {
                Text      = "",
                ForeColor = DS.Danger,
                Font      = DS.CaptionFont,
                AutoSize  = true,
                Location  = new Point(36, 262),
                Visible   = false,
            };

            btnLogin = new GlowButton
            {
                Text   = "SIGN IN",
                Bounds = new Rectangle(36, 290, 368, 56),
            };
            btnLogin.Click      += btnLogin_Click;
            btnLogin.MouseEnter += btnLogin_MouseEnter;
            btnLogin.MouseLeave += btnLogin_MouseLeave;

            var lblHint = new Label
            {
                Text      = "Admin: admin / admin  ·  Cashier: cashier / cashier",
                ForeColor = DS.AccentDim,
                Font      = DS.CaptionFont,
                AutoSize  = true,
                Location  = new Point(36, 365),
            };

            pnlCard.Controls.AddRange(new Control[]
            {
                lblWelcome, lblSubtitle, pnlDivider,
                txtUsername, txtPassword, lblError, btnLogin, lblHint
            });

            pnlRight.Controls.Add(pnlCard);

            this.Controls.AddRange(new Control[] { pnlLeft, pnlRight });
            this.ResumeLayout(false);
        }
    }
}
