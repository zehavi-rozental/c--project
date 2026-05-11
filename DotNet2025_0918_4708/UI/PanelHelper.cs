// =============================================================================
// PanelHelper.cs  –  Shared UI building helpers (no BL calls).
// =============================================================================

using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    internal static class PanelHelper
    {
        /// <summary>Creates a standard page header with title + subtitle.</summary>
        public static Panel MakeHeader(string title, string subtitle)
        {
            var panel = new Panel
            {
                BackColor = DS.Surface,
                Padding   = new Padding(24, 0, 0, 0),
            };
            panel.Paint += (s, e) =>
            {
                using var line = new System.Drawing.Pen(DS.Hairline, 1);
                e.Graphics.DrawLine(line, 0, panel.Height - 1, panel.Width, panel.Height - 1);
                // Left accent stripe
                using var accent = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(0, panel.Height),
                    DS.Accent, Color.Transparent);
                e.Graphics.FillRectangle(accent, 0, 0, 3, panel.Height);
            };

            var lblTitle = new Label
            {
                Text      = title,
                ForeColor = DS.TextPrimary,
                Font      = DS.HeadingFont,
                AutoSize  = true,
                Location  = new Point(28, 16),
            };

            var lblSub = new Label
            {
                Text         = subtitle,
                ForeColor    = DS.TextMuted,
                Font         = DS.CaptionFont,
                AutoSize     = false,
                Size         = new Size(900, 20),
                Location     = new Point(30, 54),
                MaximumSize  = new Size(1000, 20),
            };

            panel.Controls.AddRange(new Control[] { lblTitle, lblSub });
            return panel;
        }
    }
}
