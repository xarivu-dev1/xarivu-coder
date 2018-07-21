using Xarivu.Coder.Model.Tracked;
using System.Threading;

namespace Xarivu.Coder.ViewModel.Dialog
{
    public class ProgressDialogViewModel : NotifyChangeBase
    {
        CancellationTokenSource cancelTokenSource;

        public ProgressDialogViewModel(CancellationTokenSource cts)
        {
            this.cancelTokenSource = cts;

            this.CancelCommand = new DelegateCommand(CancelAction);
        }

        public DelegateCommand CancelCommand
        {
            get;
            private set;
        }

        #region OperationEnded
        bool __OperationEnded;
        public bool OperationEnded
        {
            get
            {
                return this.__OperationEnded;
            }

            set
            {
                if (this.__OperationEnded != value)
                {
                    this.__OperationEnded = value;
                    NotifyPropertyChanged(nameof(OperationEnded));
                }
            }
        }
        #endregion

        #region ProgressMessage
        string __ProgressMessage;
        public string ProgressMessage
        {
            get
            {
                return this.__ProgressMessage;
            }

            set
            {
                if (this.__ProgressMessage != value)
                {
                    this.__ProgressMessage = value;
                    NotifyPropertyChanged(nameof(ProgressMessage));
                }
            }
        }
        #endregion

        void CancelAction(object commandParam)
        {
            if (this.cancelTokenSource != null)
            {
                this.cancelTokenSource.Cancel();
            }
        }
    }
}