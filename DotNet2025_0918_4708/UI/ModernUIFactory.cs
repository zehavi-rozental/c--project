using BO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    internal static class ModernUIFactory
    {
        public static readonly Color AppBackground = Color.FromArgb(8, 12, 24);
        public static readonly Color SidebarBackground = Color.FromArgb(10, 18, 40);
        public static readonly Color PanelBackground = Color.FromArgb(12, 18, 34);
        public static readonly Color CardBackground = Color.FromArgb(18, 28, 52);
        public static readonly Color CardBorder = Color.FromArgb(36, 156, 188);
        public static readonly Color Accent = Color.FromArgb(0, 205, 178);
        public static readonly Color AccentSoft = Color.FromArgb(35, 126, 159);
        public static readonly Color TextPrimary = Color.WhiteSmoke;
        public static readonly Color TextSecondary = Color.FromArgb(176, 196, 255);
        public static readonly Font TitleFont = new Font("Segoe UI Variable Display", 22F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font SectionFont = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font BodyFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font ButtonFont = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);

        public static void StyleForm(Form form)
        {
            form.BackColor = AppBackground;
            form.ForeColor = TextPrimary;
            form.Font = BodyFont;
            form.Text = "SmartHome Hub";
        }

        public static Button CreateNavButton(string text, EventHandler click)
        {
            var button = new Button
            {
                Text = text,
                ForeColor = TextPrimary,
                BackColor = Color.FromArgb(10, 20, 38),
                FlatStyle = FlatStyle.Flat,
                Font = ButtonFont,
                Size = new Size(216, 52),
                Cursor = Cursors.Hand,
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += click;
            return button;
        }

        public static Label CreateHeaderLabel(string text)
        {
            return new Label
            {
                Text = text,
                ForeColor = TextPrimary,
                Font = SectionFont,
                AutoSize = true,
            };
        }

        public static Panel CreateTopHero(string title, string subtitle)
        {
            var hero = new Panel
            {
                Size = new Size(1080, 160),
                BackColor = Color.FromArgb(16, 28, 50),
                BorderStyle = BorderStyle.None,
            };

            var titleLabel = new Label
            {
                Text = title,
                ForeColor = Accent,
                Font = TitleFont,
                AutoSize = true,
                Location = new Point(24, 24),
            };

            var subtitleLabel = new Label
            {
                Text = subtitle,
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(26, 78),
                MaximumSize = new Size(860, 0),
            };

            hero.Controls.AddRange(new Control[] { titleLabel, subtitleLabel });
            return hero;
        }

        public static Panel CreateCard(string title, string subtitle, Point location)
        {
            var card = new Panel
            {
                Size = new Size(300, 140),
                Location = location,
                BackColor = CardBackground,
                BorderStyle = BorderStyle.None,
            };
            card.Paint += (sender, args) =>
            {
                using var pen = new Pen(CardBorder, 2);
                args.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            card.Controls.Add(new Label
            {
                Text = title,
                ForeColor = Accent,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(18, 18),
            });
            card.Controls.Add(new Label
            {
                Text = subtitle,
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                MaximumSize = new Size(264, 0),
                Location = new Point(18, 56),
            });
            return card;
        }

        public static TextBox CreateInputBox(string placeholder)
        {
            return new TextBox
            {
                ForeColor = TextPrimary,
                BackColor = Color.FromArgb(14, 22, 38),
                BorderStyle = BorderStyle.None,
                Font = BodyFont,
                Size = new Size(320, 36),
                Text = string.Empty,
                Tag = placeholder,
            };
        }

        public static Panel CreatePanelBox(Point location, Size size)
        {
            return new Panel
            {
                Location = location,
                Size = size,
                BackColor = CardBackground,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(16),
            };
        }

        public static Button CreateAccentButton(string text, EventHandler click)
        {
            var button = new Button
            {
                Text = text,
                ForeColor = Color.White,
                BackColor = Accent,
                FlatStyle = FlatStyle.Flat,
                Font = ButtonFont,
                Size = new Size(160, 42),
                Cursor = Cursors.Hand,
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(0, 161, 140);
            button.FlatAppearance.BorderSize = 0;
            button.Click += click;
            return button;
        }

        public static FlowLayoutPanel CreateCardsPanel(Point location, Size size)
        {
            return new FlowLayoutPanel
            {
                Location = location,
                Size = size,
                BackColor = Color.Transparent,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
            };
        }

        public static Panel CreateShopProductTile(Product product, EventHandler addToCartClick)
        {
            var tile = new Panel
            {
                Size = new Size(280, 200),
                BackColor = Color.FromArgb(16, 25, 44),
                Cursor = Cursors.Hand,
                Margin = new Padding(12),
            };
            tile.Paint += (sender, args) =>
            {
                using var pen = new Pen(AccentSoft, 2);
                args.Graphics.DrawRectangle(pen, 0, 0, tile.Width - 1, tile.Height - 1);
            };

            // Icon placeholder - could be replaced with actual images
            var icon = new Label
            {
                Text = GetCategoryIcon(product.Category),
                Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Accent,
                AutoSize = true,
                Location = new Point(14, 14),
            };
            tile.Controls.Add(icon);

            var name = new Label
            {
                Text = product.ProductName,
                ForeColor = Accent,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(80, 20),
                MaximumSize = new Size(180, 0),
            };
            tile.Controls.Add(name);

            var category = new Label
            {
                Text = GetFriendlyCategory(product.Category),
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(80, 50),
            };
            tile.Controls.Add(category);

            var price = new Label
            {
                Text = $"₪{product.Price:N2}",
                ForeColor = Color.FromArgb(152, 255, 210),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(80, 80),
            };
            tile.Controls.Add(price);

            var stock = new Label
            {
                Text = $"In stock: {product.Ammount}",
                ForeColor = product.Ammount > 0 ? TextSecondary : Color.FromArgb(255, 100, 100),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(14, 140),
            };
            tile.Controls.Add(stock);

            var btnAdd = CreateAccentButton("Add to Cart", addToCartClick);
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(140, 150);
            btnAdd.Tag = product;
            tile.Controls.Add(btnAdd);

            return tile;
        }

        public static Panel CreateCartItemTile(ProductInOrder item, EventHandler removeClick)
        {
            var tile = new Panel
            {
                Size = new Size(950, 80),
                BackColor = Color.FromArgb(16, 25, 44),
                Margin = new Padding(8),
            };
            tile.Paint += (sender, args) =>
            {
                using var pen = new Pen(Color.FromArgb(54, 201, 177), 2);
                args.Graphics.DrawRectangle(pen, 0, 0, tile.Width - 1, tile.Height - 1);
            };

            tile.Controls.Add(new Label
            {
                Text = item.ProductName,
                ForeColor = Accent,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(14, 14),
            });
            tile.Controls.Add(new Label
            {
                Text = $"Quantity: {item.Amount}  •  Unit: ₪{item.BasePrice:N2}  •  Total: ₪{item.FinalPrice:N2}",
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(14, 42),
            });

            var btnRemove = CreateAccentButton("Remove", removeClick);
            btnRemove.Size = new Size(100, 35);
            btnRemove.Location = new Point(820, 22);
            btnRemove.Tag = item;
            btnRemove.BackColor = Color.FromArgb(200, 50, 50);
            tile.Controls.Add(btnRemove);

            return tile;
        }

        public static string GetCategoryIcon(Category category)
        {
            return category switch
            {
                Category.LIGHTING => "💡",
                Category.SECURITY => "🔒",
                Category.CLIMATE => "🌡️",
                Category.AUDIO => "🔊",
                _ => "📦",
            };
        }

        public static string GetFriendlyCategory(Category category)
        {
            return category switch
            {
                Category.LIGHTING => "Lighting",
                Category.SECURITY => "Security",
                Category.CLIMATE => "Climate",
                Category.AUDIO => "Audio",
                _ => category.ToString(),
            };
        }
    }
}
