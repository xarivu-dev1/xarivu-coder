using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Xarivu.Coder.Model;

namespace Xarivu.Coder.Service
{
    /// <summary>
    /// Login service.
    /// Derive from this class and register the derived class with DependencyContainer with the type LoginServiceBase.
    /// </summary>
    public abstract class LoginServiceBase : INotifyPropertyChanged
    {
        NotificationService notificationService;
        string registrySwKeyName;

        public LoginServiceBase(NotificationService notificationService, string registrySwKeyName)
        {
            this.notificationService = notificationService;
            this.registrySwKeyName = registrySwKeyName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region IsSignedIn
        bool __IsSignedIn;
        public bool IsSignedIn
        {
            get
            {
                return this.__IsSignedIn;
            }

            set
            {
                if (this.__IsSignedIn != value)
                {
                    this.__IsSignedIn = value;
                    NotifyPropertyChanged(nameof(IsSignedIn));
                }
            }
        }
        #endregion

        public LoginInfo LoginInfo
        {
            get;
            private set;
        }

        public NetworkCredential LoadFromRegistry()
        {
            try
            {
                using (RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software"))
                {
                    using (var regKey = softwareKey.OpenSubKey(this.registrySwKeyName))
                    {
                        if (regKey != null)
                        {
                            var username = regKey.GetValue("username") as string;
                            var password = regKey.GetValue("password") as string;

                            // TODO: Decrypt password
                            return new NetworkCredential(username, password);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.notificationService.AddNotification(new Notification(NotificationTypeEnum.Error, "Loading from registry failed.", ex));
            }

            return null;
        }

        public async Task AutoSignIn()
        {
            var netCred = LoadFromRegistry();
            if (netCred != null)
            {
                await SignIn(netCred, false, false);
            }
        }

        /// <summary>
        /// Sign in and optionally update registry.
        /// If the update registry flag is true then
        ///     If saveToRegistry is try then the username and password are written to registry.
        ///     Othewise, if the username and password exist, they wull be deleted from registry.
        /// </summary>
        /// <param name="netCred"></param>
        /// <param name="updateRegistry"></param>
        /// <param name="savetoRegistry"></param>
        public async Task SignIn(NetworkCredential netCred, bool updateRegistry, bool savetoRegistry)
        {
            try
            {
                if (this.IsSignedIn)
                {
                    return;
                }

                this.LoginInfo = await LoginInternal(netCred);

                this.IsSignedIn = true;

                if (updateRegistry)
                {
                    if (savetoRegistry)
                    {
                        SaveToRegistry(netCred);
                    }
                    else
                    {
                        DeleteFromRegistry(netCred);
                    }
                }

                this.notificationService.AddNotification(new Notification(NotificationTypeEnum.Information, "Sign in successful."));
            }
            catch (Exception ex)
            {
                this.notificationService.AddNotification(new Notification(NotificationTypeEnum.Error, "Sign in failed.", ex));
            }
        }

        public async Task SignOut()
        {
            try
            {
                if (!this.IsSignedIn || this.LoginInfo == null)
                {
                    return;
                }

                await LogoutInternal(this.LoginInfo);

                this.LoginInfo = null;

                this.IsSignedIn = false;

                this.notificationService.AddNotification(new Notification(NotificationTypeEnum.Information, "Sign out successful."));
            }
            catch (Exception ex)
            {
                this.notificationService.AddNotification(new Notification(NotificationTypeEnum.Error, "Sign out failed.", ex));
            }
        }

        void SaveToRegistry(NetworkCredential netCred)
        {
            using (RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software", true))
            {
                using (var regKey = softwareKey.CreateSubKey(this.registrySwKeyName))
                {
                    regKey.SetValue("username", netCred.UserName);

                    // TODO: Encrypt password
                    regKey.SetValue("password", netCred.Password);
                }
            }
        }

        void DeleteFromRegistry(NetworkCredential netCred)
        {
            using (RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software", true))
            {
                using (var regKey = softwareKey.OpenSubKey(this.registrySwKeyName, true))
                {
                    if (regKey != null)
                    {
                        var username = regKey.GetValue("username");
                        if (username != null)
                        {
                            regKey.DeleteValue("username");
                        }

                        var password = regKey.GetValue("password");
                        if (password != null)
                        {
                            regKey.DeleteValue("password");
                        }
                    }
                }
            }
        }

        void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected abstract Task<LoginInfo> LoginInternal(NetworkCredential credentails);
        protected abstract Task<bool> LogoutInternal(LoginInfo loginInfo);
    }
}
