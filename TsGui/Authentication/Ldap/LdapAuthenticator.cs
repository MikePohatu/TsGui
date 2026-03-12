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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Options;

namespace TsGui.Authentication.Ldap
{
    public class LdapAuthenticator : IAuthenticator
    {
        public event AuthValueChanged AuthStateChanged;

        private AuthState _state = AuthState.NotAuthed;
        private string _domain;
        private string _server;
        private bool _requireAllGroups = false;
        private bool _createIDs = false;
        public bool _ssl = true;

        public AuthState State { get { return this._state; } }
        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string Domain { get { return this._domain; } }
        public string AuthID { get; set; }
        public List<string> Groups { get; private set; } = new List<string>();
        public LdapConnection Connection { get; private set; }
        public string BaseDN { get; private set; }

        public LdapAuthenticator(XElement inputxml)
        {
            this.LoadXml(inputxml);
            Director.Instance.AppClosing += OnAppClosing;
        }

        private void OnAppClosing(object sender, EventArgs e)
        {
            if (this.Connection != null && this.Connection.IsConnected)
            {
                this.Connection?.Close();
            }
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

            Log.Info("Authenticating user: " + this.UsernameSource.Username + " against domain " + this._domain);
            AuthState newstate = AuthState.AccessDenied;
            Dictionary<string, bool> groupmemberships = null;

            try
            {
                var options = new LdapConnectionOptions {
                    Host = this._server,
                    UseSsl = this._ssl,
                };

                this.Connection = new LdapConnection(options);
                this.Connection.Connect();
                this.Connection.SimpleBind(this.UsernameSource.Username, this.PasswordSource.Password);

                groupmemberships = LdapMethods.IsUserMemberOfGroups(this.Connection, this.UsernameSource.Username, this.Groups, this.BaseDN);

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
                    Log.Info("LDAP authorised");
                    newstate = AuthState.Authorised;
                }
                else
                {
                    Log.Info("LDAP not authorised");
                    newstate = AuthState.NotAuthorised;
                }
            }
            catch (Exception e)
            {
                Log.Warn("LDAP access denied");
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

            this._domain = XmlHandler.GetStringFromXml(inputxml, "Domain", this._domain);
            if (string.IsNullOrWhiteSpace(this._domain) == true)
            { throw new KnownException("Missing Domain attribute in XML:", inputxml.ToString()); }

            this._server = XmlHandler.GetStringFromXml(inputxml, "Server", _domain);

            this.BaseDN = XmlHandler.GetStringFromXml(inputxml, "BaseDN", this.BaseDN);
            

            this._requireAllGroups = XmlHandler.GetBoolFromXml(inputxml, "RequireAllGroups", this._requireAllGroups);
            this._createIDs = XmlHandler.GetBoolFromXml(inputxml, "CreateGroupIDs", this._createIDs);
            this._ssl = XmlHandler.GetBoolFromXml(inputxml, "SSL", this._ssl);

            var xa = inputxml.Attribute("Group");
            if (string.IsNullOrWhiteSpace(xa?.Value) == false)
            {
                this.AddGroup(xa.Value);
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

            //Check GroupBaseDN is set if needed, auto create if not
            if (this.Groups.Count > 0 && string.IsNullOrWhiteSpace(this.BaseDN) == true)
            {
                var domainParts = this.Domain.Split('.');
                for (int i = 0; i < domainParts.Length; i++)
                {
                    domainParts[i] = $"DC={domainParts[i]}";
                }
                this.BaseDN = string.Join(",", domainParts);
                Log.Info($"BaseDN not set in config. Automatically set to {this.BaseDN}");
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
            string strippedName = groupname.Replace("=", "_").Replace(",","_").Replace(" ", "_");
            string validatedName = null;

            if (Variable.ConfirmValidName(strippedName, out validatedName) == false)
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
    }
}