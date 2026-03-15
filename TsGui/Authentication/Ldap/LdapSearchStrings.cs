#region license
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

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Class to generate/return ldap search strings
    /// </summary>
    public static class LdapSearchStrings
    {
        public const string LdapDisabledUser = "userAccountControl:1.2.840.113556.1.4.803:=2";

        public static string UserUpn(string upn) { return $"(&(objectClass=user)(userPrincipalName={upn}))"; }
    }
}