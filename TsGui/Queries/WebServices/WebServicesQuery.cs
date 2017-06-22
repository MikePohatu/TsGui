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

namespace TsGui.Queries.WebServices
{
    public class WebServicesQuery : BaseQuery, IAuthenticatorConsumer
    {
        private string _serviceurl;
        private List<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();
        private string _method;
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
        public string Response { get; set; }

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
            this._serviceurl = XmlHandler.GetStringFromXElement(InputXml, "ServiceURL", this._serviceurl);
            this._method = XmlHandler.GetStringFromXElement(InputXml, "Method", this._method);
            foreach (XElement x in InputXml.Elements("Parameter"))
            {
                XAttribute xa = x.Attribute("Name");
                this._parameters.Add(new KeyValuePair<string, string>(xa.Value,x.Value));
            }
        }

        public override ResultWrangler ProcessQuery()
        {
            if (this._authenticator?.State != AuthState.Authorised)
            {
                this._returnwrangler = null;
                return null;
            }

            string methodparameters = null;
            foreach (KeyValuePair<string, string> param in this._parameters)
            {
                if (string.IsNullOrWhiteSpace(methodparameters)) { methodparameters = param.Key + "=" + param.Value; }
                else { methodparameters = methodparameters + "&" + param.Key + "=" + param.Value; }
            }

            try
            {
                if (this._processed == true ) { this._processingwrangler = this._processingwrangler.Clone(); }
                //to add code

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this._serviceurl + @"\" + this._method);

                req.Method = "POST";
                //req.ContentType = "application/x-www-form-urlencoded";

                byte[] byteArray = Encoding.UTF8.GetBytes(methodparameters);
                req.ContentLength = byteArray.Length;
                Stream dataStream = req.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    dataStream = response.GetResponseStream();
                    StreamReader SR = new StreamReader(dataStream, Encoding.Unicode);
                    this.Response = SR.ReadToEnd();
                    response.Close();
                    dataStream.Close();
                    SR.Close();
                    req.Abort();
                }
                catch { req.Abort(); }


                //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this._serviceurl + @"\" + this._method);

                //request.Method = "POST";
                ////request.Headers
                ////request.ContentType = "application/xml";
                //HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

                //using (WebResponse response = request.GetResponse())
                //{
                //    this.Response = response.ContentType;
                //}
                Debug.WriteLine("Response: " + this.Response);
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
