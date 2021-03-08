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

// TsFreeText.cs - TextBox control for entering text. Can be configured to 
// check for the validity of the entered text

using TsGui.Validation;

using System;
using System.Windows;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Queries;
using System.Windows.Controls;
using MessageCrap;

namespace TsGui.View.GuiOptions
{
    public class TsFreeText: GuiOptionBase, IGuiOption, IValidationGuiOption, ILinkTarget
    {
        protected TsFreeTextUI _freetextui;

        private ValidationToolTipHandler _validationtooltiphandler;
        public ValidationHandler ValidationHandler { get; private set; }

        //Properties
        #region
        //Custom stuff for control
        protected string _controltext;
        public string ControlText
        {
            get { return this._controltext; }
            set
            {
                if (this._charactercasing == CharacterCasing.Normal) { this._controltext = value; }
                else if (this._charactercasing == CharacterCasing.Upper) { this._controltext = value?.ToUpper(); }
                else if (this._charactercasing == CharacterCasing.Lower) { this._controltext = value?.ToLower(); }
                this.OnPropertyChanged(this, "ControlText");
                this.NotifyViewUpdate();
                this.Validate();
            }
        }
        public override string CurrentValue { get { return this._controltext; } }
        public bool IsValid { get { return this.Validate(); } }

        protected int _maxlength;
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
                { return new TsVariable(this.VariableName, this.ControlText); }
            }
        }

        protected string _validationtext;
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }

        protected CharacterCasing _charactercasing = CharacterCasing.Normal;
        public CharacterCasing CharacterCasing
        {
            get { return this._charactercasing; }
            set { this._charactercasing = value; this.OnPropertyChanged(this, "CharacterCasing"); }
        }
        #endregion

        //Constructor
        public TsFreeText(XElement InputXml, TsColumn Parent): base (Parent)
        {
            this.Init();
            this.LoadXml(InputXml);
        }

        protected TsFreeText(TsColumn Parent) : base(Parent)
        {
            this.Init();
        }

        private void Init()
        {
            this._querylist = new QueryPriorityList(this);

            this._freetextui = new TsFreeTextUI();
            this.Control = this._freetextui;
            this.InteractiveControl = this._freetextui.TextBox;
            this.Label = new TsLabelUI();

            this.ValidationHandler = new ValidationHandler(this);
            this._validationtooltiphandler = new ValidationToolTipHandler(this);

            this.UserControl.DataContext = this;
            Director.Instance.WindowLoaded += this.OnWindowLoaded;
            this._freetextui.TextBox.LostFocus += this.OnValidationEvent;
            this._freetextui.TextBox.GotFocus += this.OnGotFocus;
            this._freetextui.TextBox.TextChanged += this.OnTextChanged;
            this.UserControl.IsEnabledChanged += this.OnValidationEvent;
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", this.MaxLength);
            this.ValidationHandler.LoadLegacyXml(InputXml);
            this.ValidationHandler.AddValidations(InputXml.Elements("Validation"));

            XElement x;         

            x = InputXml.Element("DefaultValue");
            if (x != null)
            {
                this.LoadSetValueXml(x,false);
            }

            x = InputXml.Element("CharacterCasing");
            if (x?.Value != null)
            {
                if (x.Value.Equals("Upper", StringComparison.OrdinalIgnoreCase))
                { this.CharacterCasing = CharacterCasing.Upper; }
                else if (x.Value.Equals("Lower", StringComparison.OrdinalIgnoreCase))
                { this.CharacterCasing = CharacterCasing.Lower; }

            }
        }

        //Handle UI events
        #region
        public void OnGotFocus(object sender, RoutedEventArgs e)
        {
            this._freetextui.TextBox.SelectAll();
        }

        public void OnValidationEvent(bool b)
        { this.Validate(); }

        public void OnValidationEvent(object sender, RoutedEventArgs e)
        { this.Validate(); }

        public void OnValidationEvent(object sender, DependencyPropertyChangedEventArgs e)
        { this.Validate(); }

        public void OnWindowLoaded(object o, RoutedEventArgs e)
        { this.Validate(); }

        public void OnTextChanged(object sender, RoutedEventArgs e)
        { this.InvokeToggleEvent(); }
        #endregion

        public bool Validate()
        {
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }

            bool newvalid = this.ValidationHandler.IsValid(this.ControlText);

            if (newvalid == false)
            {
                string validationmessage = this.ValidationHandler.ValidationMessage;
                string s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine;
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

        public override void UpdateValue(Message message)
        {
            string s = this._querylist.GetResultWrangler(message)?.GetString();
            if (s != null) 
            {
                //if required, remove invalid characters and truncate
                string invalchars = this.ValidationHandler.GetAllInvalidCharacters();
                if (!string.IsNullOrEmpty(invalchars)) { s = ResultValidator.RemoveInvalid(s, this.ValidationHandler.GetAllInvalidCharacters()); }
                if (this.MaxLength > 0) { s = ResultValidator.Truncate(s, this.MaxLength); }

                if (this.ControlText != s) { this.ControlText = s; }
            }

            Director.Instance.LinkingHub.SendUpdateMessage(this, message);
        }

        public void OnSourceValueUpdated(Message message)
        {
            this.UpdateValue(message);
        }
    }
}
