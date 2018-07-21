using Xarivu.Coder.Model.Tracked;
using System;

namespace Xarivu.Coder.ViewModel.Dialog
{
    public class DialogServiceViewModel : NotifyChangeBase
    {
        public event Action<string, object> ShowDialogRequested;
        public event Action<ProgressDialogViewModel> ShowProgressDialogRequested;
        public event Action<FileDialogTypeEnum, FileDialogViewModel> ShowFileDialogRequested;
        public event Action<MessageDialogTypeEnum, MessageDialogViewModel> ShowMessageDialogRequested;
        public event Action<Action> DispatchToUIThreadRequested;

        public void ShowDialog(string dialogType, object vm)
        {
            if (this.ShowDialogRequested != null)
            {
                this.ShowDialogRequested(dialogType, vm);
            }
        }

        public void ShowProgressDialog(ProgressDialogViewModel vm)
        {
            if (this.ShowProgressDialogRequested != null)
            {
                this.ShowProgressDialogRequested(vm);
            }
        }

        public void ShowFileDialog(FileDialogTypeEnum fileDialogType, FileDialogViewModel vm)
        {
            this.ShowFileDialogRequested?.Invoke(fileDialogType, vm);
        }

        public void ShowMessageDialog(MessageDialogTypeEnum messageDialogType, MessageDialogViewModel vm)
        {
            this.ShowMessageDialogRequested?.Invoke(messageDialogType, vm);
        }

        public void DispatchToUIThread(Action action)
        {
            this.DispatchToUIThreadRequested?.Invoke(action);
        }
    }
}
