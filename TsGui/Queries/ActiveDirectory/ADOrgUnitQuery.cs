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

using TsGui.Queries;
using TsGui.Linking;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Authentication;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace TsGui.Queries.ActiveDirectory
{
    public class ADOrgUnitQuery : BaseQuery, IAuthenticatorConsumer
    {
        private IDirector _director;
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

        public ADOrgUnitQuery(XElement InputXml, IDirector director, ILinkTarget owner)
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
            this._baseou = InputXml.Element("BaseOU")?.Value;
            //make sure there is a group to query
            if (string.IsNullOrEmpty(this._baseou)) { throw new TsGuiKnownException("No BaseOU specified in XML: ", InputXml.ToString()); }


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
                string fullbaseou = "LDAP://" + this._authenticator.Domain + "/" + this._baseou;
                using (DirectoryEntry de = new DirectoryEntry(fullbaseou, this._authenticator.UsernameSource.Username,this._authenticator.PasswordSource.Password))
                {
                    this._processingwrangler.AddResult(this.QueryDirectoryEntry(de));
                }
            }
            catch (Exception e)
            {
                //throw new TsGuiKnownException("Active Directory OU query caused an error", e.Message);
                LoggerFacade.Warn("Active Directory OU query caused an error: " + e.Message);
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

        private Result QueryDirectoryEntry(DirectoryEntry baseou)
        {
            Result returnresult = this.CreateBaseResult(baseou, this._propertyTemplates);

            //now query for sub OUs
            using (DirectorySearcher searcher = new DirectorySearcher(baseou, "(objectCategory=organizationalUnit)"))
            {
                searcher.SearchScope = SearchScope.OneLevel;
                foreach (SearchResult searchresult in searcher.FindAll())
                {
                    using (DirectoryEntry subde = searchresult.GetDirectoryEntry())
                    {
                        Result subresult = this.QueryDirectoryEntry(subde);
                        returnresult.SubResults.Add(subresult);
                    }
                }
            }
            return returnresult;
        }

        private Result CreateBaseResult(DirectoryEntry baseou, List<KeyValuePair<string, XElement>> propertytemplates)
        {
            Result r = new Result();
            
            if (propertytemplates.Count != 0)
            {
                foreach (KeyValuePair<string, XElement> template in propertytemplates)
                {
                    FormattedProperty pf = new FormattedProperty(template.Value);
                    if (baseou.Properties[pf.Name] != null && baseou.Properties[pf.Name].Count > 0)
                    {
                        pf.Input = baseou.Properties[pf.Name][0].ToString();
                        r.Add(pf);
                    }
                        
                }
            }
            else
            {
                foreach (PropertyCollection pc in baseou.Properties)
                {
                    if (pc.Count > 0)
                    {
                        foreach (string s in pc.Values)
                        {
                            FormattedProperty pf = new FormattedProperty();
                            pf.Input = s;
                            r.Add(pf);
                        }
                    }                   
                }
            }

            return r;
        }
    }
}
