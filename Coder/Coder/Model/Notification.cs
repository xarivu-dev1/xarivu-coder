using System;

namespace Xarivu.Coder.Model
{
    public class Notification
    {
        public Notification(NotificationTypeEnum notificationType, string message)
        {
            this.NotificationType = notificationType;
            this.Message = message;
        }

        public Notification(NotificationTypeEnum notificationType, string message, Exception ex)
            : this(notificationType, message)
        {
            this.ExceptionObj = ex;
        }

        public NotificationTypeEnum NotificationType
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public Exception ExceptionObj
        {
            get;
            private set;
        }
    }
}
