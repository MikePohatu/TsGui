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

using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using TsGui.Authentication.ActiveDirectory;
using System.Xml.Linq;

namespace TsGui.Authentication
{
    public static class AuthenticationFactory
    {
        public static IAuthenticator GetAuthenticator(XElement inputxml, IDirector director)
        {
            if (inputxml == null) { return null; }

            string type = XmlHandler.GetStringFromXAttribute(inputxml,"Type",null);
            string authid = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", null);

            if (string.IsNullOrWhiteSpace(authid) == true) { throw new TsGuiKnownException("Mising AuthID attribute from XML:", inputxml.ToString()); }

            if (string.IsNullOrWhiteSpace(type) != true)
            {
                switch (type)
                {
                    case "ActiveDirectory":                     
                        return new ActiveDirectoryAuthenticator(inputxml);
                    default:
                        throw new TsGuiKnownException("Invalid type specified in query", inputxml.ToString());
                }
            }

            else
            { return null; }
        }
    }
}
