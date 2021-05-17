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
using MessageCrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Authentication;
using TsGui.Connectors.System;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using TsGui.Linking;
using TsGui.Queries;

namespace TsGui.Queries.WebServices
{
    public class CmAdminServiceQuery: BaseQuery, IAuthenticatorConsumer
    {
        private string _url = string.Empty;
        private HttpClient _client = new HttpClient();
        public string AuthID { get; set; }

        private IAuthenticator _authenticator;
        public IAuthenticator Authenticator
        {
            get { return this._authenticator; }
            set
            {
                this._authenticator = value;
            }
        }

        public CmAdminServiceQuery(ILinkTarget owner) : base(owner) { }

        public CmAdminServiceQuery(XElement InputXml, ILinkTarget owner) : base(owner)
        {
            this.LoadXml(InputXml);
            Director.Instance.AuthLibrary.AddAuthenticatorConsumer(this);
        } 

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.AuthID = XmlHandler.GetStringFromXAttribute(InputXml, "AuthID", this.AuthID);
            this._url = XmlHandler.GetStringFromXElement(InputXml, "URL", this._url);

            this._processingwrangler.Separator = XmlHandler.GetStringFromXElement(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXElement(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);

        }

        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            //Query the admin service value
            HttpClient client = null;

            try
            {
                if (this._processed == true) { this._processingwrangler = this._processingwrangler.Clone(); }

                // if in test mode, we might need to prompt for auth if there isn't an attached authenticator
                if (this._authenticator == null && EnvironmentController.TestMode == true)
                {

                }
                else if (this._authenticator != null)
                {
                    if (this._authenticator.State == AuthState.Authorised)
                    {
                        //do stuff if we're authorised. 
                        HttpClientHandler authtHandler = new HttpClientHandler();
                        authtHandler.Credentials = new NetworkCredential(this.Authenticator.UsernameSource.Username, this.Authenticator.PasswordSource.Password);

                        client = new HttpClient(authtHandler);
                        var response = await client.GetAsync(this._url);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            
                        }
                        else
                        {
                            LoggerFacade.Warn("Error querying ConfigMgr admin service: " + response.ReasonPhrase);
                        }
                    }
                    else
                    {
                        LoggerFacade.Warn("Admin Service authenticator not authorised");
                    }
                }
                else
                {
                    throw new TsGuiKnownException("Admin Service query requires an authenticator", "");
                }
                
            }
            catch (ManagementException e)
            {
                client?.Dispose();
                throw new TsGuiKnownException("ConfigMgr admin services query caused an error:" + Environment.NewLine, e.Message);
            }

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            return this._returnwrangler;
        }

        public async Task OnAuthenticatorStateChangeAsync()
        {
            await this.ProcessQueryAsync(null);
            await this._linktarget?.OnSourceValueUpdatedAsync(null);
        }
    }
}
