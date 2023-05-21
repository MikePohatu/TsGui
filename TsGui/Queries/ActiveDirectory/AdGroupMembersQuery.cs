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
    public class ADGroupMembersQuery : BaseQuery, IAuthenticatorConsumer
    {
        private List<KeyValuePair<string, XElement>> _propertyTemplates;
        private string _groupname;
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

        public ADGroupMembersQuery(XElement InputXml, ILinkTarget owner): base(owner)
        {
            this.LoadXml(InputXml);
            AuthLibrary.AddAuthenticatorConsumer(this);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.AuthID = XmlHandler.GetStringFromXml(InputXml, "AuthID", this.AuthID);
            this._groupname = InputXml.Element("GroupName")?.Value;
            //make sure there is a group to query
            if (string.IsNullOrEmpty(this._groupname)) { throw new KnownException("No group specified in XML: ", InputXml.ToString()); }


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

            try
            {
                if (this._processed == true ) { this._processingwrangler = this._processingwrangler.Clone(); }
                using (GroupPrincipal group = GroupPrincipal.FindByIdentity(this._authenticator.Context, this._groupname))
                {
                    if (group == null) { Log.Warn("Group not found: " + this._groupname); }
                    else
                    { this.AddPropertiesToWrangler(this._processingwrangler, group.Members, this._propertyTemplates); }
                }
            }
            catch (Exception e)
            {
                throw new KnownException("Active Directory group query caused an error:" + Environment.NewLine + this._groupname, e.Message);
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
            this._linktarget?.OnSourceValueUpdatedAsync(null);
        }

        private void AddPropertiesToWrangler(ResultWrangler wrangler, PrincipalCollection objectlist, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            foreach (UserPrincipal user in objectlist)
            {
                wrangler.NewResult();
                FormattedProperty prop = null;

                //if properties have been specified in the xml, query them directly in order
                if (PropertyTemplates.Count != 0)
                {
                    foreach (KeyValuePair<string, XElement> template in PropertyTemplates)
                    {
                        prop = new FormattedProperty(template.Value);
                        prop.Input = PropertyInterogation.GetStringFromPropertyValue(user, template.Key);
                        wrangler.AddFormattedProperty(prop);
                    }
                }
            }
        }
    }
}
