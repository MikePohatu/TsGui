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
using System.Net;
using System.Collections.Generic;
using System.Xml.Linq;

using TsGui.Linking;
using TsGui.Authentication;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;

namespace TsGui.Queries.WebServices
{
    public class WebServicesQuery : BaseQuery, IAuthenticatorConsumer
    {
        private IDirector _director;
        private ILinkTarget _linktargetoption;
        private List<KeyValuePair<string, XElement>> _propertyTemplates;
        private IAuthenticator _authenticator;

        public IAuthenticator Authenticator
        {
            get { return this._authenticator; }
            set
            {
                this._authenticator = value;
                this._authenticator.AuthStateChanged += this.OnAuthenticatorStateChange;
            }
        }
        public string AuthID { get; set; }

        public WebServicesQuery(XElement InputXml, IDirector director, ILinkTarget owner)
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

            try
            {
                if (this._processed == true ) { this._processingwrangler = this._processingwrangler.Clone(); }
                //to add code
            }
            catch (Exception e)
            {
                throw new TsGuiKnownException("Web Services query caused an error:" + Environment.NewLine, e.Message);
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
    }
}
