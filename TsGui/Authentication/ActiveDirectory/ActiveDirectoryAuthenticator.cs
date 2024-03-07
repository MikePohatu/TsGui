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
using System.Xml.Linq;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using Core.Logging;
using Core.Diagnostics;
using System.Linq;
using TsGui.Linking;
using System.Threading.Tasks;
using MessageCrap;
using TsGui.Options;

namespace TsGui.Authentication.ActiveDirectory
{
    public class ActiveDirectoryAuthenticator : IAuthenticator
    {
        public event AuthValueChanged AuthStateChanged;

        private AuthState _state = AuthState.NotAuthed;
        private string _domain;
        private bool _requireAllGroups = false;
        private bool _createIDs = false;
        private string _groupIdPrefix;

        public PrincipalContext Context { get; set; }
        public AuthState State { get { return this._state; } }
        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string Domain { get { return this._domain; } }
        public string AuthID { get; set; }
        public List<string> Groups { get; set; } = new List<string>();

        public ActiveDirectoryAuthenticator(XElement inputxml)
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

            Log.Info("Authenticating user: " + this.UsernameSource.Username + " against domain " + this._domain);
            AuthState newstate;
            Dictionary<string, bool> groupmemberships = null;

            try
            {
                this.Context = new PrincipalContext(ContextType.Domain, this._domain, this.UsernameSource.Username, this.PasswordSource.Password);
                

                groupmemberships = ActiveDirectoryMethods.IsUserMemberOfGroups(this.Context, this.UsernameSource.Username, this.Groups);
                
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

                    foreach (var group in this.Groups)
                    {
                        Log.Debug($"Group: {group} | IsMember: {groupmemberships[group].ToString()}");
                        var option = LinkingHub.Instance.GetSourceOption(GetGroupID(group)) as MiscOption;
                        if (option != null)
                        {
                            option.CurrentValue = groupmemberships[group].ToString().ToUpper();
                            await option.UpdateValueAsync(MessageHub.CreateMessage(this, null));
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
            }

            this.SetState(newstate);

            var result = new AuthenticationResult(newstate, groupmemberships);
            return result;
        }

        private void LoadXml(XElement inputxml)
        {
            this.AuthID = XmlHandler.GetStringFromXml(inputxml, "AuthID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new KnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            this._groupIdPrefix = this.AuthID + "_";
            this._domain = XmlHandler.GetStringFromXml(inputxml, "Domain", null);
            if (string.IsNullOrWhiteSpace(this._domain) == true)
            { throw new KnownException("Missing Domain attribute in XML:", inputxml.ToString()); }

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

        private void AddGroup(string groupname)
        {
            this.Groups.Add(groupname);
            if (this._createIDs)
            {
                var noui = new MiscOption(GetGroupID(groupname), "FALSE");
                noui.ID = GetGroupID(groupname);
                OptionLibrary.Add(noui);
            }
        }

        private string GetGroupID(string groupname)
        {
            return $"{this._groupIdPrefix}_{groupname}";
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
