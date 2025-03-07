﻿#region license
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
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Authentication;
using Core.Diagnostics;
using Core.Logging;
using System.DirectoryServices.AccountManagement;
using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.Queries.ActiveDirectory
{
    public class ADOrgUnitGroupsQuery : BaseQuery, IAuthenticatorConsumer
    {
        private ILinkTarget _linktargetoption;
        private List<KeyValuePair<string, XElement>> _propertyTemplates;
        private string _baseou;
        private ActiveDirectoryAuthenticator _authenticator;

        public IAuthenticator Authenticator
        {
            get { return this._authenticator; }
            set
            {
                this._authenticator = value as ActiveDirectoryAuthenticator;
                this._authenticator.AuthStateChanged += this.OnAuthenticatorStateChange;
            }
        }
        public string AuthID { get; set; }

        public ADOrgUnitGroupsQuery(XElement InputXml, ILinkTarget owner): base(owner)
        {
            this._linktargetoption = owner;
            this.LoadXml(InputXml);
            AuthLibrary.AddAuthenticatorConsumer(this);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.AuthID = XmlHandler.GetStringFromXml(InputXml, "AuthID", this.AuthID);
            this._baseou = InputXml.Element("BaseOU")?.Value;
            //make sure there is a group to query
            if (string.IsNullOrEmpty(this._baseou)) { throw new KnownException("No BaseOU specified in XML: ", InputXml.ToString()); }


            this._processingwrangler.Separator = XmlHandler.GetStringFromXml(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXml(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);

            this._propertyTemplates = QueryHelpers.GetTemplatesFromXmlElements(InputXml.Elements("Property"));
        }

        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            if (this._authenticator?.State != AuthState.Authorised)
            {
                this._returnwrangler = null;
                return null;
            }

            //Now go through the management objects return from WMI, and add the relevant values to the wrangler. 
            //New sublists are created for each management object in the wrangler. 
            try
            {
                if (this._processed == true ) { this._processingwrangler = this._processingwrangler.Clone(); }

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, this._authenticator.Domain, this._baseou, this._authenticator.UsernameSource.Username, this._authenticator.PasswordSource.Password))
                {
                    GroupPrincipal groups = new GroupPrincipal(context, "*");
                    using (PrincipalSearcher ps = new PrincipalSearcher(groups))
                    {
                        PrincipalSearchResult<Principal> groupresults = ps.FindAll();
                        foreach (Principal group in groupresults)
                        {
                            this.AddPropertiesToWrangler(this._processingwrangler, group, this._propertyTemplates);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //throw new KnownException("Active Directory OU query caused an error", e.Message);
                Log.Warn("Active Directory OU groups query caused an error: " + e.Message);
            }

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            await Task.CompletedTask;

            return this._returnwrangler;
        }

        public async void OnAuthenticatorStateChange()
        {
            await this.ProcessQueryAsync(null);
            await this._linktargetoption?.OnSourceValueUpdatedAsync(null);
        }

        private void AddPropertiesToWrangler(ResultWrangler wrangler, Principal group, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            wrangler.NewResult();

            //if properties have been specified in the xml, query them directly in order
            if (PropertyTemplates.Count != 0)
            {
                foreach (KeyValuePair<string, XElement> template in PropertyTemplates)
                {
                    FormattedProperty prop = new FormattedProperty(template.Value);
                    prop.Input = PropertyInterogation.GetStringFromPropertyValue(group, template.Key);
                    wrangler.AddFormattedProperty(prop);
                }
            }
        }
    }
}
