namespace WinformNotifications.Notifications
{
    public interface INotificationManager
    {
        /// <summary>
        /// Creates and displays a notification.
        /// </summary>
        public int ShowNotification(
            NotificationType type,
            string title,
            string message,
            int durationMs = 3000,
            Image? customImage = null);

        /// <summary>
        /// Creates and displays a green success notification.
        /// </summary>
        public void ShowSuccess(string title, string message);

        /// <summary>
        /// Creates and displays an red error notification.
        /// </summary>
        public void ShowError(string title, string message);

        /// <summary>
        /// Close a notification programatically by its Id
        /// </summary>
        /// <param name="notificationId">Id returned by notification creation</param>
        public void CloseNotificationById(int notificationId);
    }
}
