namespace WinformNotifications.Notifications
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class Notification : Form
    {
        private const int _fadeSpeed = 12;
        private const double _initialOpacity = 0.9;
        private const int _maxMessageLines = 5;
        private const int FadeOutInterval = 20;

        private Timer _closeTimer;
        private double _dpiScale = 1;
        private Timer _fadeTimer;
        private Font _messageFont;
        private Label _messageLabel;
        private Control _notificationGraphic;
        private Size _notificationSize = new(380, 76);
        private string _parentMonitorName;
        private bool _persistent;

        /// <summary>
        /// Sets if the notifications are semitransparent
        /// and fully on hover or not
        /// </summary>
        private bool _supportOpacity = false;

        private Font _titleFont;
        private Label _titleLabel;
        public string ParentMonitor => _parentMonitorName;

        public int Id { get; private set; }

        public Notification(
            string title,
            string message,
            string monitorDeviceName,
            int durationMs = 3000,
            bool showLoader = false,
            Image? customImage = null,
            Color? backgroundColor = null,
            bool persistent = false)
        {
            _persistent = persistent;
            _parentMonitorName = monitorDeviceName;
            _dpiScale = this.DeviceDpi / 96.0d;
            _notificationSize = new(
                Convert.ToInt32(_notificationSize.Width * _dpiScale),
                Convert.ToInt32(_notificationSize.Height * _dpiScale));

            _titleFont = new Font("Tahoma", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            _messageFont = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);

            // DPI scaling
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.Opacity = _supportOpacity ? _initialOpacity : 1;
            this.BackColor = backgroundColor ?? Color.FromArgb(50, 50, 50); // Default or provided color
            this.Size = _notificationSize;
            this.Region = Region.FromHrgn(CreateRoundRectRegion(0, 0, this.Width, this.Height, 15, 15));
            TableLayoutPanel panel = CreateLayout(title, message, showLoader, customImage);
            this.Controls.Add(panel);

            // Sets up fade out
            if (!_persistent)
            {
                _closeTimer = new Timer { Interval = durationMs };
                _closeTimer.Tick += (s, e) => StartFadeOut();
                _closeTimer.Start();
            }

            // Fade-out timer
            _fadeTimer = new Timer { Interval = FadeOutInterval };
            _fadeTimer.Tick += PerformFadeOut;

            EnableOpacity();
            AddClickEventHandlers(this);
            Id = this.GetHashCode();
        }

        // P/Invoke for creating a region with rounded corners
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRegion(
            int leftRect,
            int topRect,
            int rightRect,
            int bottomRect,
            int widthEllipse,
            int heightEllipse);

        /// <summary>
        /// Add click event to all child controls
        /// </summary>
        private void AddClickEventHandlers(Control control)
        {
            control.Click += (_, __) => this.Close();
            foreach (Control child in control.Controls)
            {
                AddClickEventHandlers(child);
            }
        }

        /// <summary>
        /// Adjusts the size to include longer messages
        /// </summary>
        /// <param name="message"></param>
        private void AdjustSizeForMessage(string message)
        {
            using (Graphics g = CreateGraphics())
            {
                int lineHeight = Convert.ToInt32(g.MeasureString("|", _messageFont).Height);
                SizeF textSize = g.MeasureString(message, _messageFont, _messageLabel.MaximumSize.Width);
                int lines = (int)Math.Ceiling(textSize.Height / lineHeight);

                // Limit the lines to the maximum allowed
                lines = Math.Min(lines, _maxMessageLines);
                int newHeight = Math.Max(_notificationSize.Height, _titleLabel.Height + (lines * lineHeight));
                this.MaximumSize = new Size(this.Width, newHeight);
                this.Size = new Size(this.Width, newHeight);
                this.Region = Region.FromHrgn(CreateRoundRectRegion(0, 0, this.Width, this.Height, 15, 15));
                _messageLabel.MaximumSize = new Size(_messageLabel.MaximumSize.Width, lines * lineHeight);
                _messageLabel.Size = new Size(_messageLabel.MaximumSize.Width, lines * lineHeight);
            }
        }

        /// <summary>
        /// Creates the images and text of the noticiation body
        /// </summary>
        private TableLayoutPanel CreateLayout(string title, string message, bool showLoader, Image customImage)
        {
            // Layout panel
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10),
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Loader spin or custom image
            if (customImage != null)
            {
                int x = Convert.ToInt32(50 * _dpiScale);
                _notificationGraphic = new PictureBox
                {
                    Image = customImage,
                    Size = new Size(x, x),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Magenta,
                    Dock = DockStyle.Top
                };
            }
            else if (showLoader)
            {
                int x = Convert.ToInt32(64 * _dpiScale);
                _notificationGraphic = new CircularLoadingIndicator
                {
                    Size = new Size(x, x),
                    Dock = DockStyle.Left,
                };
                ((CircularLoadingIndicator)_notificationGraphic).Start();
            }

            if (_notificationGraphic != null)
            {
                panel.Controls.Add(_notificationGraphic, 0, 0);
                panel.SetRowSpan(_notificationGraphic, 2);
            }

            // Title text label
            _titleLabel = new Label
            {
                Text = title,
                Font = _titleFont,
                ForeColor = Color.White,
                AutoEllipsis = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                MaximumSize = new Size(
                    Convert.ToInt32(300 * _dpiScale),
                    Convert.ToInt32(20 * _dpiScale)),
                AutoSize = false
            };
            panel.Controls.Add(_titleLabel, 1, 0);

            // Message text label
            _messageLabel = new Label
            {
                Text = message,
                Font = _messageFont,
                ForeColor = Color.WhiteSmoke,
                AutoSize = false,
                MaximumSize = new Size(
                    Convert.ToInt32(300 * _dpiScale),
                    Convert.ToInt32(60 * _dpiScale)),
                AutoEllipsis = true,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(_messageLabel, 1, 1);
            AdjustSizeForMessage(message);
            return panel;
        }

        /// <summary>
        /// Sets up mouse hover opacity
        /// </summary>
        private void EnableOpacity()
        {
            if (_supportOpacity)
            {
                /* Mouse events for hover effect
                // full on hover, _initialOpacity on not */
                this.MouseEnter += (s, e) => this.Opacity = 1.0;
                this.MouseLeave += (s, e) => this.Opacity = _initialOpacity;
            }
        }

        private void PerformFadeOut(object? sender, EventArgs e)
        {
            if (Opacity > 0.0)
            {
                // fade-out
                Opacity -= 0.1;
            }
            else
            {
                _fadeTimer.Stop();
                if (_notificationGraphic is CircularLoadingIndicator loader)
                {
                    loader.Stop();
                }
                this.Close();
            }
        }

        private void StartFadeOut()
        {
            _closeTimer.Stop();
            _fadeTimer.Start();
        }
    }
}