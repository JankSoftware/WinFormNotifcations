using System.Text;
using WinformNotifications.Notifications;

namespace WinformNotifications
{
    public partial class Form1 : Form
    {
        private Button btnShowToast;
        private Button btnShowSuccess;
        private Button btnShowError;
        private Button btnShowPersistent;
        private NotificationManager _notificationManager = new();


        public Form1()
        {
            InitializeComponent();

            this.Text = "Notification Example";
            this.Size = new Size(300, 400);

            Random random = new Random();
            Bitmap bmp = new Bitmap(64, 64);

            btnShowToast = new Button
            {
                Text = "Show Notification",
                Size = new Size(200, 40),
                Location = new Point(50, 260)
            };
            btnShowToast.Click += (s, e) =>
            {
                var unique = random.Next(0, 5000);
                var length = random.Next(200, 600);
                var even = unique % 2 == 0;
                _notificationManager.CreateNotification(
                    title: $"{unique} Title",
                    message: $"This is a message {(even ? "without" : "with")} a loader; " + GenerateGarbageText(length),
                    showLoader: !even,
                    customImage: even ? bmp : null);
            };


            btnShowSuccess = new Button
            {
                Text = "Show Success",
                Size = new Size(200, 40),
                Location = new Point(50, 50)
            };
            btnShowSuccess.Click += (s, e) =>
                _notificationManager.ShowSuccess("Success", "Operation completed successfully!");

            btnShowError = new Button
            {
                Text = "Show Error",
                Size = new Size(200, 40),
                Location = new Point(50, 120)
            };
            btnShowError.Click += (s, e) =>
                _notificationManager.ShowError("Error", "An error occurred!");

            btnShowPersistent = new Button
            {
                Text = "Show Persistent",
                Size = new Size(200, 40),
                Location = new Point(50, 190)
            };
            btnShowPersistent.Click += (s, e) =>
                _notificationManager.CreateNotification("Persistent", "This notification will stay until clicked.", false, null, null, true);



            this.Controls.Add(btnShowToast);
            this.Controls.Add(btnShowSuccess);
            this.Controls.Add(btnShowError);
            this.Controls.Add(btnShowPersistent);
        }

        /// <summary>
        /// Generate some randome garbage to make different length notifications
        /// </summary>
        private string GenerateGarbageText(int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?";
            Random random = new Random();
            int length = random.Next(1, maxLength + 1);
            StringBuilder builder = new StringBuilder(length);

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
