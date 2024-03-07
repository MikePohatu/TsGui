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
using TsGui.Authentication;

namespace TsGui.Tests.Authentication
{
    public class ActiveDirectoryAuthenticatorTestArgs
    {
        public Dictionary<string, bool> ExpectedMemberships { get; set; }
        public AuthState ExpectedState { get; set; }

        public string AuthUser { get; set; }
        public string AuthPassword { get; set; }
        public string UserName { get; set; }
        public string Domain { get; set; }
        public List<string> Groups { get; set; }

        public ActiveDirectoryAuthenticatorTestArgs(List<string> groups, AuthState expectedstate, Dictionary<string, bool> expectedmemberships)
        {
            try { this.ReadConfigFile(); }
            catch { }

            this.Groups = groups;
            this.ExpectedMemberships = expectedmemberships;
            this.ExpectedState = expectedstate;
        }

        public ActiveDirectoryAuthenticatorTestArgs(string badpassword, List<string> groups, AuthState expectedstate, Dictionary<string, bool> expectedmemberships)
        {
            try { this.ReadConfigFile(); }
            catch { }
            this.AuthPassword = badpassword;

            this.Groups = groups;
            this.ExpectedMemberships = expectedmemberships;
            this.ExpectedState = expectedstate;
        }

        //attempt to read the config.xml file, and display the right messages if it fails
        private void ReadConfigFile()
        {
            string configfile = AppDomain.CurrentDomain.BaseDirectory + @"testauth.xml";

            XElement x;

            x = XmlHandler.Read(configfile);
            this.AuthPassword = XmlHandler.GetStringFromXml(x, "Password", this.AuthPassword);
            this.AuthUser = XmlHandler.GetStringFromXml(x, "User", this.AuthUser);
            this.Domain = XmlHandler.GetStringFromXml(x, "Domain", this.Domain);
        }
    }
}
