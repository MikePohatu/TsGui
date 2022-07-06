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

// TsPasswordBox.cs - Control for passwords. These can only connected to auth, will not create 
// TsVariables because of security issues

using System;
using System.Threading.Tasks;
using System.Security;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using TsGui.Queries;
using TsGui.Authentication;
using Core.Diagnostics;
using TsGui.Validation;
using System.Windows.Input;
using Core.Logging;
using TsGui.View.Layout;
using MessageCrap;

namespace TsGui.View.GuiOptions
{
    public class TsPasswordBox: GuiOptionBase, IGuiOption, IPassword, IValidationGuiOption
    {
        public event AuthValueChanged PasswordChanged;

        private bool _expose = false;
        private TsPasswordBoxUI _passwordboxui;
        private int _maxlength;
        private ValidationToolTipHandler _validationtooltiphandler;
        private IAuthenticator _authenticator;
        private string _failuremessage = "Authorization failed";
        private string _nopasswordmessage = "Password cannot be empty";
        private static SolidColorBrush _greenbrush = new SolidColorBrush(Colors.Green);
        private static SolidColorBrush _hovergreenbrush = new SolidColorBrush(Colors.OliveDrab);

        //Properties
        #region
        public string AuthID { get; private set; }
        public SecureString SecurePassword { get { return this._passwordboxui.PasswordBox.SecurePassword; } }
        public string Password { get { return this._passwordboxui.PasswordBox.Password; } }
        public override string CurrentValue { get { return null; } }
        public int MaxLength
        {
            get { return this._maxlength; }
            set { this._maxlength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public override Variable Variable
        {
            get {
                if (this._expose) { return new Variable(this.VariableName, this.Password, this.Path); }
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

            this._expose = XmlHandler.GetBoolFromXAttribute(InputXml, "ExposePassword", this._expose);
            this._passwordboxui = new TsPasswordBoxUI();
            this.Control = this._passwordboxui;
            this.Label = new TsLabelUI();
            this._passwordboxui.PasswordBox.KeyDown += this.OnKeyDown;
            this._passwordboxui.PasswordBox.PasswordChanged += this.OnPasswordChanged;

            this.UserControl.DataContext = this;
            this.SetDefaults();

            this.ValidationHandler = new ValidationHandler(this);
            this._validationtooltiphandler = new ValidationToolTipHandler(this);
            
            this.LoadXml(InputXml);
            Director.Instance.ConfigLoadFinished += this.OnConfigLoadFinished;
        }

        private void SetDefaults()
        {
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.ControlFormatting.Padding = new Thickness(this.ControlFormatting.Padding.Left + 1);
            this.LabelText = "Password:";
        }

        public new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            this._failuremessage = XmlHandler.GetStringFromXElement(inputxml, "FailureMessage", this._failuremessage);
            this._nopasswordmessage = XmlHandler.GetStringFromXElement(inputxml, "NoPasswordMessage", this._nopasswordmessage);

            this.MaxLength = XmlHandler.GetIntFromXAttribute(inputxml, "MaxLength", this.MaxLength);
            XAttribute x = inputxml.Attribute("AuthID");
            if (x != null)
            {
                this.AuthID = x.Value;
                AuthLibrary.AddPasswordSource(this);
            }  
            else { throw new KnownException("Missing AuthID in config:", inputxml.ToString()); }      
        }


        #region validation
        public bool Validate()
        {
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }

            bool newvalid = this._authenticator.State == AuthState.Authorised;

            if (this._authenticator.State == AuthState.Authorised)
            {
                this._validationtooltiphandler.Clear();
                this.ControlFormatting.BorderBrush = _greenbrush;
                this.ControlFormatting.MouseOverBorderBrush = _hovergreenbrush;
            }
            else if (this._authenticator.State == AuthState.NoPassword)
            {
                this.ValidationText = this._nopasswordmessage;
                this._validationtooltiphandler.ShowError();
            }
            else
            {
                this.ValidationText = this._failuremessage;
                this._validationtooltiphandler.ShowError();
            }

            return newvalid;
        }

        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

        public void OnValidationChange()
        { this.Validate(); }
        #endregion


        private void OnConfigLoadFinished(object sender, EventArgs e)
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

        private void OnAuthStateChanged()
        {
            this.Validate();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                this._authenticator.Authenticate();
                e.Handled = true;
            }
        }

        public void OnPasswordChanged(object sender, EventArgs e)
        {
            //Log.Info("Password changed event");
            this.PasswordChanged?.Invoke();
        }

        //First state change needs the borderbrush thickness to be changed. Takes some thickness from padding and put it onto borderthickness
        private void FirstStateChange()
        {
            this.ControlFormatting.Padding = new Thickness(this.ControlFormatting.Padding.Left - 1);
            this.ControlFormatting.BorderThickness = new Thickness(this.ControlFormatting.BorderThickness.Left + 1);
            this._authenticator.AuthStateChanged -= this.FirstStateChange;
        }

        public override async Task UpdateValueAsync(Message message) { await Task.CompletedTask; }
    }
}
