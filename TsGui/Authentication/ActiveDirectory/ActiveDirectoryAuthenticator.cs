#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using Core.Logging;
using Core.Diagnostics;

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
        public string Domain { get { return this._domain; } }
        public string AuthID { get; set; }
        public List<string> RequiredGroups { get; set; } 

        public ActiveDirectoryAuthenticator(XElement inputxml)
        {
            this.LoadXml(inputxml);
            this._state = AuthState.AccessDenied;
            this.RequiredGroups = new List<string>();
        }

        public AuthState Authenticate()
        {
            if (string.IsNullOrWhiteSpace(this.UsernameSource.Username) == true)
            {
                Log.Warn("Cannot autheticate with empty username");
                this.SetState(AuthState.AccessDenied);
                return AuthState.AccessDenied;
            }
            if (string.IsNullOrEmpty(this.PasswordSource.Password) == true)
            {
                Log.Warn("Cannot autheticate with empty password");
                this.SetState(AuthState.AccessDenied);
                return AuthState.AccessDenied;
            }

            Log.Info("Authenticating user: " + this.UsernameSource.Username + " against domain " + this._domain);
            AuthState newstate;
            try
            {
                this.Context = new PrincipalContext(ContextType.Domain, this._domain, this.UsernameSource.Username, this.PasswordSource.Password);

                if (ActiveDirectoryMethods.IsUserMemberOfGroups(this.Context, this.UsernameSource.Username, this.RequiredGroups) == true)
                {
                    Log.Info("Active Directory authorised");
                    newstate = AuthState.Authorised;
                }
                else
                {
                    Log.Info("Active Directory not authorised");
                    newstate = AuthState.NotAuthorised;
                }
            }
            catch (Exception e)
            {
                Log.Warn("Active Directory access denied");
                Log.Trace(e.Message + Environment.NewLine + e.StackTrace);
                newstate = AuthState.AccessDenied;
            }

            this.SetState(newstate);
            return newstate;
        }

        private void LoadXml(XElement inputxml)
        {
            this.AuthID = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new KnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            this._domain = XmlHandler.GetStringFromXAttribute(inputxml, "Domain", null);
            if (string.IsNullOrWhiteSpace(this._domain) == true)
            { throw new KnownException("Missing Domain attribute in XML:", inputxml.ToString()); }
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
