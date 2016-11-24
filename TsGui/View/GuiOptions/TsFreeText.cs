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
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace TsGui.View.GuiOptions
{
    public class TsFreeText: GuiOptionBase, IGuiOption, IEditableGuiOption
    {
        protected TsFreeTextUI _freetextui;
        protected ToolTip _validationtooltip;
        protected string _controltext;
        protected string _validationtext;
        protected int _maxlength;
        protected Color _bordercolor;
        protected Color _mouseoverbordercolor;
        protected Color _focusbordercolor;
        private ToolTip _controltooltip;
        private ValidationErrorToolTip _validationerrortooltip;
        private ValidationHandler _validationhandler;

        //Properties
        #region
        //Custom stuff for control
        public string ControlText
        {
            get { return this._controltext; }
            set
            {
                this._controltext = value;
                this.OnPropertyChanged(this, "ControlText");
                this.Validate();
            }
        }
        public bool IsValid { get { return this.Validate(); } }
        public int MaxLength
        {
            get { return this._maxlength; }
            set { this._maxlength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.ControlText); }
            }
        }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }
        #endregion

        //Constructor
        public TsFreeText(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this.Init(MainController);
            this.LoadXml(InputXml);
        }

        protected TsFreeText(TsColumn Parent, MainController MainController): base(Parent, MainController)
        {
            this.Init(MainController);
        }

        private void Init(MainController MainController)
        {
            this._controller = MainController;
            this._controller.WindowLoaded += this.OnWindowLoaded;
            this._validationerrortooltip = new ValidationErrorToolTip();
            this._controltooltip = new ToolTip();
            this._controltooltip.Content = _validationerrortooltip;

            this._validationhandler = new ValidationHandler(this,MainController);
            this._freetextui = new TsFreeTextUI();
            this.Control = this._freetextui;
            this.Label = new TsLabelUI();
            
            this.UserControl.DataContext = this;
            this._freetextui.TextBox.LostFocus += this.OnValidationEvent;
            this.UserControl.IsEnabledChanged += this.OnValidationEvent;
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.ControlFormatting.Padding = new Thickness(3, 2, 3, 2);
            //record the default colors
            this._bordercolor = this.ControlFormatting.BorderBrush.Color;
            this._mouseoverbordercolor = this.ControlFormatting.MouseOverBorderBrush.Color;
            this._focusbordercolor = this.ControlFormatting.FocusedBorderBrush.Color;
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", this.MaxLength);
            this._validationhandler.LoadLegacyXml(InputXml);

            XElement x;
            IEnumerable<XElement> xlist;

            xlist = InputXml.Elements("Validation");
            if (xlist != null)
            {
                foreach (XElement xval in xlist)
                this._validationhandler.AddValidation(xval);
            }

            x = InputXml.Element("DefaultValue");
            if (x != null)
            {
                XAttribute xusecurrent = x.Attribute("UseCurrent");
                if (xusecurrent != null)
                {
                    //default behaviour is to check if the ts variable is already set. If it is, set that as the default i.e. add a query for 
                    //an environment variable to the start of the query list. 
                    if (!string.Equals(xusecurrent.Value, "false", StringComparison.OrdinalIgnoreCase))
                    {
                        XElement xcurrentquery = new XElement("Query", new XElement("Variable", this.VariableName));
                        xcurrentquery.Add(new XAttribute("Type", "EnvironmentVariable"));
                        x.AddFirst(xcurrentquery);
                    }
                }

                this._controltext = this._controller.GetValueFromList(x);
                if (this._controltext == null) { this._controltext = string.Empty; }

                //if required, remove invalid characters and truncate
                string invalchars = this._validationhandler.GetAllInvalidCharacters();
                if (!string.IsNullOrEmpty(invalchars)) { this._controltext = ResultValidator.RemoveInvalid(this.ControlText, this._validationhandler.GetAllInvalidCharacters()); }
                if (this.MaxLength > 0) { this._controltext = ResultValidator.Truncate(this.ControlText, this.MaxLength); }
            }
        }

        //Handle UI events
        #region

        public void OnValidationEvent(bool b)
        { this.Validate(); }

        public void OnValidationEvent(object sender, RoutedEventArgs e)
        { this.Validate(); }

        public void OnValidationEvent(object sender, DependencyPropertyChangedEventArgs e)
        { this.Validate(); }

        public void OnWindowLoaded()
        { this.Validate(); }
        #endregion

        public bool Validate()
        {
            if (this.IsActive == false) { this.ClearToolTips(); return true; }

            bool newvalid = this._validationhandler.IsValid(this.ControlText);

            if (newvalid == false)
            {
                string validationmessage = this._validationhandler.ValidationMessage;
                string s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + _validationhandler.FailedValidationMessage; }
                else { s = s + validationmessage; }

                this.ValidationText = s;
                this.ShowInvalidToolTip();
            }
            else { this.ClearToolTips(); }

            return newvalid;
        }

        public void ClearToolTips()
        {
            this._controltooltip.IsOpen = false;
            this._controltooltip.StaysOpen = false;
            this._freetextui.ToolTip = null;
            this.ControlFormatting.BorderBrush.Color = this._bordercolor;
            this.ControlFormatting.MouseOverBorderBrush.Color = this._mouseoverbordercolor;
            this.ControlFormatting.FocusedBorderBrush.Color = this._focusbordercolor;
        }

        public void ShowInvalidToolTip()
        {         
            this._freetextui.ToolTip = this._controltooltip;
            this._controltooltip.PlacementTarget = this._freetextui;
            this._controltooltip.Placement = PlacementMode.Right;
            this._controltooltip.StaysOpen = true;
            this._controltooltip.IsOpen = true;

            //update the colors to red. 
            this.ControlFormatting.BorderBrush.Color = Colors.Red;
            this.ControlFormatting.MouseOverBorderBrush.Color = Colors.Red;
            this.ControlFormatting.FocusedBorderBrush.Color = Colors.Red;
        }

        public void OnValidationChange()
        { this.Validate(); }
    }
}
