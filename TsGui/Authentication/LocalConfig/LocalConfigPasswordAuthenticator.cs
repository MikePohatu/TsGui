#region license
// Copyright (c) 2025 Mike Pohatu
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using Core.Logging;

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
            this.State = AuthState.NotAuthed;
        }

        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            if (string.IsNullOrEmpty(this.PasswordSource.Password) == true)
            {
                Log.Warn("Cannot autheticate with empty password");
                this.SetState(AuthState.NoPassword);
                this.AuthStateChanged?.Invoke();
                return new AuthenticationResult(AuthState.AccessDenied);
            }

            Log.Info("Authenticating against local config with ID " + this.AuthID);
            AuthState newstate = AuthState.AccessDenied;

            foreach (Password pw in this._validpasswords)
            {
                if (pw.PasswordMatches(this.PasswordSource?.Password))
                {
                    newstate = AuthState.Authorised;
                    Log.Info("Authorised.");
                    break;
                }
            }

            if (newstate != AuthState.Authorised)
            {
                Log.Warn("Authentication failed");
            }


            this.SetState(newstate);
            this.AuthStateChanged?.Invoke();

            await Task.CompletedTask;

            return new AuthenticationResult(newstate);
        }

        private void LoadXml(XElement inputxml)
        {
            this.AuthID = XmlHandler.GetStringFromXml(inputxml, "AuthID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new KnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

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
