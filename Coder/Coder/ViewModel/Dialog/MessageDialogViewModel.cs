using Xarivu.Coder.Model.Tracked;
using System;

namespace Xarivu.Coder.ViewModel.Dialog
{
    public class MessageDialogViewModel : NotifyChangeBase
    {
        #region Caption
        string __Caption;
        public string Caption
        {
            get
            {
                return this.__Caption;
            }

            set
            {
                if (this.__Caption != value)
                {
                    this.__Caption = value;
                    NotifyPropertyChanged(nameof(Caption));
                }
            }
        }
        #endregion
        
        #region Message
        string __Message;
        public string Message
        {
            get
            {
                return this.__Message;
            }

            set
            {
                if (this.__Message != value)
                {
                    this.__Message = value;
                    NotifyPropertyChanged(nameof(Message));
                }
            }
        }
        #endregion

        /// <summary>
        /// The exception string is appended to message area when message dialog type is Error.
        /// </summary>
        #region Exception
        Exception __Exception;
        public Exception Exception
        {
            get
            {
                return this.__Exception;
            }

            set
            {
                if (this.__Exception != value)
                {
                    this.__Exception = value;
                    NotifyPropertyChanged(nameof(Exception));
                }
            }
        }
        #endregion

        #region ConfirmationResult
        bool __ConfirmationResult;
        public bool ConfirmationResult
        {
            get
            {
                return this.__ConfirmationResult;
            }

            set
            {
                if (this.__ConfirmationResult != value)
                {
                    this.__ConfirmationResult = value;
                    NotifyPropertyChanged(nameof(ConfirmationResult));
                }
            }
        }
        #endregion
    }
}
