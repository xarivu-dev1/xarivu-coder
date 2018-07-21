using Xarivu.Coder.Model.Tracked;

namespace Xarivu.Coder.ViewModel.Dialog
{
    public class FileDialogViewModel : NotifyChangeBase
    {
        #region FileDialogResult
        FileDialogResultEnum? __FileDialogResult;
        public FileDialogResultEnum? FileDialogResult
        {
            get
            {
                return this.__FileDialogResult;
            }

            set
            {
                if (this.__FileDialogResult != value)
                {
                    this.__FileDialogResult = value;
                    NotifyPropertyChanged(nameof(FileDialogResult));
                }
            }
        }
        #endregion

        #region FilePath
        string __FilePath;
        public string FilePath
        {
            get
            {
                return this.__FilePath;
            }

            set
            {
                if (this.__FilePath != value)
                {
                    this.__FilePath = value;
                    NotifyPropertyChanged(nameof(FilePath));
                }
            }
        }
        #endregion
    }
}
