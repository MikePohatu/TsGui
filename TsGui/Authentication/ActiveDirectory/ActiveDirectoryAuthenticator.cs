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

using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using TsGui.Diagnostics.Logging;

namespace TsGui.Authentication.ActiveDirectory
{
    public class ActiveDirectoryAuthenticator : IAuthenticator
    {
        public event AuthValueChanged AuthStateChanged;

        private AuthState _state;
        private string _domain;

        public PrincipalContext Context { get; set; }
        public AuthState State { get { return this._state; } }
        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string AuthID { get; set; }
        public List<string> RequiredGroups { get; set; } 

        public ActiveDirectoryAuthenticator(string authid, string domain)
        {
            this.AuthID = authid;
            this._state = AuthState.AccessDenied;
            this._domain = domain;
            this.RequiredGroups = new List<string>();
        }

        public AuthState Authenticate()
        {
            if (string.IsNullOrWhiteSpace(this.UsernameSource.Username) == true)
            {
                LoggerFacade.Warn("Cannot autheticate with empty username");
                this.SetState(AuthState.AccessDenied);
                return AuthState.AccessDenied;
            }
            if (string.IsNullOrEmpty(this.PasswordSource.Password) == true)
            {
                LoggerFacade.Warn("Cannot autheticate with empty password");
                this.SetState(AuthState.AccessDenied);
                return AuthState.AccessDenied;
            }

            LoggerFacade.Info("Authenticating user: " + this.UsernameSource.Username + " against domain " + this._domain);
            AuthState newstate;
            try
            {
                this.Context = new PrincipalContext(ContextType.Domain, this._domain, this.UsernameSource.Username, this.PasswordSource.Password);

                if (ActiveDirectoryMethods.IsUserMemberOfGroups(this.Context, this.UsernameSource.Username, this.RequiredGroups) == true)
                {
                    LoggerFacade.Info("Active Directory authorised");
                    newstate = AuthState.Authorised;
                }
                else
                {
                    LoggerFacade.Info("Active Directory not authorised");
                    newstate = AuthState.NotAuthorised;
                }
            }
            catch (Exception e)
            {
                LoggerFacade.Error("Active Directory authentication access denied");
                LoggerFacade.Trace(e.Message + Environment.NewLine + e.StackTrace);
                newstate = AuthState.AccessDenied;
            }

            this.SetState(newstate);
            return newstate;
        }

        private void SetState(AuthState newstate)
        {
            if (newstate != this._state)
            {
                this._state = newstate;
                this.AuthStateChanged?.Invoke();
            }
        }
    }
}
