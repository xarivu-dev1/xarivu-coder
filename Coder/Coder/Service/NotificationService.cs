using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xarivu.Coder.Model;

namespace Xarivu.Coder.Service
{
    public class NotificationService
    {
        ConcurrentQueue<Notification> notificationQueue;
        Dictionary<Type, Func<Exception, string>> notificationExceptionHandler;

        public NotificationService()
        {
            this.notificationQueue = new ConcurrentQueue<Notification>();
            this.notificationExceptionHandler = new Dictionary<Type, Func<Exception, string>>();
        }

        public event Action NotificationAvailable;

        public void AddNotification(Notification notification)
        {
            var exceptionType = notification.ExceptionObj?.GetType();
            if (exceptionType != null && this.notificationExceptionHandler.ContainsKey(exceptionType))
            {
                var handler = this.notificationExceptionHandler[exceptionType];
                var message = handler(notification.ExceptionObj);
                var updatedNotification = new Notification(notification.NotificationType, message);
                notification = updatedNotification;
            }

            this.notificationQueue.Enqueue(notification);

            if (this.NotificationAvailable != null)
            {
                this.NotificationAvailable();
            }
        }

        public void AddNotificationExceptionHandler<T>(Func<T, string> exceptionHandler)
            where T : Exception
        {
            this.notificationExceptionHandler.Add(typeof(T), e => exceptionHandler(e as T));
        }

        public bool TryGetNotification(out Notification notification)
        {
            return this.notificationQueue.TryDequeue(out notification);
        }
    }
}
