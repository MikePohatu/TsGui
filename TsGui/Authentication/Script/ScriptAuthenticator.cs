#region license
// Copyright (c) 2026 Mike Pohatu
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
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Options;
using TsGui.Scripts;

namespace TsGui.Authentication.Script
{
    public class ScriptAuthenticator: IAuthenticator
    {
        public event AuthValueChanged AuthStateChanged;

        private int _runningCount = 0;
        private Task _processingTask;
        private PoshScript _script;
        private bool _exceptionOnError = true;
        private AuthState _state = AuthState.NotAuthed;
        private bool _createIDs = false;
        private bool _requireAllGroups = false;

        public AuthState State { get { return this._state; } }
        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string AuthID { get; set; }
        public List<string> Groups { get; private set; } = new List<string>();


        public ScriptAuthenticator(XElement inputxml)
        {
            this.LoadXml(inputxml);
        }
        private void LoadXml(XElement inputxml)
        {
            string scriptid = XmlHandler.GetStringFromXml(inputxml, "Global", null);
            this.AuthID = XmlHandler.GetStringFromXml(inputxml, "AuthID", null);

            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new KnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            this._requireAllGroups = XmlHandler.GetBoolFromXml(inputxml, "RequireAllGroups", this._requireAllGroups);
            this._createIDs = XmlHandler.GetBoolFromXml(inputxml, "CreateGroupIDs", this._createIDs);


            if (string.IsNullOrEmpty(scriptid))
            {
                if (inputxml.HasElements)
                {
                    XElement scriptx = inputxml.Element("Script");
                    this._script = new PoshScript(scriptx);
                }
                else
                {
                    this._script = new PoshScript(inputxml);
                }

            }
            else
            {
                this._script = ScriptLibrary.GetScript(scriptid) as PoshScript;
            }
            if (this._script == null) { throw new KnownException($"No script configuration for authenticator:\n{inputxml}", null); }



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


        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            PSDataCollection<PSObject> results = null;
            this._runningCount++;
            AuthState newstate = AuthState.AccessDenied;
            ScriptAuthObject authObj = null;

            if (this._processingTask == null)
            {
                try
                {
                    var userParam = new Parameter("Username", this.UsernameSource.Username);
                    var pwParam = new SecureStringParameter("Password", this.PasswordSource);
                    var runtimeParams = new List<IParameter> { userParam, pwParam };

                    this._processingTask = this._script.RunScriptAsync(runtimeParams);
                    await this._processingTask;
                    results = this._script.Result.ReturnedObject;
                }
                catch (Exception e)
                {
                    if (this._exceptionOnError)
                    {
                        throw new KnownException($"PowerShell query {this._script.Path} caused an error: {Environment.NewLine}", e.Message);
                    }
                    else
                    {
                        Log.Error(e, $"PowerShell query {this._script.Path} caused an error: {e.Message}");
                    }
                }

                //Now go through the objects returned by the script, and add the relevant values to the auth object. 
                if (results != null)
                {
                    try
                    {
                        authObj = ScriptAuthObject.GetAuthFromPosh(results);
                    }
                    catch (Exception e)
                    {
                        var submessage = $"{Environment.NewLine}Check that script returns the authentication object in the correct format. ";
                        if (this._script.IsInlineScript)
                        {
                            throw new KnownException($"Couldn't process results of PowerShell script: {Environment.NewLine}{this._script.ScriptContent}{submessage}{Environment.NewLine}Error: ", e.Message);
                        }
                        else
                        {
                            throw new KnownException($"Couldn't process results of PowerShell script {this._script.Path}{submessage}{Environment.NewLine}Error: ", e.Message);
                        }
                    }
                }

                this._processingTask = null;
            }
            //if the script is currently processing, wait for it to finish, then return the results from the other run
            else
            {
                Log.Warn($"Authentication script {this._script.Name} run multiple times, run count: {this._runningCount}");
                await this._processingTask;
            }
            
            this._runningCount--;

            //script processing finished. check the result. 
            if (authObj != null)
            {
                if (authObj.IsAuthenticated)
                {
                    //Default to the authObj.IsAuthorized. Can be overriden by group eval even if IsAuthorized is true
                    bool authorized = authObj.IsAuthorized;

                    //Check groups if they exist
                    if (this.Groups.Count > 0)
                    {
                        if (this._requireAllGroups)
                        {
                            authorized = authObj.GroupMemberships.Values.All(x => x == true);
                        }
                        else
                        {
                            authorized = authObj.GroupMemberships.Values.Any(x => x == true);
                        }

                        if (this._createIDs)
                        {
                            foreach (var group in this.Groups)
                            {
                                bool outval = false;
                                if (authObj.GroupMemberships.TryGetValue(group, out outval))
                                {
                                    await this.UpdateGroupIDAsync(group, authObj.GroupMemberships[group]);
                                }
                                else
                                {
                                    throw new KnownException($"Group not found in script result: {group}\nCheck groups in your config match your script output (case sensitive)", string.Empty);
                                }
                            }
                        }
                    }


                    if (authorized)
                    {
                        Log.Info("Script auth: authorised");
                        newstate = AuthState.Authorised;
                    }
                    else
                    {
                        Log.Info("Script auth: not authorised");
                        newstate = AuthState.NotAuthorised;
                    }
                }

            }

            this.SetState(newstate);

            var result = new AuthenticationResult(newstate, authObj.GroupMemberships);
            return result;
        }


        private void SetState(AuthState newstate)
        {
            if (newstate != this._state)
            {
                this._state = newstate;
                this.AuthStateChanged?.Invoke();
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
    }
}
