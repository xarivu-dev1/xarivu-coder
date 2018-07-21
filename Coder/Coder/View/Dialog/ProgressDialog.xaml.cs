using Xarivu.Coder.ViewModel.Dialog;
using System.Windows;

namespace Xarivu.Coder.View.Dialog
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog()
        {
            InitializeComponent();

            RegisterVMPropertyChangedEventHandlers(null, this.DataContext as ProgressDialogViewModel);

            this.DataContextChanged += ProgressDialog_DataContextChanged;
        }

        private void ProgressDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as ProgressDialogViewModel;
            var newVm = e.NewValue as ProgressDialogViewModel;
            RegisterVMPropertyChangedEventHandlers(oldVm, newVm);
        }

        void RegisterVMPropertyChangedEventHandlers(ProgressDialogViewModel oldVm, ProgressDialogViewModel newVm)
        {
            if (oldVm != null)
            {
                oldVm.PropertyChanged -= VM_PropertyChanged;
            }

            if (newVm != null)
            {
                newVm.PropertyChanged += VM_PropertyChanged;
            }
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ProgressDialogViewModel vm = this.DataContext as ProgressDialogViewModel;

            if (vm != null && e.PropertyName == nameof(ProgressDialogViewModel.OperationEnded))
            {
                if (vm.OperationEnded)
                {
                    this.Close();
                }
            }
        }
    }
}
