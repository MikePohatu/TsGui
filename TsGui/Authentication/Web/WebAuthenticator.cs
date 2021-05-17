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
using TsGui.Diagnostics.Logging;
using TsGui.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;

namespace TsGui.Authentication.ActiveDirectory
{
    public class WebAuthenticator : IAuthenticator
    {
        public event AuthValueChanged AuthStateChanged;

        private string _url = string.Empty;
        private AuthState _state = AuthState.NotAuthorised;

        public AuthState State { get { return this._state; } }
        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string AuthID { get; set; }

        public WebAuthenticator(XElement inputxml)
        {
            this.LoadXml(inputxml);
        }

        public bool IsAsync { get; } = true;
        public async Task AuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(this.UsernameSource.Username) == true)
            {
                LoggerFacade.Warn("Cannot autheticate with empty username");
                this.SetState(AuthState.AccessDenied);
                return;
            }
            if (string.IsNullOrEmpty(this.PasswordSource.Password) == true)
            {
                LoggerFacade.Warn("Cannot autheticate with empty password");
                this.SetState(AuthState.AccessDenied);
                return;
            }

            LoggerFacade.Info("Authenticating user: " + this.UsernameSource.Username + " against web service " + this._url);
            AuthState newstate;
            HttpClient client = null;

            try
            {
                HttpClientHandler authtHandler = new HttpClientHandler();
                authtHandler.Credentials = new NetworkCredential(this.UsernameSource.Username, this.PasswordSource.SecurePassword);

                client = new HttpClient(authtHandler);

                var response = await client.GetAsync(this._url);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    newstate = AuthState.NotAuthorised; 
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    newstate = AuthState.AccessDenied;
                }
                else
                {
                    newstate = AuthState.Authorised; //if all OK credentials must be fine
                }
            }
            catch (Exception e)
            {
                client?.Dispose();
                LoggerFacade.Warn("Access denied");
                LoggerFacade.Trace(e.Message + Environment.NewLine + e.StackTrace);
                newstate = AuthState.AccessDenied;
            }

            this.SetState(newstate);
        }

        public void Authenticate()
        {
            throw new NotImplementedException();
        }

        private void LoadXml(XElement inputxml)
        {
            this.AuthID = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new TsGuiKnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            this._url = XmlHandler.GetStringFromXAttribute(inputxml, "URL", this._url);
            this._url = XmlHandler.GetStringFromXElement(inputxml, "URL", this._url);
            if (string.IsNullOrWhiteSpace(this._url) == true)
            { throw new TsGuiKnownException("Missing URL attribute in XML:", inputxml.ToString()); }
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
