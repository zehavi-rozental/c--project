// =============================================================================
// Program.cs  –  Application entry point.
// Creates the BL instance once and passes it to LoginForm.
// =============================================================================

using BlApi;

namespace UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // BL: single factory call — one IBl for the entire app lifetime
            IBl bl = BlApi.Factory.Get();

            Application.Run(new LoginForm(bl));
        }
    }
}
