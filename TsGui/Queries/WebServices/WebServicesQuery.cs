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
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Xml.Linq;

using TsGui.Linking;
using TsGui.Authentication;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using System.Text;
using System.IO;
using MessageCrap;

namespace TsGui.Queries.WebServices
{
    public class WebServicesQuery : BaseQuery, IAuthenticatorConsumer
    {
        private string _serviceurl;
        private List<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();
        private string _method;
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
        public string Response { get; set; }

        public WebServicesQuery(XElement InputXml, ILinkTarget owner): base(owner)
        {
            this.LoadXml(InputXml);
            Director.Instance.AuthLibrary.AddAuthenticatorConsumer(this);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.AuthID = XmlHandler.GetStringFromXAttribute(InputXml, "AuthID", this.AuthID);

            this._processingwrangler.Separator = XmlHandler.GetStringFromXElement(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXElement(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);

            this._propertyTemplates = QueryHelpers.GetTemplatesFromXmlElements(InputXml.Elements("Property"));
            this._serviceurl = XmlHandler.GetStringFromXElement(InputXml, "ServiceURL", this._serviceurl);
            this._method = XmlHandler.GetStringFromXElement(InputXml, "Method", this._method);
            foreach (XElement x in InputXml.Elements("Parameter"))
            {
                XAttribute xa = x.Attribute("Name");
                if (xa == null) { throw new TsGuiKnownException("Missing Name attribute in XML: " + x.ToString(), ""); }
                this._parameters.Add(new KeyValuePair<string, string>(xa.Value, x.Value));
            }
        }

        public override ResultWrangler ProcessQuery(Message message)
        {
            //if (this._authenticator?.State != AuthState.Authorised)
            //{
            //    this._returnwrangler = null;
            //    return null;
            //}


            //try
            //{
            if (this._processed == true) { this._processingwrangler = this._processingwrangler.Clone(); }
            //to add code
            this.Response = this.CallWebMethod();
            Debug.WriteLine(this.Response);

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            return this._returnwrangler;
        }

        public void OnAuthenticatorStateChange()
        {
            this.ProcessQuery(null);
            this._linktarget?.OnSourceValueUpdated(null);
        }

        public byte[] GetParametersByteArray(List<KeyValuePair<string, string>> parameterlist)
        {
            string s = "";
            foreach (KeyValuePair<string, string> param in parameterlist)
            {
                if (string.IsNullOrWhiteSpace(s)) { s = param.Key + "=" + param.Value; }
                else { s = s + "&" + param.Key + "=" + param.Value; }
            }
            Debug.WriteLine(s);
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(s);
        }

        private string CallWebMethod()
        {
            byte[] requestData = this.GetParametersByteArray(this._parameters);
            //Debug.WriteLine(requestData);

            string uri = this._serviceurl + "/" + this._method;
            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            httpRequest.Method = "POST";
            httpRequest.KeepAlive = false;
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = requestData.Length;
            //httpRequest.Timeout = 30000;
            HttpWebResponse httpResponse = null;
            string response = string.Empty;
            try
            {
                httpRequest.GetRequestStream().Write(requestData, 0, requestData.Length);
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream baseStream = httpResponse.GetResponseStream();
                StreamReader responseStreamReader = new StreamReader(baseStream);
                response = responseStreamReader.ReadToEnd();
                responseStreamReader.Close();
            }
            catch (WebException e)
            {
                //const string CONST_ERROR_FORMAT = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Exception><{0}Error>{1}<InnerException>{2}</InnerException></{0}Error></Exception>";
                //response = string.Format(CONST_ERROR_FORMAT, this._method, e.ToString(), (e.InnerException != null ? e.InnerException.ToString() : string.Empty));
                throw new TsGuiKnownException("Web error", "Web error");
            }
            return response;
        }
    }
}
