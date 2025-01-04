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
using MessageCrap;
using System.Collections.Generic;
using System.Xml.Linq;
using Core.Diagnostics;
using Core.Logging;
using TsGui.Linking;
using TsGui.Options;
using Core;
using System.Threading.Tasks;

namespace TsGui.Authentication.ExposedPassword
{
    public class ExposedPasswordAuthenticator: ViewModelBase, IAuthenticator, IOption, IPasswordConfirmingAuthenticator
    {
        private bool _confirm = false;
        private bool _blankallowed = false;
        private AuthState _state = AuthState.NotAuthed;

        public event AuthValueChanged AuthStateChanged;

        public AuthState State
        {
            get { return this._state; }
        }
        public IUsername UsernameSource { get; set; }
        public IPassword PasswordSource { get; set; }
        public IPassword PasswordConfirmationSource { get; set; }
        public string Path { get; set; }
        public string AuthID { get; set; }
        public List<string> RequiredGroups { get; private set; } = new List<string>();

        public ExposedPasswordAuthenticator(XElement inputxml)
        {
            this.LoadXml(inputxml);
        }

        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            AuthState prevstate = this._state;

            if (this.PasswordSource == null) { this._state = AuthState.AuthError; }
            else if (this._blankallowed == false && string.IsNullOrEmpty(this.PasswordSource.Password)) { this._state = AuthState.NoPassword; }
            else if (this._confirm)
            {
                if (this.PasswordConfirmationSource == null) { this._state = AuthState.AuthError; }
                else
                {
                    if (this.PasswordConfirmationSource.Password == this.PasswordSource.Password) { this._state = AuthState.Authorised; }
                    else { this._state = AuthState.NotAuthorised; }
                }
            }
            else { this._state = AuthState.Authorised; }

            if (prevstate != this._state) { this.AuthStateChanged?.Invoke(); }
            await Task.CompletedTask;
            return new AuthenticationResult(this._state);
        }

        private void LoadXml(XElement inputxml)
        {
            this.VariableName = XmlHandler.GetStringFromXml(inputxml, "Variable", this.VariableName);
            this.VariableName = XmlHandler.GetStringFromXml(inputxml, "Variable", this.VariableName);
            this.Path = XmlHandler.GetStringFromXml(inputxml, "Path", this.Path);
            this.Path = XmlHandler.GetStringFromXml(inputxml, "Path", this.Path);

            this.AuthID = XmlHandler.GetStringFromXml(inputxml, "AuthID", null);
            this.ID = XmlHandler.GetStringFromXml(inputxml, "ID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new KnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            this._confirm = XmlHandler.GetBoolFromXml(inputxml, "Confirm", this._confirm);
            this._blankallowed = XmlHandler.GetBoolFromXml(inputxml, "AllowBlank", this._blankallowed);
        }

        public string ID { get; private set; }
        public IEnumerable<Variable> Variables 
        { 
            get 
            { 
                var variable = new Variable(this.VariableName, this.LiveValue, this.Path);
                return new List<Variable> { variable };
            } 
        }
        public string LiveValue { get { return this.PasswordSource?.Password; } }
        public string CurrentValue { get { return this.LiveValue; } }
        public string VariableName { get; private set; }
        public string InactiveValue { get { return this.LiveValue; } }
        public bool PurgeInactive { get; set; } = false;
        public bool IsActive { get; private set; } = true;

        public async Task InitialiseAsync() {
            if (this.PasswordSource == null) { Log.Warn($"AuthID {this.AuthID} does not have a PasswordSource defined"); }
            else
            {
                this.PasswordSource.PasswordChangedAsync += this.OnPasswordChangedAsync;
            }
            if (this._confirm)
            {
                if (this.PasswordConfirmationSource == null) { Log.Warn($"AuthID {this.AuthID} does not have a PasswordConfirmationSource defined"); }
                else { this.PasswordConfirmationSource.PasswordChangedAsync += this.OnPasswordChangedAsync; }
            }

            await Task.CompletedTask;
        }

        public async Task OnPasswordChangedAsync()
        {
            await this.AuthenticateAsync();
            this.NotifyUpdate();
        }

        protected void NotifyUpdate()
        {
            //Log.Info(this.VariableName + " variable value changed.");
            this.OnPropertyChanged(this, "CurrentValue");
            this.OnPropertyChanged(this, "LiveValue");
        }

        public async Task UpdateLinkedValueAsync(Message message)
        {
            LinkingHub.Instance.SendUpdateMessage(this, message);
            await Task.CompletedTask;
        }
    }
}
