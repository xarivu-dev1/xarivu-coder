using Xarivu.Coder.Model;
using Xarivu.Coder.Model.Tracked;
using Xarivu.Coder.Service;
using Xarivu.Coder.ViewModel;
using Xarivu.Coder.ViewModel.Dialog;
using System;
using System.Text;

namespace Xarivu.Coder.ViewModel
{
    public class NotificationViewModel : NotifyChangeBase
    {
        NotificationService notificationService;
        DialogServiceViewModel dialogServiceViewModel;

        StringBuilder sbNotificationText;

        public NotificationViewModel(
            NotificationService notificationService,
            DialogServiceViewModel dialogServiceViewModel)
        {
            this.notificationService = notificationService;
            this.dialogServiceViewModel = dialogServiceViewModel;

            this.ClearCommand = new DelegateCommand(p => ClearAction());

            this.sbNotificationText = new StringBuilder();

            this.notificationService.NotificationAvailable += NotificationService_NotificationAvailable;
        }

        public DelegateCommand ClearCommand { get; private set; }

        #region NotificationText
        string __NotificationText;
        public string NotificationText
        {
            get
            {
                return this.__NotificationText;
            }

            set
            {
                if (this.__NotificationText != value)
                {
                    this.__NotificationText = value;
                    NotifyPropertyChanged(nameof(NotificationText));
                }
            }
        }
        #endregion

        private void NotificationService_NotificationAvailable()
        {
            var text = GetNotificationText();

            this.dialogServiceViewModel.DispatchToUIThread(
                () =>
                {
                    this.NotificationText = text;
                });
        }

        string GetNotificationText()
        {
            Notification notification = null;
            while (this.notificationService.TryGetNotification(out notification))
            {
                this.sbNotificationText.AppendLine($"{DateTime.Now.ToLocalTime().ToShortTimeString()} | {notification.NotificationType} | {notification.Message}");
                if (notification.ExceptionObj != null)
                {
                    this.sbNotificationText.AppendLine($"{notification.ExceptionObj.ToString()}");
                }

                // Trim logic
                var len = this.sbNotificationText.Length;
                if (len > 10000)
                {
                    // Keep at most 10000 chars.
                    var remove = len - 10000;
                    this.sbNotificationText.Remove(0, remove);

                    // Try to start on a new line (i.e. right after '\n' char).
                    // Check another 1000 characters for a '\n'.
                    remove = -1;
                    for (int i = 0; i < this.sbNotificationText.Length && i < 1000; ++i)
                    {
                        if (this.sbNotificationText[i] == '\n')
                        {
                            remove = i + 1;
                            break;
                        }
                    }

                    this.sbNotificationText.Remove(0, remove);
                }
            }

            return this.sbNotificationText.ToString();
        }

        void ClearAction()
        {
            this.sbNotificationText.Clear();
            this.NotificationText = string.Empty;
        }
    }
}
