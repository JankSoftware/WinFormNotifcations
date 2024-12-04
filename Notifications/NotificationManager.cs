namespace WinformNotifications.Notifications
{
    using System.Collections.Generic;

    public class NotificationManager : INotificationManager
    {
        /// <summary>
        /// vertical space between toasts
        /// </summary>
        private readonly int _toastSpacing = 10;

        /// <summary>
        /// holds the active notifications grouped by which monitor they belong to
        /// </summary>
        private Dictionary<string, List<Notification>> _activeNotifications;

        private Dictionary<string, Screen> _screens;

        public NotificationManager()
        {
            _screens = new();
            _activeNotifications = new();
        }

        /// <summary>
        /// Creates and displays a standard toast notification.
        /// </summary>
        private int CreateNotification(
            NotificationType type,
            string title,
            string message,
            int duration = 3000,
            Image? customImage = null,
            Color? backgroundColor = null)
        {
            var activeScreen = SafeAddScreen(Screen.FromPoint(Cursor.Position));
            var newNotification = type switch
            {
                NotificationType.Standard => new Notification(
                    title: title,
                    message: message,
                    monitorDeviceName: activeScreen.DeviceName,
                    durationMs: duration,
                    showLoader: false,
                    customImage: customImage,
                    backgroundColor: backgroundColor,
                    persistent: false),
                NotificationType.Loading => new Notification(
                    title: title,
                    message: message,
                    monitorDeviceName: activeScreen.DeviceName,
                    durationMs: duration,
                    showLoader: true,
                    customImage: null,
                    backgroundColor: backgroundColor,
                    persistent: true),
                NotificationType.Persistant => new Notification(
                    title: title,
                    message: message,
                    monitorDeviceName: activeScreen.DeviceName,
                    durationMs: duration,
                    showLoader: false,
                    customImage: customImage,
                    backgroundColor: backgroundColor,
                    persistent: true),
                NotificationType.Success => new Notification(
                    title: title,
                    message: message,
                    monitorDeviceName: activeScreen.DeviceName,
                    durationMs: duration,
                    showLoader: false,
                    customImage: customImage,
                    backgroundColor: backgroundColor,
                    persistent: false),
                NotificationType.Error => new Notification(
                    title: title,
                    message: message,
                    monitorDeviceName: activeScreen.DeviceName,
                    durationMs: duration,
                    showLoader: false,
                    customImage: customImage,
                    backgroundColor: backgroundColor,
                    persistent: false),
                _ => null
            };

            if (newNotification != null)
            {
                SafeAddNotification(activeScreen.DeviceName, newNotification);
                newNotification.FormClosed += (_, __) => OnNotificationClosed(newNotification);
                UpdateToastPositions();
                newNotification.Show();
                return newNotification.Id;
            }

            return 0;
        }

        /// <summary>
        /// Called when a toast notification is closed.
        /// </summary>
        private void OnNotificationClosed(Notification notification)
        {
            foreach (var notificationList in _activeNotifications)
            {
                if (notificationList.Value.Remove(notification))
                {
                    break;
                }
            }

            UpdateToastPositions();
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
        /// Close a notification programatically by its Id
        /// </summary>
        /// <param name="notificationId">Id returned by notification creation</param>
        public void CloseNotificationById(int notificationId)
        {
            foreach (var notificationList in _activeNotifications)
            {
                if (notificationList.Value.FirstOrDefault(n => n.Id.Equals(notificationId)) is Notification match)
                {
                    notificationList.Value.Remove(match);
                    match.Close();
                    break;
                }
            }
        }

        /// <summary>
        /// Creates and displays an red error notification.
        /// </summary>
        public void ShowError(string title, string message) =>
            CreateNotification(
                type: NotificationType.Error,
                title: title,
                message: message,
                duration: 3000,
                customImage: null,
                backgroundColor: Color.FromArgb(255, 205, 50, 50));

        public int ShowNotification
                                    (NotificationType type,
            string title,
            string message,
            int durationMs = 3000,
            Image? customImage = null) =>
            CreateNotification(
                type: type,
                title: title,
                message: message,
                duration: durationMs,
                customImage: customImage);

        /// <summary>
        /// Creates and displays a green success notification.
        /// </summary>
        public void ShowSuccess(string title, string message) =>
            CreateNotification(
                type: NotificationType.Success,
                title: title,
                message: message,
                duration: 3000,
                customImage: null,
                backgroundColor: Color.FromArgb(255, 50, 205, 50));
    }
}