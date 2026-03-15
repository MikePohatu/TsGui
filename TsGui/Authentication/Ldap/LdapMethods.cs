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

using Core.Diagnostics;
using Core.Logging;
using Novell.Directory.Ldap;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TsGui.Authentication.Ldap
{
    internal static class LdapMethods
    {
        /// <summary>
        /// Find if a user is a member of the list of groups. Will return null if multiple users are found.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userUPN"></param>
        /// <param name="groups"></param>
        /// <param name="baseDN"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<string, bool>> IsUserMemberOfGroupsAsync(LdapConnection connection, string userUPN, List<string> groups, string baseDN)
        {
            ThrowIfNotConnected(connection);

            var memberships = new Dictionary<string, bool>();
            

            if (groups != null && groups.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(baseDN))
                {
                    Log.Error("LDAP: No BaseDN configured, required for group evaluation");
                    return memberships;
                }

                var searchfilter = LdapSearchStrings.UserUpn(userUPN);
                var attribs = new[] { "cn", "mail", "sAMAccountName", "memberOf", "userPrincipalName", "distinguishedName", "primaryGroupID", "objectSid" };

                LdapEntry userEntry = null;
                LdapSearchConstraints constraints = new LdapSearchConstraints();
                constraints.ReferralFollowing = true;

                var userEntries = await connection.SearchAsync(baseDN, LdapConnection.ScopeSub, searchfilter, attribs, false, constraints);
                while (await userEntries.HasMoreAsync())
                {
                    try
                    {
                        var nextEntry = await userEntries.NextAsync();
                        if (userEntry != null)
                        {
                            Log.Error($"Multiple users found: {userUPN}");
                            return memberships;
                        }
                        userEntry = nextEntry;
                    }
                    catch (LdapReferralException eRef)
                    {
                        Log.Trace("Referral exception, this is expected, message follows");
                        Log.Trace(eRef.Message);
                        continue;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                if (userEntry == null)
                {
                    Log.Warn($"User not found: {userUPN}");
                    return memberships;
                }

                var userDn = userEntry.GetStringValueOrDefault("distinguishedName");

                //first get the primary group which isn't included in memberOf
                var objectSid = userEntry.GetBytesValueOrDefault("objectSid");
                var primaryGroupId = userEntry.GetStringValueOrDefault("primaryGroupID");

                var primaryGroupEntry = await GetUserPrimaryGroupAsync(connection, objectSid, primaryGroupId, baseDN, new[] { "distinguishedName" });
                var primaryGroupDN = primaryGroupEntry?.GetStringValueOrDefault("distinguishedName");

                if (string.IsNullOrWhiteSpace(primaryGroupDN) == false && groups.Contains(primaryGroupDN))
                {
                    Log.Info($"Group is member: {primaryGroupDN}");
                    memberships.Add(primaryGroupDN, true);
                }
                else
                {
                    Log.Error($"Unable to find primary group for user: {userUPN}");
                }


                string cn = userEntry.GetStringValueOrDefault("cn");
                string mail = userEntry.GetStringValueOrDefault("mail") ?? "(no mail)";
                string sam = userEntry.GetStringValueOrDefault("sAMAccountName");
                string userPrincipalName = userEntry.GetStringValueOrDefault("userPrincipalName");
                Log.Debug($" LDAP: {sam} {cn} {mail}");

                // Multi-valued attribute
                var memberOf = userEntry.GetOrDefault("memberOf");
                foreach (var group in memberOf.StringValues)
                {
                    if (groups.Contains(group))
                    {
                        Log.Info($"Group is member: {group}");
                        memberships.Add(group, true);
                    }
                }

                foreach (var group in groups)
                {
                    if (memberships.ContainsKey(group) == false)
                    {
                        Log.Warn($"Group is not a member: {group}");
                        memberships.Add(group, false);
                    }
                }
            }

            return memberships;
        }


        public static async Task<LdapEntry> GetUserPrimaryGroupAsync(LdapConnection connection, byte[] objectSid, string primaryGroupId, string searchBaseDn, string[] groupAttributes = null)
        {
            ThrowIfNotConnected(connection);

            if (string.IsNullOrEmpty(searchBaseDn)) throw new ArgumentNullException("searchBaseDn");
            
            int primaryGroupRid;
            if (!int.TryParse(primaryGroupId, out primaryGroupRid))
            { throw new LdapException($"primaryGroupID value '{primaryGroupId}' is not a valid integer."); }
 
            if (objectSid == null || objectSid.Length < 24)
            { throw new LdapException("Could not read objectSid from user entry."); }
                

            // Step 2: Clone the user SID and overwrite the last sub-authority with the group RID.
            //
            // Windows SID binary layout:
            //   Byte 0    : Revision (always 1)
            //   Byte 1    : SubAuthorityCount (n)
            //   Bytes 2-7 : IdentifierAuthority (big-endian, 6 bytes)
            //   Bytes 8+  : n × 4-byte sub-authorities (little-endian)
            byte[] groupSidBytes = (byte[])objectSid.Clone();
            int subAuthCount = groupSidBytes[1];
            int lastRidOffset = 8 + (subAuthCount - 1) * 4;

            groupSidBytes[lastRidOffset + 0] = (byte)(primaryGroupRid & 0xFF);
            groupSidBytes[lastRidOffset + 1] = (byte)((primaryGroupRid >> 8) & 0xFF);
            groupSidBytes[lastRidOffset + 2] = (byte)((primaryGroupRid >> 16) & 0xFF);
            groupSidBytes[lastRidOffset + 3] = (byte)((primaryGroupRid >> 24) & 0xFF);

            // Step 3: Search for the group using an RFC 4515 escaped-hex SID filter.
            string sidFilter = $"(objectSid={SidBytesToFilterEscape(groupSidBytes)})";
            var groupResults = await connection.SearchAsync(
                @base: searchBaseDn,
                scope: LdapConnection.ScopeSub,
                filter: sidFilter,
                attrs: groupAttributes,
                typesOnly: false);

            while (await groupResults.HasMoreAsync())
            {
                var entry = await groupResults.NextAsync();
                return entry;
            }
            return null;
        }

        private static void ThrowIfNotConnected(LdapConnection connection)
        {
            if (connection == null || connection.Connected == false)
            {
                throw new KnownException("Ldap not connected","");
            }
        }

        /// <summary>
        /// Encodes a binary SID as the RFC 4515 escaped-hex notation used in LDAP filters,
        /// e.g.      ...
        /// </summary>
        private static string SidBytesToFilterEscape(byte[] sid)
        {
            var sb = new System.Text.StringBuilder(sid.Length * 3);
            foreach (byte b in sid)
                sb.Append('\\').Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
