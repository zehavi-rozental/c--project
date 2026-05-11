// =============================================================================
// LoginForm.cs  –  UI Logic / Event Handling
// UI Layout is defined in LoginForm.Designer.cs
// BL is injected from Program.cs via the constructor.
// =============================================================================

using BlApi;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    public partial class LoginForm : Form
    {
        // BL entry-point – injected so the form has no knowledge of the
        // concrete implementation (Factory is called once in Program.cs).
        private readonly IBl _bl;

        public LoginForm(IBl bl)
        {
            _bl = bl;
            InitializeComponent();
        }

        // ── Event handlers ────────────────────────────────────────────────

        // BL HOOK: validate credentials against bl.Client or a config store.
        private void btnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowValidationError("Please fill in all fields.");
                return;
            }

            // ── Role routing ──────────────────────────────────────────────
            // TODO (BL): replace hardcoded check with bl.Auth.Validate(username, password)
            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "admin")
            {
                OpenRole(UserRole.Admin);
            }
            else if (username.Equals("cashier", StringComparison.OrdinalIgnoreCase) && password == "cashier")
            {
                OpenRole(UserRole.Cashier);
            }
            else
            {
                ShowValidationError("Invalid credentials. Try admin/admin or cashier/cashier.");
            }
        }

        private void OpenRole(UserRole role)
        {
            this.Hide();
            var shell = new ShellForm(_bl, role);
            shell.FormClosed += (s, e) => this.Close();
            shell.Show();
        }

        private void ShowValidationError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        // Placeholder focus behavior
        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Username") { txtUsername.Text = ""; txtUsername.ForeColor = DS.TextPrimary; }
        }
        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text)) { txtUsername.Text = "Username"; txtUsername.ForeColor = DS.TextHint; }
        }
        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Password") { txtPassword.Text = ""; txtPassword.UseSystemPasswordChar = true; txtPassword.ForeColor = DS.TextPrimary; }
        }
        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text)) { txtPassword.UseSystemPasswordChar = false; txtPassword.Text = "Password"; txtPassword.ForeColor = DS.TextHint; }
        }

        private void btnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = DS.AccentHover;
        }
        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = DS.Accent;
        }
    }

    public enum UserRole { Admin, Cashier }
}
