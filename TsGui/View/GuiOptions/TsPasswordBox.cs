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

// TsPasswordBox.cs - Control for passwords. These can only connected to auth, will not create 
// TsVariables because of security issues

using System;
using System.Threading.Tasks;
using System.Security;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using TsGui.Authentication;
using Core.Diagnostics;
using TsGui.Validation;
using System.Windows.Input;
using Core.Logging;
using TsGui.View.Layout;
using TsGui.Linking;
using System.Collections.Generic;

namespace TsGui.View.GuiOptions
{
    public class TsPasswordBox: GuiOptionBase, IGuiOption, IPassword, IValidationGuiOption
    {
        public event AuthValueChangedAsync PasswordChangedAsync;

        private bool _allowempty = false;
        private bool _isauthenticator = true;   //is this actually used for authentication, or just to record a pwf
        private bool _expose = false;
        private string _exposedpassword = null;
        private TsPasswordBoxUI _passwordboxui;
        private int _maxlength;
        private IAuthenticator _authenticator;
        private string _authenticationfailuremessage = "Authentication failed";
        private string _authorizationfailuremessage = "Authorization failed";
        private string _nopasswordmessage = "Password cannot be empty";
        private static SolidColorBrush _greenbrush = new SolidColorBrush(Colors.Green);
        private static SolidColorBrush _hovergreenbrush = new SolidColorBrush(Colors.OliveDrab);

        //Properties
        #region
        public string AuthID { get; private set; }
        public SecureString SecureString { get { return this._passwordboxui.PasswordBox.SecurePassword; } }

        public string Password { get { return this._passwordboxui.PasswordBox.Password; } }
        public override string CurrentValue { get { return this._expose ? this._exposedpassword : null; } }
        public int MaxLength
        {
            get { return this._maxlength; }
            set { this._maxlength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public override IEnumerable<Variable> Variables
        {
            get {
                if (this._expose) {
                    var variable = new Variable(this.VariableName, this._exposedpassword, this.Path);
                    return new List<Variable> { variable };
                }
                else { return null; }
            }
        }
        public ValidationHandler ValidationHandler { get; private set; }

        protected string _validationtext;
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }
        public bool IsValid { get { return this.Validate(); } }
        #endregion

        //Constructor
        public TsPasswordBox(XElement InputXml, ParentLayoutElement Parent): base (Parent)
        {
            this._querylist = null;
            this._passwordboxui = new TsPasswordBoxUI();
            this.Control = this._passwordboxui;
            this.Label = new TsLabelUI();

            this.UserControl.DataContext = this;
            this.SetDefaults();

            this.ValidationHandler = new ValidationHandler(this);
            
            this.LoadXml(InputXml);

            this._passwordboxui.PasswordBox.LostFocus += this.OnLostFocus;
            this._passwordboxui.PasswordBox.KeyDown += this.OnKeyDown;
            this._passwordboxui.PasswordBox.Loaded += this.OnLoaded;

            Director.Instance.ConfigLoadFinished += this.OnConfigLoadFinished;
        }

        private void SetDefaults()
        {
            this.ControlStyle.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.ControlStyle.Padding = new Thickness(this.ControlStyle.Padding.Left + 1);
            this.LabelText = "Password:";
        }

        public new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            this.ValidationHandler.LoadXml(inputxml);

            this._authorizationfailuremessage = XmlHandler.GetStringFromXml(inputxml, "FailureMessage", this._authorizationfailuremessage);
            this._authorizationfailuremessage = XmlHandler.GetStringFromXml(inputxml, "AuthorizationWarning", this._authorizationfailuremessage);
            this._authenticationfailuremessage = XmlHandler.GetStringFromXml(inputxml, "AuthenticationWarning", this._authenticationfailuremessage);
            this._nopasswordmessage = XmlHandler.GetStringFromXml(inputxml, "NoPasswordMessage", this._nopasswordmessage);
            this._expose = XmlHandler.GetBoolFromXml(inputxml, "ExposePassword", this._expose);
            this._allowempty = XmlHandler.GetBoolFromXml(inputxml, "AllowEmpty", this._allowempty);

            this.MaxLength = XmlHandler.GetIntFromXml(inputxml, "MaxLength", this.MaxLength);
            XAttribute x = inputxml.Attribute("AuthID");
            if (x != null)
            {
                this.AuthID = x.Value;
                AuthLibrary.AddPasswordSource(this);
            }  
            else { this._isauthenticator = false; }

            if (this._expose == false && this._isauthenticator == false) 
            { Log.Info("PasswordBox does not define an AuthID in config:" + inputxml.ToString()); }     
            
            if (this._expose == true && string.IsNullOrEmpty(this.VariableName))
            { throw new KnownException("Variable name must be set if ExposePassword option is used:", inputxml.ToString()); }
        }


        #region validation
        public bool Validate()
        {
            if (this.IsActive == false) { this.ValidationHandler.ToolTipHandler.Clear(); return true; }
            bool newvalid = true;

            if (this._isauthenticator)
            {
                newvalid = this._authenticator.State == AuthState.Authorised;

                if (this._authenticator.State == AuthState.Authorised)
                {
                    this.ValidationHandler.ToolTipHandler.Clear();
                    this.ControlStyle.BorderBrush = _greenbrush;
                    this.ControlStyle.MouseOverBorderBrush = _hovergreenbrush;
                }
                else if (this._authenticator.State == AuthState.NoPassword)
                {
                    this.ValidationText = this._nopasswordmessage;
                    this.ValidationHandler.ToolTipHandler.ShowError();
                }
                else if (this._authenticator.State == AuthState.NotAuthorised)
                {
                    this.ValidationText = this._authorizationfailuremessage;
                    this.ValidationHandler.ToolTipHandler.ShowError();
                }
                else
                {
                    this.ValidationText = this._authenticationfailuremessage;
                    this.ValidationHandler.ToolTipHandler.ShowError();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.Password) && this._allowempty == false)
                {
                    this.ValidationText = this._nopasswordmessage;
                    this.ValidationHandler.ToolTipHandler.ShowError();
                    newvalid = false;
                }
                else
                {
                    newvalid = this.ValidationHandler.IsValid(this.Password);
                    if (newvalid == false)
                    {
                        this.ValidationText = this.ValidationHandler.ValidationMessage;
                        this.ValidationHandler.ToolTipHandler.ShowError();
                    }
                    else { this.ValidationHandler.ToolTipHandler.Clear(); }
                }
            }


            return newvalid;
        }

        public void ClearToolTips()
        { this.ValidationHandler.ToolTipHandler.Clear(); }

        public void OnValidationChange()
        { this.Validate(); }
        #endregion


        private void OnConfigLoadFinished(object sender, EventArgs e)
        {
            if (this._isauthenticator)
            {
                this._authenticator = AuthLibrary.GetAuthenticator(this.AuthID);
                if (this._authenticator != null)
                {
                    this._authenticator.AuthStateChanged += this.OnAuthStateChanged;
                    this._authenticator.AuthStateChanged += this.FirstStateChange;
                }
                else
                {
                    throw new KnownException("Password box is not connected to a configured AuthID.", string.Empty);
                }
            }            
        }

        private void OnAuthStateChanged()
        {
            this.Validate();
        }

        //Loaded event happens during navigation into the page. Navigating away from the page empties the 
        //passwordBox, but we want to keep that value if exposed is enabled. When we navigate back we
        //need to empty it to match the empty controls
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ChangePassword();
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.ChangePassword();
            this.Validate();
        }

        public async void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                this.ChangePassword();
                if (this._isauthenticator)
                {
                    //validation will be initiated by the authenticator
                    await this._authenticator.AuthenticateAsync();
                }
                else
                {
                    this.Validate();
                }

                e.Handled = true;
            }
        }

        private void ChangePassword()
        {
            //Log.Info("Password changed event");
            if (this._expose) { this._exposedpassword = this._passwordboxui.PasswordBox.Password; }
            this.PasswordChangedAsync?.Invoke();
            this.NotifyViewUpdate();
            LinkingHub.Instance.SendUpdateMessage(this, null);
        }

        //First state change needs the borderbrush thickness to be changed. Takes some thickness from padding and put it onto borderthickness
        private void FirstStateChange()
        {
            this.ControlStyle.Padding = new Thickness(this.ControlStyle.Padding.Left - 1);
            this.ControlStyle.BorderThickness = new Thickness(this.ControlStyle.BorderThickness.Left + 1);
            this._authenticator.AuthStateChanged -= this.FirstStateChange;
        }

        public override async Task UpdateLinkedValueAsync(MessageCrap.Message message) { await Task.CompletedTask; }
    }
}
