using Xarivu.Coder.Model;
using Xarivu.Coder.Service;
using Xarivu.Coder.Utilities;
using Xarivu.Coder.ViewModel.Dialog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xarivu.Coder.ViewModel.Utilities
{
    public static class VmUtilities
    {
        /// <summary>
        /// Call this method from UI thread. It has code at the beginning which needs to run
        /// in UI thread and then awaits the async operation.
        /// </summary>
        /// <param name="asyncAction"></param>
        /// <returns></returns>
        public static async Task<bool> StartActionWithProgress(Func<CancellationToken, Task> asyncAction, bool sendNotificationOnError = true)
        {
            bool error = false;

            var dialogServiceVm = DependencyContainer.Get<DialogServiceViewModel>();
            NotificationService notificationService = null;
            if (sendNotificationOnError)
            {
                notificationService = DependencyContainer.Get<NotificationService>();
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            ProgressDialogViewModel progressVm = new ProgressDialogViewModel(cts);
            dialogServiceVm.ShowProgressDialog(progressVm);

            try
            {
                await asyncAction(ct);
            }
            catch (Exception ex)
            {
                error = true;

                if (sendNotificationOnError)
                {
                    Notification notification = new Notification(NotificationTypeEnum.Error, "Operation failed.", ex);
                    notificationService.AddNotification(notification);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                progressVm.OperationEnded = true;
            }

            if (error) return false;
            return true;
        }
    }
}
