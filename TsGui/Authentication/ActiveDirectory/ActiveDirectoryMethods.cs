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
using System.Windows;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using Core.Logging;

namespace TsGui.Authentication.ActiveDirectory
{
    public static class ActiveDirectoryMethods
    {
        public static bool IsUserMemberOfGroups(PrincipalContext context, string samaccountname, List<string> groups)
        {
            using (UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samaccountname))
            {
                if (user != null)
                {
                    GroupPrincipal prigroup = GetPrimaryGroup(context, user);
                    PrincipalSearchResult<Principal> usergroups = user.GetAuthorizationGroups();

                    if (groups == null) { return true; }

                    foreach (string groupname in groups)
                    {
                        using (GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupname))
                        {
                            if (group == null) { Log.Warn("Group not found: " + groupname); }
                            else
                            {
                                //work around issue where IsMemherOf always returns false on users primary group
                                if (group.Equals(prigroup)) { return true; }

                                //now do normal processing
                                bool ismember = user.IsMemberOf(group);
                                if (ismember == false) { return false; }
                            }
                        }
                    }
                    return true;
                }

                else { throw new NoMatchingPrincipalException("User not found: " + samaccountname); }
            }
        }

        public static GroupPrincipal GetPrimaryGroup(PrincipalContext context, UserPrincipal user)
        {
            DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry;
            return GroupPrincipal.FindByIdentity(context, IdentityType.Sid, GetPrimaryGroupSid(de));
        }

        /// <summary>
        /// Get the primaryGroupID attribute of the user DirectoryEntry in string format
        /// </summary>
        /// <param name="userentry"></param>
        /// <returns></returns>
        public static string GetPrimaryGroupSid(DirectoryEntry userentry)
        {
            string primaryGroupID = userentry.Properties["primaryGroupID"].Value.ToString();
            byte[] objectSidByteArray = (byte[])userentry.Properties["objectSid"].Value;
            string sid = new SecurityIdentifier(objectSidByteArray,0).ToString();
            StringBuilder builder = new StringBuilder();

            string[] splitsid = sid.Split('-');
            int i = 0;

            while (i < splitsid.Count() - 1)
            {
                if (i == 0) { builder.Append(splitsid[i]); }
                else { builder.Append("-" + splitsid[i]); }
                i++;
            }

            builder.Append("-" + primaryGroupID);
            return builder.ToString();
        }
    }
}
