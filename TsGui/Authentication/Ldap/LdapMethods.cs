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

using Core.Logging;
using System.Collections.Generic;

namespace TsGui.Authentication.Ldap
{
    internal static class LdapMethods
    {
        internal static Dictionary<string, bool> IsUserMemberOfGroups(LdapConnection connection, string userUPN, List<string> groups, string baseDN)
        {
            var memberships = new Dictionary<string, bool>();
            

            if (groups != null && groups.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(baseDN))
                {
                    Log.Error("LDAP: No BaseDN configured, required for group evaluation");
                    return memberships;
                }

                var searchfilter = LdapSearchStrings.UserUpn(userUPN);
                var userEntries = connection.Search(
                    baseDn: baseDN,
                    scope: SearchScope.Subtree,
                    filter: searchfilter,
                    attributes: new[] { "cn", "mail", "sAMAccountName", "memberOf", "userPrincipalName", "distinguishedName" },
                    sizeLimit: 500,
                    timeLimit: 30
                );

                var userEntry = userEntries[0];
                var userDn = userEntry.GetFirstValue("distinguishedName");

                //first get the primary group which isn't included in memberOf
                var primaryGroupEntry = connection.GetPrimaryGroup(userDn, baseDN, new[] { "distinguishedName" });
                var primaryGroupDN = primaryGroupEntry?.GetFirstValue("distinguishedName");

                if (string.IsNullOrWhiteSpace(primaryGroupDN) == false && groups.Contains(primaryGroupDN))
                {
                    memberships.Add(primaryGroupDN, true);
                }
                else
                {
                    Log.Error($"Unable to find primary group for user: {userUPN}");
                }


                if (userEntries.Count == 0)
                {
                    Log.Warn($"User not found: {userUPN}");
                    return memberships;
                }

                if (userEntries.Count > 1)
                {
                    Log.Error($"Multiple users found: {userUPN}");
                    return memberships;
                }

                string cn = userEntry.GetFirstValue("cn");

                string mail = userEntry.GetFirstValue("mail") ?? "(no mail)";
                string sam = userEntry.GetFirstValue("sAMAccountName");
                string userPrincipalName = userEntry.GetFirstValue("userPrincipalName");
                Log.Debug($" LDAP: {sam} {cn} {mail}");

                // Multi-valued attribute
                var memberOf = userEntry.GetAttribute("memberOf");
                foreach (var group in memberOf)
                {
                    if (groups.Contains(group))
                    {
                        memberships.Add(group, true);
                    }
                }

                foreach (var group in groups)
                {
                    if (memberships.ContainsKey(group) == false)
                    {
                        memberships.Add(group, false);
                    }
                }
            }

            return memberships;
        }
    }
}
