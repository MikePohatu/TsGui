//    Copyright (C) 2017 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.


using System.Security;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Net;
using TsGui.Diagnostics.Logging;

namespace TsGui.Authentication.ActiveDirectory
{
    public class ActiveDirectoryAuthenticator : IAuthenticator
    {
        private NetworkCredential _netcredential;
        private AuthState _state;
        private AuthenticationBroker _broker;

        public PrincipalContext Context { get; set; }
        public AuthState State { get { return this._state; } }
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
        public List<string> RequiredGroups { get; set; } 

        public ActiveDirectoryAuthenticator(string domain)
        {
            this._state = AuthState.AccessDenied;
            this._netcredential = new NetworkCredential();
            this._netcredential.Domain = domain;
            this.RequiredGroups = new List<string>();
        }

        public AuthState Authenticate()
        {
            LoggerFacade.Info("Authenticating user:" + this._netcredential.UserName + " against domain " + this._netcredential.Domain);
            try
            {
                this.Context = new PrincipalContext(ContextType.Domain, this._netcredential.Domain, this._netcredential.UserName, this._netcredential.Password);
                if (ActiveDirectoryMethods.IsUserMemberOfGroups(this.Context,this._netcredential.UserName,this.RequiredGroups) == true)
                {
                    LoggerFacade.Info("Active Directory authorised");
                    return AuthState.Authorised;
                }
                else
                {
                    LoggerFacade.Info("Active Directory not authorised");
                    return AuthState.NotAuthorised;
                }
                
            }
            catch
            {
                LoggerFacade.Info("Active Directory access denied");
                return AuthState.AccessDenied;
            }

        }

        //private bool IsValidLogin()
        //{
        //    bool authed = false;
        //    try
        //    {
        //        DirectoryEntry entry = new DirectoryEntry("LDAP://" + this.Domain, this._netcredential.UserName, this._netcredential.Password);
        //        object nativeObject = entry.NativeObject;
        //        authed = true;
        //    }
        //    catch (DirectoryServicesCOMException) { }

        //    return authed;
        //}
    }
}
