//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// TsFreeText.cs - TextBox control for entering text. Can be configured to 
// check for the validity of the entered text

using TsGui.Validation;

using System;
using System.Windows;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Queries;
using System.Windows.Controls;

namespace TsGui.View.GuiOptions
{
    public class TsFreeText: GuiOptionBase, IGuiOption, IValidationGuiOption, ILinkTarget
    {
        protected TsFreeTextUI _freetextui;

        private ValidationToolTipHandler _validationtooltiphandler;
        private ValidationHandler _validationhandler;

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
                this.NotifyUpdate();
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
        public TsFreeText(XElement InputXml, TsColumn Parent, IDirector MainController): base (Parent, MainController)
        {
            this.Init(MainController);
            this.LoadXml(InputXml);
            this.RefreshValue();
        }

        protected TsFreeText(TsColumn Parent, IDirector MainController) : base(Parent, MainController)
        {
            this.Init(MainController);
        }

        private void Init(IDirector MainController)
        {
            this._director = MainController;
            this._setvaluequerylist = new QueryPriorityList(this, this._director);

            this._freetextui = new TsFreeTextUI();
            this.Control = this._freetextui;
            this.InteractiveControl = this._freetextui.TextBox;
            this.Label = new TsLabelUI();

            this._validationhandler = new ValidationHandler(this,MainController);
            this._validationtooltiphandler = new ValidationToolTipHandler(this,this._director);

            this.UserControl.DataContext = this;
            this._director.WindowLoaded += this.OnWindowLoaded;
            this._freetextui.TextBox.LostFocus += this.OnValidationEvent;
            this._freetextui.TextBox.GotFocus += this.OnGotFocus;
            this.UserControl.IsEnabledChanged += this.OnValidationEvent;
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.ControlFormatting.Padding = new Thickness(3, 2, 3, 2);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", this.MaxLength);
            this._validationhandler.LoadLegacyXml(InputXml);
            this._validationhandler.AddValidations(InputXml.Elements("Validation"));

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
        #endregion

        public bool Validate()
        {
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }

            bool newvalid = this._validationhandler.IsValid(this.ControlText);

            if (newvalid == false)
            {
                string validationmessage = this._validationhandler.ValidationMessage;
                string s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + _validationhandler.FailedValidationMessage; }
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

        public void RefreshValue()
        {
            string s = this._setvaluequerylist.GetResultWrangler()?.GetString();
            if (s != null) 
            {
                //if required, remove invalid characters and truncate
                string invalchars = this._validationhandler.GetAllInvalidCharacters();
                if (!string.IsNullOrEmpty(invalchars)) { s = ResultValidator.RemoveInvalid(s, this._validationhandler.GetAllInvalidCharacters()); }
                if (this.MaxLength > 0) { s = ResultValidator.Truncate(s, this.MaxLength); }

                if (this.ControlText != s) { this.ControlText = s; }
            }
        }

        public void RefreshAll()
        { this.RefreshValue(); }
    }
}
