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
using System.Xml.Linq;

using TsGui.Linking;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Authentication;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using System.DirectoryServices.AccountManagement;

namespace TsGui.Queries.ActiveDirectory
{
    public class ADGroupMembersQuery : BaseQuery, IAuthenticatorConsumer
    {
        private IDirector _director;
        private ILinkTarget _linktargetoption;
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

        public ADGroupMembersQuery(XElement InputXml, IDirector director, ILinkTarget owner)
        {
            this._linktargetoption = owner;
            this._director = director;
            this.LoadXml(InputXml);
            this._director.AuthLibrary.AddAuthenticatorConsumer(this);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.AuthID = XmlHandler.GetStringFromXAttribute(InputXml, "AuthID", this.AuthID);
            this._groupname = InputXml.Element("GroupName")?.Value;
            //make sure there is a group to query
            if (string.IsNullOrEmpty(this._groupname)) { throw new TsGuiKnownException("No group specified in XML: ", InputXml.ToString()); }


            this._processingwrangler.Separator = XmlHandler.GetStringFromXElement(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXElement(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);

            this._propertyTemplates = QueryHelpers.GetTemplatesFromXmlElements(InputXml.Elements("Property"));
        }

        public override ResultWrangler ProcessQuery()
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
                using (GroupPrincipal group = GroupPrincipal.FindByIdentity(this._authenticator.Context, this._groupname))
                {
                    if (group == null) { LoggerFacade.Warn("Group not found: " + this._groupname); }
                    else
                    { this.AddPropertiesToWrangler(this._processingwrangler, group.Members, this._propertyTemplates); }
                }
            }
            catch (Exception e)
            {
                throw new TsGuiKnownException("Active Directory group query caused an error:" + Environment.NewLine + this._groupname, e.Message);
            }

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            return this._returnwrangler;
        }

        public void OnAuthenticatorStateChange()
        {
            this.ProcessQuery();
            this._linktargetoption?.RefreshAll();
        }

        private void AddPropertiesToWrangler(ResultWrangler wrangler, PrincipalCollection objectlist, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            foreach (UserPrincipal user in objectlist)
            {
                wrangler.NewResult();
                PropertyFormatter rf = null;

                //if properties have been specified in the xml, query them directly in order
                if (PropertyTemplates.Count != 0)
                {
                    foreach (KeyValuePair<string, XElement> template in PropertyTemplates)
                    {
                        rf = new PropertyFormatter(template.Value);
                        rf.Input = PropertyInterogation.GetStringFromPropertyValue(user, template.Key);
                        wrangler.AddPropertyFormatter(rf);
                    }
                }
            }
        }
    }
}
