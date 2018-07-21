using Xarivu.Coder.Utilities;
using Xarivu.Coder.ViewModel.Dialog;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Xarivu.Coder.View.Dialog
{
    /// <summary>
    /// Interaction logic for DialogServiceControl.xaml
    /// </summary>
    public partial class DialogServiceControl : UserControl
    {
        IDialogCreator dialogCreator;

        public DialogServiceControl()
        {
            InitializeComponent();

            var vm = ViewUtilities.SetDataContext<DialogServiceViewModel>(this);
            if (vm != null)
            {
                vm.ShowDialogRequested += Vm_ShowDialogRequested;
                vm.ShowProgressDialogRequested += Vm_ShowProgressDialogRequested;
                vm.ShowFileDialogRequested += Vm_ShowFileDialogRequested;
                vm.ShowMessageDialogRequested += Vm_ShowMessageDialogRequested;
                vm.DispatchToUIThreadRequested += Vm_DispatchToUIThreadRequested;
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (DependencyContainer.IsTypeRegistered<IDialogCreator>())
                {
                    this.dialogCreator = DependencyContainer.Get<IDialogCreator>();
                }
            }
        }
        
        void Vm_ShowDialogRequested(string dialogName, object dialogVm)
        {
            if (this.dialogCreator == null)
            {
                throw new Exception($"Could not resolve dependency '{nameof(IDialogCreator)}'.");
            }

            Window dialog = this.dialogCreator.CreateDialog(dialogName);
            if (dialog == null)
            {
                throw new NotSupportedException($"Dialog '{dialog}' creation failed.");
            }

            ShowDialog(dialog, dialogVm);
        }

        void ShowDialog(Window dialog, object dialogVm)
        {
            var mainWin = Application.Current.MainWindow;

            dialog.Owner = mainWin;
            dialog.DataContext = dialogVm;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Show();
            dialog.Closed += Dialog_Closed;

            this.Background = new SolidColorBrush(Color.FromArgb(16, 0, 0, 0));
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }

        void Dialog_Closed(object sender, EventArgs e)
        {
            Window w = sender as Window;
            w.Closed -= Dialog_Closed;

            this.Background = null;
            this.HorizontalAlignment = HorizontalAlignment.Right;
            this.VerticalAlignment = VerticalAlignment.Top;
        }

        void Vm_ShowProgressDialogRequested(ProgressDialogViewModel progressDialogViewModel)
        {
            var progressDialog = new ProgressDialog();
            ShowDialog(progressDialog, progressDialogViewModel);
        }

        void Vm_ShowFileDialogRequested(FileDialogTypeEnum fileDialogType, FileDialogViewModel filedDialogViewModel)
        {
            switch (fileDialogType)
            {
                case FileDialogTypeEnum.Save:
                    ShowSaveDialog(filedDialogViewModel);
                    break;
                case FileDialogTypeEnum.Open:
                    ShowOpenDialog(filedDialogViewModel);
                    break;
                default:
                    throw new NotSupportedException(fileDialogType.ToString());
            }
        }

        void Vm_ShowMessageDialogRequested(MessageDialogTypeEnum messageDialogType, MessageDialogViewModel messageDialogViewModel)
        {
            var caption = messageDialogViewModel.Caption;
            var message = messageDialogViewModel.Message;
            if (messageDialogViewModel.Exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + messageDialogViewModel.Exception.ToString();
            }

            switch (messageDialogType)
            {
                case MessageDialogTypeEnum.InfoMessage:
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case MessageDialogTypeEnum.ErrorMessage:
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case MessageDialogTypeEnum.Confirmation:
                    var result = MessageBox.Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    messageDialogViewModel.ConfirmationResult = result == MessageBoxResult.OK;
                    break;
                default:
                    throw new NotSupportedException(messageDialogType.ToString());
            }
        }

        void Vm_DispatchToUIThreadRequested(Action action)
        {
            Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
        }

        void ShowSaveDialog(FileDialogViewModel fileDialogViewModel)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.Filter = "JSON File|*.json";
            saveFileDialog.OverwritePrompt = true;

            var filePath = fileDialogViewModel.FilePath;

            string basePath = null;
            string fileName = null;
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                basePath = System.IO.Path.GetDirectoryName(filePath);
                fileName = System.IO.Path.GetFileName(filePath);
            }

            saveFileDialog.InitialDirectory = basePath;
            saveFileDialog.FileName = fileName;

            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                filePath = saveFileDialog.FileName;
                fileDialogViewModel.FilePath = filePath;
                fileDialogViewModel.FileDialogResult = FileDialogResultEnum.Ok;
            }
            else
            {
                fileDialogViewModel.FileDialogResult = FileDialogResultEnum.Cancel;
            }
        }

        void ShowOpenDialog(FileDialogViewModel fileDialogViewModel)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "JSON File|*.json";
            openFileDialog.Multiselect = false;

            var filePath = fileDialogViewModel.FilePath;
            string basePath = null;
            string fileName = null;
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                basePath = System.IO.Path.GetDirectoryName(filePath);
                fileName = System.IO.Path.GetFileName(filePath);
            }

            openFileDialog.InitialDirectory = basePath;
            openFileDialog.FileName = fileName;

            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                filePath = openFileDialog.FileName;
                fileDialogViewModel.FilePath = filePath;
                fileDialogViewModel.FileDialogResult = FileDialogResultEnum.Ok;
            }
            else
            {
                fileDialogViewModel.FileDialogResult = FileDialogResultEnum.Cancel;
            }
        }
    }
}
