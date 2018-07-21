using System;

namespace Xarivu.Coder.Model
{
    public class LoginInfo
    {
        public LoginInfo(string userName, string token, DateTime loginTime)
        {
            this.IsSuccessful = true;
            this.UserName = userName;
            this.Token = token;
            this.LoginTime = loginTime;
        }

        public LoginInfo(string userName, DateTime loginTime, string errorMessage, Exception loginException)
        {
            this.IsSuccessful = false;
            this.UserName = userName;
            this.LoginTime = loginTime;
            this.Error = errorMessage;
            this.LoginException = loginException;
        }

        public bool IsSuccessful { get; private set; }
        public string UserName { get; private set; }
        public string Token { get; private set; }
        public DateTime LoginTime { get; private set; }
        public string Error { get; private set; }
        public Exception LoginException { get; private set; }
    }
}
