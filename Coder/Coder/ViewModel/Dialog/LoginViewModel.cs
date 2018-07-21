using Xarivu.Coder.Model.Tracked;
using Xarivu.Coder.Service;
using Xarivu.Coder.ViewModel;
using Xarivu.Coder.ViewModel.Dialog;
using System;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Xarivu.Coder.ViewModel.Dialog
{
    public class LoginViewModel : NotifyChangeBase
    {
        LoginServiceBase loginService;
        DialogServiceViewModel dialogServiceViewModel;

        public LoginViewModel(LoginServiceBase loginService, DialogServiceViewModel dialogServiceViewModel)
        {
            this.loginService = loginService;
            this.dialogServiceViewModel = dialogServiceViewModel;

            var netCred = this.loginService.LoadFromRegistry();
            if (netCred != null && !string.IsNullOrWhiteSpace(netCred.UserName) && !string.IsNullOrWhiteSpace(netCred.Password))
            {
                this.Username = netCred.UserName;

                // Don't load the password back into the dialog.
                //this.Password = netCred.SecurePassword;

                this.RememberCredentials = true;
            }

            this.OkCommand = new DelegateCommand(async p => await OkAction(p));
            this.CancelCommand = new DelegateCommand(p => CancelAction(p));
        }

        public event Action CloseRequested;

        public DelegateCommand OkCommand
        {
            get;
            private set;
        }

        public DelegateCommand CancelCommand
        {
            get;
            private set;
        }

        #region Username
        string __Username;
        public string Username
        {
            get
            {
                return this.__Username;
            }

            set
            {
                if (this.__Username != value)
                {
                    this.__Username = value;
                    NotifyPropertyChanged(nameof(Username));
                }
            }
        }
        #endregion

        #region Password
        SecureString __Password;
        public SecureString Password
        {
            get
            {
                return this.__Password;
            }

            set
            {
                if (this.__Password != value)
                {
                    this.__Password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }
        #endregion

        #region RememberCredentials
        bool __RememberCredentials;
        public bool RememberCredentials
        {
            get
            {
                return this.__RememberCredentials;
            }

            set
            {
                if (this.__RememberCredentials != value)
                {
                    this.__RememberCredentials = value;
                    NotifyPropertyChanged(nameof(RememberCredentials));
                }
            }
        }
        #endregion


        #region ErrorMessage
        string __ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                return this.__ErrorMessage;
            }

            set
            {
                if (this.__ErrorMessage != value)
                {
                    this.__ErrorMessage = value;
                    NotifyPropertyChanged(nameof(ErrorMessage));
                }
            }
        }
        #endregion


        async Task OkAction(object commandParam)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            var progressVm = new ProgressDialogViewModel(cts);
            this.dialogServiceViewModel.ShowProgressDialog(progressVm);

            try
            {
                await this.loginService.SignIn(new NetworkCredential(this.Username, this.Password), true, this.RememberCredentials);
            }
            finally
            {
                progressVm.OperationEnded = true;
            }

            if (this.loginService.IsSignedIn)
            {
                // Since execution is async and caller does not await, need to send close request.
                // Cannot returnn dialog command.
                //var dialogCommandParam = commandParam as DialogCommandParameter;
                //if (dialogCommandParam != null)
                //{
                //    dialogCommandParam.Close = true;
                //}

                this.CloseRequested?.Invoke();
            }
            else
            {
                this.ErrorMessage = "Sign in failed.";
            }
        }

        void CancelAction(object commandParam)
        {
            var dialogCommandParam = commandParam as DialogCommandParameter;
            if (dialogCommandParam != null)
            {
                dialogCommandParam.Close = true;
            }
        }
    }
}
