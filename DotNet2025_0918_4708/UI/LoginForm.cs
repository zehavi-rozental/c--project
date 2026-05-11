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

            // Use BL for authentication (supports Email or Name)
            try
            {
                try { Tools.LogManager.Log("UI", "btnLogin_Click", $"Login attempt for '{username}'"); } catch { }
                var session = _bl.Login(username, password);
                try { Tools.LogManager.Log("UI", "btnLogin_Click", $"Login success for '{username}' (Id={session.Id}, IsAdmin={session.IsAdmin})"); } catch { }
                if (session == null)
                {
                    ShowValidationError("Invalid credentials. Try admin/admin or cashier/cashier.");
                    return;
                }

                OpenRole(session.IsAdmin ? UserRole.Admin : UserRole.Cashier);
            }
            catch (Exception ex)
            {
                try { Tools.LogManager.Log("UI", "btnLogin_Click", $"Login failed for '{username}': {ex}"); } catch { }
                ShowValidationError(ex.Message ?? "Invalid credentials.");
            }
        }

        private void OpenRole(UserRole role)
        {
            // Create the ShellForm first. If construction fails, keep the login form visible.
            ShellForm? shell = null;
            try
            {
                shell = new ShellForm(_bl, role);
                // When the shell closes (logout), show the login form again instead of closing the app.
                shell.FormClosed += (s, e) => this.Show();
                shell.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                // If shell failed to open, ensure it's disposed, log full details and show the error.
                shell?.Dispose();
                try { Tools.LogManager.Log("UI", "OpenRole", ex.ToString()); } catch { }
                ShowValidationError("Failed to open application shell: " + (ex.Message ?? "Unknown error"));
            }
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
