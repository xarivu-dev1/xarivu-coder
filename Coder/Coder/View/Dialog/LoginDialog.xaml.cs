using Xarivu.Coder.ViewModel.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Xarivu.Coder.View.Dialog
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public LoginDialog()
        {
            InitializeComponent();

            RegisterVMPropertyChangedEventHandlers(null, this.DataContext as LoginViewModel);

            this.DataContextChanged += SignInDialog_DataContextChanged;
        }

        private void SignInDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as LoginViewModel;
            var newVm = e.NewValue as LoginViewModel;
            RegisterVMPropertyChangedEventHandlers(oldVm, newVm);
        }

        void RegisterVMPropertyChangedEventHandlers(LoginViewModel oldVm, LoginViewModel newVm)
        {
            if (oldVm != null)
            {
                oldVm.PropertyChanged -= VM_PropertyChanged;
                oldVm.CloseRequested -= Vm_CloseRequested;
            }

            if (newVm != null)
            {
                newVm.PropertyChanged += VM_PropertyChanged;
                newVm.CloseRequested += Vm_CloseRequested;
            }
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LoginViewModel vm = this.DataContext as LoginViewModel;

            if (vm != null && e.PropertyName == nameof(LoginViewModel.Password))
            {
                var vmNetCred = new NetworkCredential(string.Empty, vm.Password);
                var uiNetCred = new NetworkCredential(string.Empty, this.PasswordBoxObj.SecurePassword);
                if (vmNetCred.Password != uiNetCred.Password)
                {
                    this.PasswordBoxObj.Password = vmNetCred.Password;
                }
            }
        }

        private void Vm_CloseRequested()
        {
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginViewModel vm = this.DataContext as LoginViewModel;
            if (vm == null) return;

            PasswordBox pb = (PasswordBox)sender;
            vm.Password = pb.SecurePassword;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            LoginViewModel vm = this.DataContext as LoginViewModel;
            if (vm == null) return;

            if (vm.OkCommand.CanExecute(null))
            {
                var commandParam = new DialogCommandParameter();
                vm.OkCommand.Execute(commandParam);
                if (commandParam.Close)
                {
                    this.Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoginViewModel vm = this.DataContext as LoginViewModel;
            if (vm == null) return;

            if (vm.CancelCommand.CanExecute(null))
            {
                var commandParam = new DialogCommandParameter();
                vm.CancelCommand.Execute(commandParam);
                if (commandParam.Close)
                {
                    this.Close();
                }
            }
        }
    }
}
