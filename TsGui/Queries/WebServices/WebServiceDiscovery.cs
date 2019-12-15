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
    public static class WebServiceDiscovery
    {
        public static string GetWSDLForWebMethod(string serviceurl)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(serviceurl + "?WSDL");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            System.IO.Stream baseStream = response.GetResponseStream();
            System.IO.StreamReader responseStreamReader = new System.IO.StreamReader(baseStream);
            string wsdl = responseStreamReader.ReadToEnd();
            responseStreamReader.Close();
            return ExtractTypesXmlFragment(wsdl);
        }

        private static string ExtractTypesXmlFragment(string wsdl)
        {
            const string CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_HTTP = "http:";
            const string CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_SOAP = "soap:";
            const string CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_SOAPENC = "soapenc:";
            const string CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_TM = "tm:";
            const string CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_S = "s:";
            const string CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_MIME = "mime:";
            const string CONST_TYPES_REGULAR_EXPRESSION = "<types>[\\s\\n\\r=\"<>a-zA-Z0-9.\\.:/\\w\\d%]+</types>";
            System.Collections.ArrayList namespaceDeclarationsToRemove = new System.Collections.ArrayList();
            namespaceDeclarationsToRemove.Add(CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_HTTP);
            namespaceDeclarationsToRemove.Add(CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_MIME);
            namespaceDeclarationsToRemove.Add(CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_S);
            namespaceDeclarationsToRemove.Add(CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_SOAP);
            namespaceDeclarationsToRemove.Add(CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_SOAPENC);
            namespaceDeclarationsToRemove.Add(CONST_XML_NAMESPACE_REFERENCE_TO_REMOVE_TM);
            for (int i = 0; i < namespaceDeclarationsToRemove.Count; i++)
            {
                wsdl = wsdl.Replace((string)namespaceDeclarationsToRemove[i], string.Empty);
            }
            System.Text.RegularExpressions.Match match =
            System.Text.RegularExpressions.Regex.Match(wsdl, CONST_TYPES_REGULAR_EXPRESSION);
            return match.Groups[0].Value;
        }
    }
}
