namespace WinformNotifications.Notifications
{
    using System.Collections.Generic;

    public class NotificationManager
    {
        /// <summary>
        /// holds the active notifications grouped by which monitor they belong to
        /// </summary>
        private Dictionary<string, List<Notification>> _activeNotifications;

        /// <summary>
        /// vertical space between toasts
        /// </summary>
        private readonly int _toastSpacing = 10;
        private Dictionary<string, Screen> _screens;

        public NotificationManager()
        {
            _screens = new();
            _activeNotifications = new();
        }

        /// <summary>
        /// Adds a connected screen to the list of available ones
        /// <br /> used to arrange which notifications are on different displays
        /// </summary>
        private Screen SafeAddScreen(Screen screen)
        {
            if (!_screens.ContainsKey(screen.DeviceName))
            {
                _screens.Add(screen.DeviceName, screen);
            }

            return _screens[screen.DeviceName];
        }

        /// <summary>
        /// Adds a notification to the active list arranged by screen
        /// </summary>
        private void SafeAddNotification(string screenName, Notification notification)
        {
            if (!_activeNotifications.ContainsKey(screenName))
            {
                _activeNotifications.Add(screenName, new());
            }

            _activeNotifications[screenName].Insert(0, notification);
        }

        /// <summary>
        /// Creates and displays a standard toast notification.
        /// </summary>
        public void CreateNotification(string title, string message, bool showLoader = false, Image? customImage = null, Color? backgroundColor = null, bool persistent = false)
        {
            var activeScreen = SafeAddScreen(Screen.FromPoint(Cursor.Position));
            var newNotification = new Notification(
                title: title,
                message: message,
                monitorDeviceName: activeScreen.DeviceName,
                durationMs: 3000,
                showLoader: showLoader,
                customImage: customImage,
                backgroundColor: backgroundColor,
                persistent: persistent);
            SafeAddNotification(activeScreen.DeviceName, newNotification);
            newNotification.FormClosed += (_, __) => OnNotificationClosed(newNotification);
            UpdateToastPositions();
            newNotification.Show();
        }

        /// <summary>
        /// Creates and displays a green success notification.
        /// </summary>
        public void ShowSuccess(string title, string message)
        {
            CreateNotification(
                title,
                message,
                false,
                null,
                Color.FromArgb(255, 50, 205, 50),
                false);
        }

        /// <summary>
        /// Creates and displays an red error notification.
        /// </summary>
        public void ShowError(string title, string message)
        {
            CreateNotification(
                title,
                message,
                false,
                null,
                Color.FromArgb(255, 205, 50, 50),
                false);
        }

        /// <summary>
        /// Updates the positions of all active toasts in the bottom-right corner.
        /// </summary>
        private void UpdateToastPositions()
        {
            foreach (var toastList in _activeNotifications)
            {
                int bottomOffset = _toastSpacing;
                foreach (var toast in toastList.Value)
                {
                    if (_screens[toast.ParentMonitor].WorkingArea is Rectangle screen)
                    {
                        // Align to right and stack upwards
                        toast.Location = new Point(
                            screen.Right - toast.Width - _toastSpacing,
                            screen.Bottom - toast.Height - bottomOffset
                        );
                        bottomOffset += toast.Height + _toastSpacing;
                    }
                }

            }
        }

        /// <summary>
        /// Called when a toast notification is closed.
        /// </summary>
        private void OnNotificationClosed(Notification toast)
        {
            foreach (var notificationList in _activeNotifications)
            {
                notificationList.Value.Remove(toast);
            }

            UpdateToastPositions();
        }
    }
}
