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
using MessageCrap;
using Novell.Directory.Ldap;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using System.Xml.Linq;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Linking;
using TsGui.Options;

namespace TsGui.Authentication.LDAP
{
    internal class LdapAuthenticator: IAuthenticator
    {
        public event AuthValueChanged AuthStateChanged;

        private AuthState _state = AuthState.NotAuthed;
        private bool _requireAllGroups = false;
        private bool _createIDs = false;

        public AuthState State { get { return this._state; } }
        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string ConnectedUserDN { get; private set; } = string.Empty;
        public string Domain { get; private set; }
        public string DomainDN { get; private set; }
        public string Server { get; private set; }
        public int Port { get; private set; } = LdapConnection.DefaultSslPort;
        public bool Ssl { get; private set; } = true;
        public string AuthID { get; set; }
        public int TimeoutSecs { get; private set; } = 5;
        public List<string> Groups { get; private set; } = new List<string>();
        public LdapConnection Context { get; private set; }

        public LdapAuthenticator(XElement inputxml)
        {
            this.LoadXml(inputxml);
        }

        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(this.UsernameSource.Username) == true)
            {
                Log.Warn("Cannot autheticate with empty username");
                this.SetState(AuthState.AccessDenied);
                return new AuthenticationResult(AuthState.AccessDenied);
            }
            if (string.IsNullOrEmpty(this.PasswordSource.Password) == true)
            {
                Log.Warn("Cannot autheticate with empty password");
                this.SetState(AuthState.NoPassword);
                return new AuthenticationResult(AuthState.NoPassword);
            }

            string server = string.IsNullOrWhiteSpace(this.Server) ? this.Domain : this.Server;

            Log.Info("Authenticating user: " + this.UsernameSource.Username + " against " + server);
            AuthState newstate = AuthState.AccessDenied;
            Dictionary<string, bool> groupmemberships = null;

            try
            {
                this.Context = new LdapConnection() { SecureSocketLayer = this.Ssl };

                // connect
                try
                {
                    await this.Context.ConnectAsync(server, this.Port);

                    // bind with an username and password
                    // this how you can verify the password of a user
                    if (this.UsernameSource.Username.Contains("@") == false)
                    {
                        throw new KnownException("Username required in user@domain format", string.Empty);
                    }

                    await this.Context.BindAsync(this.UsernameSource.Username, this.PasswordSource.Password);
                    var userFilter = $"(userPrincipalName='{this.UsernameSource.Username}')";
                    string[] attrs = { "userPrincipalName", "samAccountName" };
                    var results = await this.SearchLdapAsync(userFilter, attrs);


                    await this.Context.GetRootDseInfoAsync();
                }
                catch (Exception e)
                {
                    Log.Warn("Active Directory access denied");
                    Log.Trace(e.Message + Environment.NewLine + e.StackTrace);
                    newstate = AuthState.AccessDenied;

                    //set group IDs
                    if (this._createIDs)
                    {
                        foreach (var group in this.Groups)
                        {
                            await this.UpdateGroupIDAsync(group, false);
                        }
                    }
                }

                groupmemberships = await this.IsUserMemberOfGroupsAsync(this.Context, this.UsernameSource.Username, this.Groups, this.TimeoutSecs);

                //if there are no groups required, default auth is true, otherwise false and requires check
                bool authorized = this.Groups.Count == 0;

                //not authorized, check groups
                if (authorized == false)
                {
                    if (this._requireAllGroups)
                    {
                        authorized = groupmemberships.Values.All(x => x == true);
                    }
                    else
                    {
                        authorized = groupmemberships.Values.Any(x => x == true);
                    }

                    if (this._createIDs)
                    {
                        foreach (var group in this.Groups)
                        {
                            await this.UpdateGroupIDAsync(group, groupmemberships[group]);
                        }
                    }
                }

                if (authorized)
                {
                    Log.Info("Active Directory authorised");
                    newstate = AuthState.Authorised;
                }
                else
                {
                    Log.Info("Active Directory not authorised");
                    newstate = AuthState.NotAuthorised;
                }
            }
            catch (Exception e)
            {
                Log.Warn("Active Directory access denied");
                Log.Trace(e.Message + Environment.NewLine + e.StackTrace);
                newstate = AuthState.AccessDenied;

                //set group IDs
                if (this._createIDs)
                {
                    foreach (var group in this.Groups)
                    {
                        await this.UpdateGroupIDAsync(group, false);
                    }
                }
            }

            this.SetState(newstate);

            var result = new AuthenticationResult(newstate, groupmemberships);
            return result;
        }

        private async Task UpdateGroupIDAsync(string groupName, bool ismember)
        {
            string member = ismember.ToString().ToUpper();
            Log.Debug($"Group: {groupName} | IsMember: {member}");
            var option = LinkingHub.Instance.GetSourceOption(GetGroupID(groupName)) as MiscOption;
            if (option != null)
            {
                option.CurrentValue = member;
                await option.UpdateLinkedValueAsync(MessageHub.CreateMessage(this, null));
            }
        }

        private void LoadXml(XElement inputxml)
        {
            this.AuthID = XmlHandler.GetStringFromXml(inputxml, "AuthID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new KnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            this.Server = XmlHandler.GetStringFromXml(inputxml, "Server", this.Server);
            this.Domain = XmlHandler.GetStringFromXml(inputxml, "Domain", this.Domain);
            if (string.IsNullOrWhiteSpace(this.Domain))
            {
                this.DomainDN = string.Empty;
            }
            else
            {
                var domainParts = this.Domain.Split('.');
                var dnParts = new List<string>();
                foreach ( var part in domainParts )
                {
                    dnParts.Add($"DC={part}");
                }
                this.DomainDN = String.Join(",", dnParts);
            }

            if (string.IsNullOrWhiteSpace(this.Domain) && string.IsNullOrWhiteSpace(this.Server))
            { throw new KnownException("Missing Domain or Server attribute in XML:", inputxml.ToString()); }

            this._requireAllGroups = XmlHandler.GetBoolFromXml(inputxml, "RequireAllGroups", this._requireAllGroups);
            this._createIDs = XmlHandler.GetBoolFromXml(inputxml, "CreateGroupIDs", this._createIDs);

            var xa = inputxml.Attribute("Groups");
            if (xa != null)
            {
                if (string.IsNullOrWhiteSpace(xa.Value) == false)
                {
                    var groupsplit = xa.Value.Split(',');
                    foreach (var group in groupsplit)
                    {
                        if (string.IsNullOrWhiteSpace(group) == false)
                        { this.AddGroup(group); }
                    }
                }
            }

            var x = inputxml.Element("Groups");
            if (x != null)
            {
                foreach (var g in x.Elements("Group"))
                {
                    if (string.IsNullOrWhiteSpace(g.Value) == false)
                    { this.AddGroup(g.Value); }

                }
            }
        }

        public void AddGroups(List<string> groupnames)
        {
            if (groupnames != null)
            {
                foreach (string group in groupnames) { this.AddGroup(group); }
            }
        }

        public void AddGroup(string groupname)
        {
            this.Groups.Add(groupname);
            if (this._createIDs)
            {
                var groupid = GetGroupID(groupname);
                var noui = new MiscOption(groupid, "FALSE");
                noui.ID = groupid;
                OptionLibrary.Add(noui);
            }
        }

        private string GetGroupID(string groupname)
        {
            string validatedName = null;
            if (Variable.ConfirmValidName(groupname, out validatedName) == false)
            {
                Log.Warn($"Invalid characters removed from group ID '{groupname}'");
            }
            return $"{this.AuthID}_{validatedName}";
        }

        private void SetState(AuthState newstate)
        {
            if (newstate != this._state)
            {
                this._state = newstate;
                this.AuthStateChanged?.Invoke();
            }
        }

        public async Task<Dictionary<string, bool>> IsUserMemberOfGroupsAsync(LdapConnection context, string samaccountname, List<string> groups, int timeoutSecs)
        {
            var memberships = new Dictionary<string, bool>();
            if (groups != null)
            {

                var attrs = new string[] {"objectClass", "primaryGroupID"};

                LdapSearchConstraints cons = new LdapSearchConstraints();
                cons.TimeLimit = timeoutSecs * 1000;

                var user = await context.SearchAsync(samaccountname, LdapConnection.ScopeBase, "objectClass=user", attrs, false, cons);
                if (user != null)
                {
                    foreach (string groupname in groups)
                    {
                        //using (GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupname))
                        //{
                        //    if (group == null) { Log.Warn("Group not found: " + groupname); }
                        //    else
                        //    {
                        //        //work around issue where IsMemherOf always returns false on users primary group
                        //        if (group.Equals(prigroup)) { memberships.Add(groupname, true); }
                        //        else { memberships.Add(groupname, user.IsMemberOf(group)); }
                        //    }
                        //}
                    }
                }

                else { throw new KnownException("User not found: " + samaccountname, ""); }
            }

            return memberships;
        }

        private async Task<string> SearchLdapAsync(string filter, string[] attrs)
        {
            var results = await this.Context.SearchAsync(this.DomainDN, LdapConnection.ScopeSub, filter, attrs, false);

            while (await results.HasMoreAsync())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = await results.NextAsync();
                }
                catch (LdapException e)
                {
                    Log.Error(e.LdapErrorMessage);
                    // Exception is thrown, go for next entry
                    continue;
                }
                Log.Debug($"Ldap search result DN: {nextEntry.Dn}");

                LdapAttributeSet attributeSet = nextEntry.GetAttributeSet();
                System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                while (ienum.MoveNext())
                {
                    LdapAttribute attribute = (LdapAttribute)ienum.Current;
                    string attributeName = attribute.Name;
                    string attributeVal = attribute.StringValue;
                    //if (!Base64.isLDIFSafe(attributeVal))
                    //{
                    //    byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                    //    attributeVal = Base64.EncodeToUtf8(SupportClass.ToSByteArray(tbyte));
                    //}
                    Log.Trace(attributeName + "value:" + attributeVal);
                }
            }

            return string.Empty;
        }
    }
}
