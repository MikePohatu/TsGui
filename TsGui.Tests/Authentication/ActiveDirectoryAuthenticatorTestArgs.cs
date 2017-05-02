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
using TsGui.Authentication;

namespace TsGui.Tests.Authentication
{
    public class ActiveDirectoryAuthenticatorTestArgs
    {
        public AuthState ExpectedResult { get; set; }
        public string AuthUser { get; set; }
        public string AuthPassword { get; set; }
        public string UserName { get; set; }
        public string Domain { get; set; }
        public List<string> Groups { get; set; }

        public ActiveDirectoryAuthenticatorTestArgs(string authuser, string authpw, string domain, List<string> groups, AuthState expectedresult)
        {
            try { this.ReadConfigFile(); }
            catch { }

            if (authuser != null)  { this.AuthUser = authuser; }
            
            if (authpw != null) { this.AuthPassword = authpw; }
            
            if (domain != null) { this.Domain = domain; }

            this.Groups = groups;
            this.ExpectedResult = expectedresult;
        }

        //attempt to read the config.xml file, and display the right messages if it fails
        private void ReadConfigFile()
        {
            string configfile = AppDomain.CurrentDomain.BaseDirectory + @"testauth.xml";

            XElement x;

            x = XmlHandler.Read(configfile);
            this.AuthPassword = XmlHandler.GetStringFromXElement(x, "Password", this.AuthPassword);
            this.AuthUser = XmlHandler.GetStringFromXElement(x, "User", this.AuthUser);
            this.Domain = XmlHandler.GetStringFromXElement(x, "Domain", this.Domain);
        }
    }
}
