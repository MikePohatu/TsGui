using System;
using System.Collections.Generic;
using System.Security;
using System.Net;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Authentication;

namespace TsGui.Tests.Authentication
{
    public class ActiveDirectoryAuthenticatorTestSource: IUsername, IPassword
    {
        private NetworkCredential _netcredential = new NetworkCredential();

        public string AuthID { get; set; }
        public string Username
        {
            get { return this._netcredential.UserName; }
            set { this._netcredential.UserName = value; }
        }
        public SecureString SecurePassword
        {
            get { return this._netcredential.SecurePassword; }
            set { this._netcredential.SecurePassword = value; }
        }
        public string Password { get { return this._netcredential.Password; } }
    }
}
