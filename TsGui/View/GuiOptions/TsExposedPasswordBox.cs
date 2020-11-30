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
using System.Security;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using TsGui.Queries;
using TsGui.Authentication;
using TsGui.Diagnostics;
using TsGui.Validation;
using System.Windows.Input;

namespace TsGui.View.GuiOptions
{
    public class TsExposedPasswordBox : GuiOptionBase, IGuiOption, IValidationGuiOption
    {
        private TsPasswordBoxUI _passwordboxui;
        private int _maxlength;
        private ValidationToolTipHandler _validationtooltiphandler;
        private string _nopasswordmessage = "Password cannot be empty";
        private string _exposedpassword = null;
        private string _invalidmessage = "Password is invalid";

        //Properties
        #region
        public SecureString SecurePassword { get { return this._passwordboxui.PasswordBox.SecurePassword; } }
        public string Password { get { return this._passwordboxui.PasswordBox.Password; } }
        public override string CurrentValue { get { return this._exposedpassword; } }
        public int MaxLength
        {
            get { return this._maxlength; }
            set { this._maxlength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public override TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this._exposedpassword); }
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
        public TsExposedPasswordBox(XElement InputXml, TsColumn Parent): base (Parent)
        {
            this._setvaluequerylist = null;

            this._passwordboxui = new TsPasswordBoxUI();
            this.Control = this._passwordboxui;
            this.Label = new TsLabelUI();

            this.UserControl.DataContext = this;
            this.SetDefaults();

            this.ValidationHandler = new ValidationHandler(this);
            this._validationtooltiphandler = new ValidationToolTipHandler(this);
            
            this.LoadXml(InputXml);
            this._passwordboxui.PasswordBox.PasswordChanged += OnExposedPasswordChanged;
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
            this._invalidmessage = XmlHandler.GetStringFromXElement(inputxml, "InvalidMessage", this._invalidmessage);
            this._nopasswordmessage = XmlHandler.GetStringFromXElement(inputxml, "NoPasswordMessage", this._nopasswordmessage);

            this.MaxLength = XmlHandler.GetIntFromXAttribute(inputxml, "MaxLength", this.MaxLength);   
        }


        #region validation
        public bool Validate()
        {
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }

            bool newvalid = this.ValidationHandler.IsValid(this._exposedpassword);

            if (newvalid == false)
            {
                string validationmessage = this.ValidationHandler.ValidationMessage;
                string s = _invalidmessage + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + ValidationHandler.FailedValidationMessage; }
                else { s = s + validationmessage; }

                this.ValidationText = s;
                this._validationtooltiphandler.ShowError();
            }
            else { this._validationtooltiphandler.Clear(); }

            return newvalid;
        }

        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

        public void OnValidationChange()
        { this.Validate(); }
        #endregion

        public void OnExposedPasswordChanged(object sender, EventArgs e)
        {
            this._exposedpassword = this._passwordboxui.PasswordBox.Password;
            this.NotifyUpdate();
        }
    }
}
