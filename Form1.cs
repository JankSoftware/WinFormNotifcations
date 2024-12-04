using System.Text;
using WinformNotifications.Notifications;

namespace WinformNotifications
{
    public partial class Form1 : Form
    {
        private List<int> _notificationIds = new();
        private readonly Button _btnShowToast;
        private readonly Button _btnShowSuccess;
        private readonly Button _btnShowError;
        private readonly Button _btnShowPersistent;
        private readonly Button _btnClosePersistent;
        private readonly ComboBox _cmbIds;
        private INotificationManager _notificationManager = new NotificationManager();


        public Form1()
        {
            InitializeComponent();

            this.Text = "Notification Example";
            this.Size = new Size(350, 500);

            Random random = new();
            Bitmap bmp = new(64, 64);

            _btnShowToast = new Button
            {
                Text = "Show Notification",
                Size = new Size(200, 40),
                Location = new Point(50, 190)
            };
            _btnShowToast.Click += (s, e) =>
            {
                var unique = random.Next(0, 5000);
                var length = random.Next(200, 600);
                var even = unique % 2 == 0;
                _notificationManager.ShowNotification(
                    type: !even ? NotificationType.Loading : NotificationType.Standard,
                    title: $"{unique} Title",
                    message: $"This is a message {(even ? "without" : "with")} a loader; " + GenerateGarbageText(length),
                    customImage: even ? bmp : null);
            };


            _btnShowSuccess = new Button
            {
                Text = "Show Success",
                Size = new Size(200, 40),
                Location = new Point(50, 50)
            };
            _btnShowSuccess.Click += (s, e) =>
                _notificationManager.ShowSuccess("Success", "Operation completed successfully!");

            _btnShowError = new Button
            {
                Text = "Show Error",
                Size = new Size(200, 40),
                Location = new Point(50, 120)
            };
            _btnShowError.Click += (s, e) =>
                _notificationManager.ShowError("Error", "An error occurred!");

            _btnShowPersistent = new Button
            {
                Text = "Show Persistent",
                Size = new Size(200, 40),
                Location = new Point(50, 260)
            };
            _btnShowPersistent.Click += BtnShowPersistent_Click;

            _cmbIds = new ComboBox
            {
                Width = 200,
                Location = new Point(25, 340),
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            _btnClosePersistent = new Button
            {
                Text = "Close",
                Size = new Size(100, 40),
                Location = new Point(230, 330)
            };
            _btnClosePersistent.Click += _btnClosePersistent_Click;


            this.Controls.Add(_btnShowToast);
            this.Controls.Add(_btnShowSuccess);
            this.Controls.Add(_btnShowError);
            this.Controls.Add(_btnShowPersistent);
            this.Controls.Add(_cmbIds);
            this.Controls.Add(_btnClosePersistent);
        }

        private void _btnClosePersistent_Click(object? sender, EventArgs e)
        {
            if(_cmbIds.SelectedItem is int id)
            {
                _notificationManager.CloseNotificationById(id);
                _cmbIds.Items.Remove(id);
                if (_cmbIds.Items.Count > 0)
                {
                    _cmbIds.SelectedIndex = 0;
                }
                    
                Refresh();
            }
        }

        private void BtnShowPersistent_Click(object? sender, EventArgs e)
        {
            var id = _notificationManager.ShowNotification(
                    NotificationType.Persistant,
                    "Persistent",
                    "This notification will stay until clicked or closed from the form.");

            _cmbIds.Items.Add(id);
            _cmbIds.SelectedIndex = 0;
            Refresh();
        }

        /// <summary>
        /// Generate some randome garbage to make different length notifications
        /// </summary>
        private string GenerateGarbageText(int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?";
            Random random = new();
            int length = random.Next(1, maxLength + 1);
            StringBuilder builder = new(length);

            for (int i = 0; i < length; i++)
            {
                // Add a random character
                char randomChar = chars[random.Next(chars.Length)];
                builder.Append(randomChar);

                // Occasionally insert a space
                if (random.Next(0, 5) == 0 && i < length - 1)
                {
                    builder.Append(' ');
                }
            }

            return builder.ToString();
        }
    }
}
