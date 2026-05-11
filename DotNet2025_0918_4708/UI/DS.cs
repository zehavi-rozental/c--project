// =============================================================================
// DS.cs  –  Design System: all colors, fonts, and shared visual tokens.
// Referenced by ALL forms and custom controls.  Never put BL calls here.
// =============================================================================

using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    /// <summary>
    /// Central design token registry.
    /// All forms/controls pull from here so a palette change is a one-line edit.
    /// </summary>
    internal static class DS
    {
        // ── Palette ───────────────────────────────────────────────────────

        public static readonly Color Background   = Color.FromArgb(7,  11, 22);   // near-black navy
        public static readonly Color Surface       = Color.FromArgb(10, 16, 32);   // sidebar / left panels
        public static readonly Color CardBg        = Color.FromArgb(14, 22, 44);   // card surface
        public static readonly Color CardBgHover   = Color.FromArgb(18, 30, 58);   // card hover
        public static readonly Color ModalBg       = Color.FromArgb(12, 20, 40);

        public static readonly Color Accent        = Color.FromArgb(0,  210, 180); // teal glow
        public static readonly Color AccentHover   = Color.FromArgb(0,  240, 208);
        public static readonly Color AccentDim     = Color.FromArgb(0,  140, 120);
        public static readonly Color AccentSoft    = Color.FromArgb(0,  210, 180, 40); // semi-transparent

        public static readonly Color Hairline      = Color.FromArgb(30, 50, 80);
        public static readonly Color Danger        = Color.FromArgb(255, 90, 90);
        public static readonly Color Success       = Color.FromArgb(60, 230, 140);
        public static readonly Color Warning       = Color.FromArgb(255, 190, 60);

        public static readonly Color TextPrimary   = Color.FromArgb(230, 240, 255);
        public static readonly Color TextMuted     = Color.FromArgb(130, 155, 200);
        public static readonly Color TextHint      = Color.FromArgb(70,  95, 140);

        // ── Typography ────────────────────────────────────────────────────

        public static readonly Font TitleFont    = new Font("Segoe UI Variable Display", 26F, FontStyle.Bold,   GraphicsUnit.Point);
        public static readonly Font HeadingFont  = new Font("Segoe UI",                  20F, FontStyle.Bold,   GraphicsUnit.Point);
        public static readonly Font SectionFont  = new Font("Segoe UI",                  14F, FontStyle.Bold,   GraphicsUnit.Point);
        public static readonly Font SubtitleFont = new Font("Segoe UI",                  11F, FontStyle.Regular,GraphicsUnit.Point);
        public static readonly Font BodyFont     = new Font("Segoe UI",                  10F, FontStyle.Regular,GraphicsUnit.Point);
        public static readonly Font BodyBold     = new Font("Segoe UI",                  10F, FontStyle.Bold,   GraphicsUnit.Point);
        public static readonly Font CaptionFont  = new Font("Segoe UI",                   8F, FontStyle.Regular,GraphicsUnit.Point);
        public static readonly Font MonoFont     = new Font("Cascadia Mono",              9F, FontStyle.Regular,GraphicsUnit.Point);
        public static readonly Font BigPriceFont = new Font("Segoe UI Variable Display", 32F, FontStyle.Bold,   GraphicsUnit.Point);

        // ── Shared Helpers ────────────────────────────────────────────────

        public static Color CategoryColor(BO.Category c) => c switch
        {
            BO.Category.LIGHTING => Color.FromArgb(255, 200, 60),
            BO.Category.SECURITY => Color.FromArgb(100, 180, 255),
            BO.Category.CLIMATE  => Color.FromArgb(100, 230, 150),
            BO.Category.AUDIO    => Color.FromArgb(220, 120, 255),
            _                    => Accent
        };

        public static string CategoryIcon(BO.Category c) => c switch
        {
            BO.Category.LIGHTING => "💡",
            BO.Category.SECURITY => "🔒",
            BO.Category.CLIMATE  => "🌡",
            BO.Category.AUDIO    => "🔊",
            _                    => "📦"
        };

        public static string CategoryLabel(BO.Category c) => c switch
        {
            BO.Category.LIGHTING => "Lighting",
            BO.Category.SECURITY => "Security",
            BO.Category.CLIMATE  => "Climate",
            BO.Category.AUDIO    => "Audio",
            _                    => c.ToString()
        };
    }
}
