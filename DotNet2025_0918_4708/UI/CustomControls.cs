// =============================================================================
// CustomControls.cs  –  All custom-painted controls for the SmartHome Hub UI.
// No BL calls.  Pure visual / interaction behavior.
// =============================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace UI
{
    // =========================================================================
    // GlowTextBox – thin-lined dark input with teal focus glow
    // =========================================================================
    public class GlowTextBox : TextBox
    {
        private bool _focused;
        public new string PlaceholderText { get; set; } = "";

        public GlowTextBox()
        {
            BackColor   = DS.CardBg;
            ForeColor   = DS.TextPrimary;
            BorderStyle = BorderStyle.None;
            Font        = DS.BodyFont;
            Height      = 48;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _focused = true;
            Parent?.Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _focused = false;
            Parent?.Invalidate();
        }

        // The border is drawn by a wrapper Panel painted in the parent.
        // We expose focus state so the parent (Designer) can paint the glow.
        public bool IsFocused => _focused;
    }

    // =========================================================================
    // GlowInputPanel – wraps a GlowTextBox and paints the styled border
    // =========================================================================
    public class GlowInputPanel : Panel
    {
        public GlowTextBox Input { get; }
        private readonly string _label;

        public GlowInputPanel(string label, bool isPassword = false)
        {
            _label      = label;
            Size        = new Size(368, 64);
            BackColor   = Color.Transparent;

            Input = new GlowTextBox
            {
                Location              = new Point(12, 28),
                Size                  = new Size(Width - 24, 28),
                Anchor                = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                BackColor             = Color.Transparent,
                UseSystemPasswordChar = isPassword,
            };
            Controls.Add(Input);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Label
            using var labelBrush = new SolidBrush(Input.IsFocused ? DS.Accent : DS.TextMuted);
            g.DrawString(_label, DS.CaptionFont, labelBrush, new PointF(2, 2));

            // Bottom border line
            var y = Height - 2;
            if (Input.IsFocused)
            {
                using var glowBrush = new LinearGradientBrush(
                    new Point(0, y), new Point(Width, y),
                    Color.Transparent, Color.Transparent)
                {
                    InterpolationColors = new ColorBlend
                    {
                        Colors   = new[] { Color.Transparent, DS.Accent, DS.AccentHover, DS.Accent, Color.Transparent },
                        Positions= new[] { 0f, 0.2f, 0.5f, 0.8f, 1f }
                    }
                };
                g.FillRectangle(glowBrush, 0, y - 1, Width, 3);
            }
            else
            {
                using var linePen = new Pen(DS.Hairline, 1);
                g.DrawLine(linePen, 0, y, Width, y);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Input.Width = Width - 24;
        }
    }

    // =========================================================================
    // GlowButton – large flat button with animated glow on hover/press
    // =========================================================================
    public class GlowButton : Control
    {
        private bool _hover;
        private bool _pressed;

        public GlowButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            Cursor    = Cursors.Hand;
            BackColor = DS.Accent;
            ForeColor = Color.White;
            Font      = DS.BodyBold;
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true;  Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); _pressed = true;  Invalidate(); }
        protected override void OnMouseUp(MouseEventArgs e)   { base.OnMouseUp(e);   _pressed = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var baseColor = _pressed ? DS.AccentDim : _hover ? DS.AccentHover : DS.Accent;

            // Background
            using var bgBrush = new SolidBrush(baseColor);
            g.FillRectangle(bgBrush, ClientRectangle);

            // Glow overlay on hover
            if (_hover && !_pressed)
            {
                using var glowBrush = new LinearGradientBrush(
                    ClientRectangle, Color.FromArgb(40, Color.White), Color.Transparent, 90f);
                g.FillRectangle(glowBrush, ClientRectangle);
            }

            // Text
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var textBrush = new SolidBrush(ForeColor);
            g.DrawString(Text, Font, textBrush, ClientRectangle, sf);
        }
    }

    // =========================================================================
    // SmallAccentButton – for card actions (Edit, Delete, Add)
    // =========================================================================
    public class SmallAccentButton : GlowButton
    {
        public SmallAccentButton(string text, Color? color = null)
        {
            Text      = text;
            Size      = new Size(90, 32);
            Font      = DS.CaptionFont;
            BackColor = color ?? DS.Accent;
        }
    }

    // =========================================================================
    // AnimatedLogo – rotating geometric wireframe (architecture motif)
    // =========================================================================
    public class AnimatedLogo : Control
    {
        private float _angle;
        private readonly System.Windows.Forms.Timer _timer;

        public AnimatedLogo()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size      = new Size(200, 200);

            _timer = new System.Windows.Forms.Timer { Interval = 30 };
            _timer.Tick += (s, e) => { _angle += 0.6f; if (_angle > 360) _angle -= 360; Invalidate(); };
            _timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var cx = Width / 2f;
            var cy = Height / 2f;

            DrawHexRing(g, cx, cy, 70, _angle,       DS.Accent,    2.0f);
            DrawHexRing(g, cx, cy, 50, -_angle * 1.3f, DS.AccentDim, 1.2f);
            DrawHexRing(g, cx, cy, 30, _angle  * 0.7f, DS.TextMuted, 0.8f);

            // Center dot
            using var dotBrush = new SolidBrush(DS.Accent);
            g.FillEllipse(dotBrush, cx - 5, cy - 5, 10, 10);
        }

        private static void DrawHexRing(Graphics g, float cx, float cy, float r, float offset, Color color, float width)
        {
            using var pen = new Pen(color, width) { DashStyle = DashStyle.Dot };
            int sides = 6;
            var pts   = new PointF[sides];
            for (int i = 0; i < sides; i++)
            {
                var a = (offset + i * 360f / sides) * Math.PI / 180.0;
                pts[i] = new PointF(cx + r * (float)Math.Cos(a), cy + r * (float)Math.Sin(a));
            }
            g.DrawPolygon(pen, pts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _timer.Stop(); _timer.Dispose(); }
            base.Dispose(disposing);
        }
    }

    // =========================================================================
    // SideNavButton – left sidebar navigation item
    // =========================================================================
    public class SideNavButton : Control
    {
        private bool _active;
        private bool _hover;
        private readonly string _icon;

        public bool IsActive
        {
            get => _active;
            set { _active = value; Invalidate(); }
        }

        public SideNavButton(string icon, string label)
        {
            _icon   = icon;
            Text    = label;
            Size    = new Size(220, 52);
            Cursor  = Cursors.Hand;
            Font    = DS.BodyFont;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true;  Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_active)
            {
                using var bg = new SolidBrush(Color.FromArgb(25, DS.Accent));
                g.FillRectangle(bg, ClientRectangle);
                using var accent = new SolidBrush(DS.Accent);
                g.FillRectangle(accent, 0, 0, 3, Height);
            }
            else if (_hover)
            {
                using var hoverBg = new SolidBrush(Color.FromArgb(12, DS.Accent));
                g.FillRectangle(hoverBg, ClientRectangle);
            }

            var textColor = _active ? DS.Accent : (_hover ? DS.TextPrimary : DS.TextMuted);
            using var sf = new StringFormat { LineAlignment = StringAlignment.Center };
            using var iconBrush = new SolidBrush(textColor);
            g.DrawString(_icon,  new Font("Segoe UI", 14F), iconBrush, new RectangleF(14, 0, 30, Height), sf);
            g.DrawString(Text, DS.BodyFont,                iconBrush, new RectangleF(52, 0, Width - 60, Height), sf);
        }
    }

    // =========================================================================
    // ProductCard – card tile used in Admin catalog view
    // =========================================================================
    public class ProductCard : Panel
    {
        private bool _hover;
        public BO.Product Product { get; }

        public event EventHandler? EditClicked;
        public event EventHandler? DeleteClicked;

        public ProductCard(BO.Product product)
        {
            Product   = product;
            Size      = new Size(280, 200);
            BackColor = DS.CardBg;
            Margin    = new Padding(10);
            Cursor    = Cursors.Default;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            BuildInternals();
        }

        private void BuildInternals()
        {
            var icon = new Label
            {
                Text      = DS.CategoryIcon(Product.Category),
                Font      = new Font("Segoe UI", 28F),
                ForeColor = DS.CategoryColor(Product.Category),
                AutoSize  = true,
                Location  = new Point(16, 16),
            };

            var name = new Label
            {
                Text         = Product.ProductName,
                Font         = DS.BodyBold,
                ForeColor    = DS.TextPrimary,
                AutoSize     = false,
                Size         = new Size(180, 40),
                Location     = new Point(68, 16),
                MaximumSize  = new Size(180, 40),
            };

            var catLabel = new Label
            {
                Text      = DS.CategoryLabel(Product.Category),
                Font      = DS.CaptionFont,
                ForeColor = DS.CategoryColor(Product.Category),
                AutoSize  = true,
                Location  = new Point(68, 56),
            };

            var price = new Label
            {
                Text      = $"₪{Product.Price:N2}",
                Font      = DS.SectionFont,
                ForeColor = DS.Accent,
                AutoSize  = true,
                Location  = new Point(16, 90),
            };

            var stock = new Label
            {
                Text      = $"Stock: {Product.Ammount}",
                Font      = DS.CaptionFont,
                ForeColor = Product.Ammount > 5 ? DS.TextMuted : DS.Danger,
                AutoSize  = true,
                Location  = new Point(16, 130),
            };

            var btnEdit = new SmallAccentButton("Edit");
            btnEdit.Location = new Point(16, 158);
            btnEdit.Click += (s, e) => EditClicked?.Invoke(this, e);

            var btnDel = new SmallAccentButton("Delete", DS.Danger);
            btnDel.Location = new Point(118, 158);
            btnDel.Click += (s, e) => DeleteClicked?.Invoke(this, e);

            Controls.AddRange(new Control[] { icon, name, catLabel, price, stock, btnEdit, btnDel });
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true;  Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var borderColor = _hover ? DS.Accent : DS.Hairline;
            using var pen = new Pen(borderColor, _hover ? 1.5f : 1f);
            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            if (_hover)
            {
                using var glow = new LinearGradientBrush(
                    new Rectangle(0, 0, Width, 3), DS.Accent, Color.Transparent, 0f);
                g.FillRectangle(glow, 0, 0, Width, 3);
            }
        }
    }

    // =========================================================================
    // CustomerCard – card tile used in Admin customer view
    // =========================================================================
    public class CustomerCard : Panel
    {
        private bool _hover;
        public BO.Client Client { get; }

        public event EventHandler? EditClicked;
        public event EventHandler? DeleteClicked;

        public CustomerCard(BO.Client client)
        {
            Client    = client;
            Size      = new Size(280, 180);
            BackColor = DS.CardBg;
            Margin    = new Padding(10);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            BuildInternals();
        }

        private void BuildInternals()
        {
            var avatar = new Label
            {
                Text      = "👤",
                Font      = new Font("Segoe UI", 24F),
                ForeColor = DS.Accent,
                AutoSize  = true,
                Location  = new Point(16, 16),
            };

            var name = new Label
            {
                Text      = Client.Name,
                Font      = DS.BodyBold,
                ForeColor = DS.TextPrimary,
                AutoSize  = true,
                Location  = new Point(62, 16),
            };

            var idLabel = new Label
            {
                Text      = $"ID #{Client.Id}",
                Font      = DS.CaptionFont,
                ForeColor = DS.AccentDim,
                AutoSize  = true,
                Location  = new Point(62, 42),
            };

            var addr = new Label
            {
                Text         = Client.Address,
                Font         = DS.CaptionFont,
                ForeColor    = DS.TextMuted,
                AutoSize     = false,
                Size         = new Size(248, 30),
                Location     = new Point(16, 80),
                MaximumSize  = new Size(248, 30),
            };

            var phone = new Label
            {
                Text      = Client.PhoneNumber,
                Font      = DS.CaptionFont,
                ForeColor = DS.TextMuted,
                AutoSize  = true,
                Location  = new Point(16, 112),
            };

            var btnEdit = new SmallAccentButton("Edit");
            btnEdit.Location = new Point(16, 140);
            btnEdit.Click += (s, e) => EditClicked?.Invoke(this, e);

            var btnDel = new SmallAccentButton("Delete", DS.Danger);
            btnDel.Location = new Point(118, 140);
            btnDel.Click += (s, e) => DeleteClicked?.Invoke(this, e);

            Controls.AddRange(new Control[] { avatar, name, idLabel, addr, phone, btnEdit, btnDel });
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true;  Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var pen = new Pen(_hover ? DS.Accent : DS.Hairline, _hover ? 1.5f : 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }

    // =========================================================================
    // SaleCard – promotion tile for Admin promotions view
    // =========================================================================
    public class SaleCard : Panel
    {
        private bool _hover;
        public BO.Sale Sale { get; }

        public event EventHandler? EditClicked;
        public event EventHandler? DeleteClicked;

        public SaleCard(BO.Sale sale)
        {
            Sale      = sale;
            Size      = new Size(300, 180);
            BackColor = DS.CardBg;
            Margin    = new Padding(10);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            BuildInternals();
        }

        private void BuildInternals()
        {
            var tag = new Label
            {
                Text      = "🎁",
                Font      = new Font("Segoe UI", 24F),
                ForeColor = DS.Warning,
                AutoSize  = true,
                Location  = new Point(16, 16),
            };

            var saleId = new Label
            {
                Text      = $"Sale #{Sale.Id}",
                Font      = DS.BodyBold,
                ForeColor = DS.TextPrimary,
                AutoSize  = true,
                Location  = new Point(58, 16),
            };

            var prodLine = new Label
            {
                Text      = $"Product ID: {Sale.ProductId}",
                Font      = DS.CaptionFont,
                ForeColor = DS.TextMuted,
                AutoSize  = true,
                Location  = new Point(58, 42),
            };

            var priceLine = new Label
            {
                Text      = $"₪{Sale.TotalPrice:N2} for {Sale.AmmontRequird} units",
                Font      = DS.BodyFont,
                ForeColor = DS.Accent,
                AutoSize  = true,
                Location  = new Point(16, 76),
            };

            bool isActive = Sale.StartSale <= DateTime.Now && Sale.EndSale >= DateTime.Now;
            var statusLabel = new Label
            {
                Text      = isActive ? "● ACTIVE" : "○ INACTIVE",
                Font      = DS.CaptionFont,
                ForeColor = isActive ? DS.Success : DS.TextHint,
                AutoSize  = true,
                Location  = new Point(16, 104),
            };

            var dateRange = new Label
            {
                Text      = $"{Sale.StartSale:dd/MM/yy} – {Sale.EndSale:dd/MM/yy}",
                Font      = DS.CaptionFont,
                ForeColor = DS.TextMuted,
                AutoSize  = true,
                Location  = new Point(16, 120),
            };

            var btnEdit = new SmallAccentButton("Edit");
            btnEdit.Location = new Point(16, 142);
            btnEdit.Click += (s, e) => EditClicked?.Invoke(this, e);

            var btnDel = new SmallAccentButton("Delete", DS.Danger);
            btnDel.Location = new Point(118, 142);
            btnDel.Click += (s, e) => DeleteClicked?.Invoke(this, e);

            Controls.AddRange(new Control[]
            { tag, saleId, prodLine, priceLine, statusLabel, dateRange, btnEdit, btnDel });
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true;  Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var pen = new Pen(_hover ? DS.Warning : DS.Hairline, _hover ? 1.5f : 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }

    // =========================================================================
    // OrderItemCard – row card in the POS shopping cart
    // =========================================================================
    public class OrderItemCard : Panel
    {
        public BO.ProductInOrder Item { get; }
        public event EventHandler? RemoveClicked;

        public OrderItemCard(BO.ProductInOrder item)
        {
            Item      = item;
            Size      = new Size(700, 76);
            BackColor = DS.CardBg;
            Margin    = new Padding(0, 0, 0, 8);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            BuildInternals();
        }

        private void BuildInternals()
        {
            var nameLabel = new Label
            {
                Text      = Item.ProductName,
                Font      = DS.BodyBold,
                ForeColor = DS.TextPrimary,
                AutoSize  = true,
                Location  = new Point(16, 10),
            };

            var details = new Label
            {
                Text      = $"Qty: {Item.Amount}  ·  Unit: ₪{Item.BasePrice:N2}",
                Font      = DS.CaptionFont,
                ForeColor = DS.TextMuted,
                AutoSize  = true,
                Location  = new Point(16, 36),
            };

            bool hasSale = Item.Sales?.Count > 0;
            if (hasSale)
            {
                var saleTag = new Label
                {
                    Text      = "🏷 PROMO",
                    Font      = DS.CaptionFont,
                    ForeColor = DS.Warning,
                    AutoSize  = true,
                    Location  = new Point(180, 36),
                };
                Controls.Add(saleTag);
            }

            var finalPrice = new Label
            {
                Text      = $"₪{Item.FinalPrice:N2}",
                Font      = DS.SectionFont,
                ForeColor = DS.Accent,
                AutoSize  = true,
                Location  = new Point(500, 20),
            };

            var btnRemove = new SmallAccentButton("✕", DS.Danger);
            btnRemove.Size     = new Size(32, 32);
            btnRemove.Location = new Point(660, 22);
            btnRemove.Click += (s, e) => RemoveClicked?.Invoke(this, e);

            Controls.AddRange(new Control[] { nameLabel, details, finalPrice, btnRemove });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var pen = new Pen(DS.Hairline, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            // Left accent stripe
            using var accentBrush = new SolidBrush(DS.AccentDim);
            e.Graphics.FillRectangle(accentBrush, 0, 0, 3, Height);
        }
    }

    // =========================================================================
    // ProductPickerTile – tile in the POS product catalog modal
    // =========================================================================
    public class ProductPickerTile : Panel
    {
        private bool _hover;
        public BO.Product Product { get; }
        public event EventHandler? Selected;

        public ProductPickerTile(BO.Product product)
        {
            Product   = product;
            Size      = new Size(200, 160);
            BackColor = DS.ModalBg;
            Margin    = new Padding(8);
            Cursor    = Cursors.Hand;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            BuildInternals();
        }

        private void BuildInternals()
        {
            var icon = new Label
            {
                Text     = DS.CategoryIcon(Product.Category),
                Font     = new Font("Segoe UI", 32F),
                ForeColor= DS.CategoryColor(Product.Category),
                AutoSize = true,
                Location = new Point(70, 14),
            };

            var name = new Label
            {
                Text        = Product.ProductName,
                Font        = DS.CaptionFont,
                ForeColor   = DS.TextPrimary,
                AutoSize    = false,
                Size        = new Size(180, 32),
                Location    = new Point(10, 72),
                TextAlign   = ContentAlignment.MiddleCenter,
                MaximumSize = new Size(180, 32),
            };

            var price = new Label
            {
                Text      = $"₪{Product.Price:N2}",
                Font      = DS.BodyBold,
                ForeColor = DS.Accent,
                AutoSize  = true,
                Location  = new Point(60, 108),
            };

            Controls.AddRange(new Control[] { icon, name, price });
            Click += (s, e) => Selected?.Invoke(this, e);
            foreach (Control c in Controls) c.Click += (s, e) => Selected?.Invoke(this, e);
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true;  Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_hover)
            {
                using var glowBrush = new SolidBrush(Color.FromArgb(15, DS.Accent));
                g.FillRectangle(glowBrush, ClientRectangle);
            }

            var borderColor = _hover ? DS.Accent : DS.Hairline;
            using var pen = new Pen(borderColor, _hover ? 1.5f : 1f);
            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            if (_hover)
            {
                using var topGlow = new LinearGradientBrush(
                    new Rectangle(0, 0, Width, 4), DS.Accent, Color.Transparent, 90f);
                g.FillRectangle(topGlow, 0, 0, Width, 4);
            }
        }
    }

    // =========================================================================
    // TotalWidget – large glowing "Final Total" display
    // =========================================================================
    public class TotalWidget : Panel
    {
        private double _total;
        public double Total
        {
            get => _total;
            set { _total = value; Invalidate(); }
        }

        public TotalWidget()
        {
            Size      = new Size(340, 120);
            BackColor = DS.CardBg;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Border with glow
            using var pen = new Pen(DS.Accent, 1.5f);
            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            // Top gradient stripe
            using var topBrush = new LinearGradientBrush(
                new Rectangle(0, 0, Width, 4), DS.Accent, Color.Transparent, 0f);
            g.FillRectangle(topBrush, 0, 0, Width, 4);

            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            using var labelBrush = new SolidBrush(DS.TextMuted);
            g.DrawString("TOTAL", DS.CaptionFont, labelBrush,
                new RectangleF(0, 10, Width, 24), sf);

            using var totalBrush = new SolidBrush(DS.Accent);
            g.DrawString($"₪{_total:N2}", DS.BigPriceFont, totalBrush,
                new RectangleF(0, 36, Width, 60), sf);
        }
    }

    // =========================================================================
    // FilterComboBox – styled dark dropdown
    // =========================================================================
    public class FilterComboBox : ComboBox
    {
        public FilterComboBox()
        {
            FlatStyle    = FlatStyle.Flat;
            DropDownStyle= ComboBoxStyle.DropDownList;
            BackColor    = DS.CardBg;
            ForeColor    = DS.TextPrimary;
            Font         = DS.BodyFont;
        }
    }

    // =========================================================================
    // SearchBox – live-search input with icon
    // =========================================================================
    public class SearchBox : Panel
    {
        public TextBox Input { get; }
        private bool _focused;

        public SearchBox()
        {
            Size      = new Size(280, 40);
            BackColor = DS.CardBg;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            Input = new TextBox
            {
                Location    = new Point(34, 8),
                Size        = new Size(Width - 44, 22),
                BackColor   = DS.CardBg,
                ForeColor   = DS.TextPrimary,
                BorderStyle = BorderStyle.None,
                Font        = DS.BodyFont,
            };
            Input.GotFocus  += (s, e) => { _focused = true;  Invalidate(); };
            Input.LostFocus += (s, e) => { _focused = false; Invalidate(); };
            Controls.Add(Input);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var borderColor = _focused ? DS.Accent : DS.Hairline;
            using var pen = new Pen(borderColor, _focused ? 1.5f : 1f);
            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

            using var iconBrush = new SolidBrush(_focused ? DS.Accent : DS.TextMuted);
            g.DrawString("🔍", DS.CaptionFont, iconBrush, new PointF(6, 10));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Input.Width = Width - 44;
        }
    }
}
