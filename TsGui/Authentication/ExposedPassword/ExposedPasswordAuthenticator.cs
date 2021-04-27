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
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using TsGui.Linking;
using TsGui.Options;
using TsGui.View;

namespace TsGui.Authentication.ExposedPassword
{
    public class ExposedPasswordAuthenticator: ViewModelBase, IAuthenticator, IOption, IPasswordConfirmingAuthenticator
    {
        private bool _confirm = false;
        private bool _blankallowed = false;
        private AuthState _state = AuthState.NoPassword;

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

        public AuthState Authenticate()
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

            return this._state;
        }

        private void LoadXml(XElement inputxml)
        {
            this.VariableName = XmlHandler.GetStringFromXElement(inputxml, "Variable", this.VariableName);
            this.VariableName = XmlHandler.GetStringFromXAttribute(inputxml, "Variable", this.VariableName);
            this.Path = XmlHandler.GetStringFromXElement(inputxml, "Path", this.Path);
            this.Path = XmlHandler.GetStringFromXAttribute(inputxml, "Path", this.Path);

            this.AuthID = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", null);
            this.ID = XmlHandler.GetStringFromXAttribute(inputxml, "ID", null);
            if (string.IsNullOrWhiteSpace(this.AuthID) == true)
            { throw new TsGuiKnownException("Missing AuthID attribute in XML:", inputxml.ToString()); }

            this._confirm = XmlHandler.GetBoolFromXAttribute(inputxml, "Confirm", this._confirm);
            this._blankallowed = XmlHandler.GetBoolFromXAttribute(inputxml, "AllowBlank", this._blankallowed);
        }

        public string ID { get; private set; }
        public Variable Variable { get { return new Variable(this.VariableName, this.LiveValue, this.Path); } }
        public string LiveValue { get { return this.PasswordSource?.Password; } }
        public string CurrentValue { get { return this.LiveValue; } }
        public string VariableName { get; private set; }
        public string InactiveValue { get { return this.LiveValue; } }
        public bool PurgeInactive { get; set; } = false;
        public bool IsActive { get; private set; } = true;

        public void Initialise() {
            if (this.PasswordSource == null) { LoggerFacade.Warn($"AuthID {this.AuthID} does not have a PasswordSource defined"); }
            else
            {
                this.PasswordSource.PasswordChanged += this.OnPasswordChanged;
            }
            if (this._confirm)
            {
                if (this.PasswordConfirmationSource == null) { LoggerFacade.Warn($"AuthID {this.AuthID} does not have a PasswordConfirmationSource defined"); }
                else { this.PasswordConfirmationSource.PasswordChanged += this.OnPasswordChanged; }
            }
        }

        public void OnPasswordChanged()
        {
            this.Authenticate();
            this.NotifyUpdate();
        }

        protected void NotifyUpdate()
        {
            //LoggerFacade.Info(this.VariableName + " variable value changed.");
            this.OnPropertyChanged(this, "CurrentValue");
            this.OnPropertyChanged(this, "LiveValue");
        }

        public void UpdateValue(Message message)
        {
            LinkingHub.Instance.SendUpdateMessage(this, message);
        }
    }
}
