namespace WinformNotifications.Notifications
{
    using System.Windows.Forms;

    public class CircularLoadingIndicator : Control
    {
        /// <summary>
        /// Decreasing colors for each orb
        /// </summary>
        private List<Color> _ballColors = new();

        /// <summary>
        /// Number of orbs in the circle
        /// </summary>
        private int _ballCount = 8;

        /// <summary>
        /// Size of the loading orbs
        /// </summary>
        private int _ballSize = 8;

        /// <summary>
        /// Tracks the locations of the loading orbs
        /// </summary>
        private int _currentAngle = 0;

        private double _dpiScale = 1;

        /// <summary>
        /// Radius the orbs will follow
        /// </summary>
        private int _pathRadius;

        private Size _size = new(64, 64);

        /// <summary>
        /// Update per x milliseconds
        /// </summary>
        private int _speed = 24;

        private Timer _timer = new();

        public Color BallColor
        {
            get => _ballColors.Last();
            set
            {
                _ballColors.Clear();
                int maxOpacity = 255;
                int decrement = maxOpacity / _ballCount;
                for (int i = 0; i < _ballCount; i++)
                {
                    _ballColors.Add(Color.FromArgb(maxOpacity - (decrement * i), value));
                }
                _ballColors.Reverse();
                Invalidate();
            }
        }

        /// <summary>
        /// A custom circular loading indicator that can be set to a specific color
        /// </summary>
        public CircularLoadingIndicator()
        {
            _dpiScale = this.DeviceDpi / 96.0d;
            _size = new Size((int)(_size.Width * _dpiScale), (int)(_size.Height * _dpiScale));
            _ballSize = Convert.ToInt32(_ballSize * _dpiScale);
            this.DoubleBuffered = true;
            this.Size = _size;
            this.MinimumSize = _size;
            this.MaximumSize = _size;
            _pathRadius = (this.Width / 2) - (_ballSize * 2);
            BallColor = Color.DarkGray;
            InitTimer();
        }

        private void InitTimer()
        {
            _timer.Interval = _speed;
            _timer.Tick += (sender, e) =>
            {
                _currentAngle = (_currentAngle + 4) % 360;
                this.Invalidate();
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (int i = 0; i < _ballCount; i++)
            {
                int currentAngle = (_currentAngle + (i * 360 / _ballCount)) % 360;
                float x = (float)((this.Width / 2)
                    + ((_pathRadius * Math.Cos(currentAngle * (Math.PI / 180))) - (10 * _dpiScale)));
                float y = (float)((this.Height / 2)
                    + ((_pathRadius * Math.Sin(currentAngle * (Math.PI / 180))) - (10 * _dpiScale)));

                Brush brush = new SolidBrush(_ballColors[i]);
                e.Graphics.FillEllipse(brush, x, y, _ballSize, _ballSize);
                brush.Dispose();
            }
        }

        /// <summary>
        /// begin the animation timer
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// stop the animation timer
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }
    }
}