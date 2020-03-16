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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;

namespace TsGui.Authentication.LocalConfig
{
    public class LocalConfigPasswordAuthenticator: IAuthenticator
    {
        public event AuthValueChanged AuthStateChanged;

        private List<Password> _validpasswords = new List<Password>();

        public AuthState State { get; private set; }
        public IUsername UsernameSource { get; set; }
        public IPassword PasswordSource { get; set; }
        public string AuthID { get; set; }
        public List<string> RequiredGroups { get; private set; } = new List<string>();

        public LocalConfigPasswordAuthenticator(XElement inputxml)
        {
            this.LoadXml(inputxml);
            this.State = AuthState.AccessDenied;
        }

        public AuthState Authenticate()
        {
            if (string.IsNullOrEmpty(this.PasswordSource.Password) == true)
            {
                LoggerFacade.Warn("Cannot autheticate with empty password");
                this.SetState(AuthState.AccessDenied);
                return AuthState.AccessDenied;
            }

            LoggerFacade.Info("Authenticating against local config with ID " + this.AuthID);
            AuthState newstate = AuthState.AuthError;

            foreach (Password pw in this._validpasswords)
            {
                if (pw.PasswordMatches(this.PasswordSource?.Password))
                {
                    newstate = AuthState.Authorised;
                    LoggerFacade.Info("Authorised.");
                    break;
                }
            }

            if (newstate != AuthState.Authorised)
            {
                LoggerFacade.Warn("Authentication failed");
            }


            this.SetState(newstate);
            return newstate;
        }

        private void LoadXml(XElement inputxml)
        {
            this.AuthID = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new TsGuiKnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            foreach (XElement x in inputxml.Elements("Password"))
            {
                this._validpasswords.Add(new Password(x));
            }
        }

        private void SetState(AuthState newstate)
        {
            if (newstate != this.State)
            {
                this.State = newstate;
                this.AuthStateChanged?.Invoke();
            }
        }
    }
}
