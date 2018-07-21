namespace Xarivu.Coder.Model.Tracked
{
    public enum PropertyChangedNotificationType
    {
        /// <summary>
        /// Replays the change notifications in order after an action, that potentially changes multiple properties, is completed.
        /// </summary>
        Delayed,

        /// <summary>
        /// No change notifications are sent while action is executing.
        /// </summary>
        Disabled
    }
}
