﻿#region license
// Copyright (c) 2025 Mike Pohatu
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
using Core.Diagnostics;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Authentication.LocalConfig;
using TsGui.Authentication.ExposedPassword;
using System.Xml.Linq;

namespace TsGui.Authentication
{
    public static class AuthenticationFactory
    {
        public static IAuthenticator GetAuthenticator(XElement inputxml)
        {
            if (inputxml == null) { return null; }

            string type = XmlHandler.GetStringFromXml(inputxml,"Type",null);
            string authid = XmlHandler.GetStringFromXml(inputxml, "AuthID", null);

            if (string.IsNullOrWhiteSpace(authid) == true) { throw new KnownException("Mising AuthID attribute from XML:", inputxml.ToString()); }

            if (string.IsNullOrWhiteSpace(type) != true)
            {
                switch (type)
                {
                    case "ActiveDirectory":                     
                        return new ActiveDirectoryAuthenticator(inputxml);
                    case "Password":
                        return new LocalConfigPasswordAuthenticator(inputxml);
                    case "ExposedPassword":
                        ExposedPasswordAuthenticator auth = new ExposedPasswordAuthenticator(inputxml);
                        Director.Instance.AddOptionToLibary(auth);
                        return auth;
                    default:
                        throw new KnownException("Invalid type specified in query", inputxml.ToString());
                }
            }

            else
            { return null; }
        }
    }
}
