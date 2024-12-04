namespace WinformNotifications.Notifications
{
    /// <summary>
    /// <list>
    /// <item><description>Standard - A notification with an optional image</description></item>
    /// <item><description>Loading - A notification with an animated loading indicator</description></item>
    /// <item><description>Persistant - A notification that stays open until manually clicked or programatically closed</description></item>
    /// <item><description>Error - A red notification used to denote an error</description></item>
    /// <item><description>Success - A green notification used to denote success</description></item>
    /// </list>
    /// </summary>
    public enum NotificationType
    {
        None = 0,
        Standard = 1,
        Loading = 2,
        Persistant = 3,
        Error = 4,
        Success = 5,
    }
}
