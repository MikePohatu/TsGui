﻿#region license
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

// TsCheckBox.cs - combobox control for user input

using System;
using System.Xml.Linq;
using System.Windows;
using System.Collections.Generic;

using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;
using System.Windows.Media;
using System.Windows.Controls;
using TsGui.Validation;
using TsGui.View.Layout;
using MessageCrap;

namespace TsGui.View.GuiOptions
{
    public class TsCheckBox : GuiOptionBase, IGuiOption, IValidationGuiOption, ILinkTarget
    {
        private ValidationToolTipHandler _validationtooltiphandler;
        public ValidationHandler ValidationHandler { get; private set; }

        private TsCheckBoxUI _checkboxui;
        private bool _ischecked;
        private string _valTrue = "TRUE";
        private string _valFalse = "FALSE";
        private Thickness _cbBorderMargin = new Thickness(0);

        public bool IsChecked
        {
            get { return this._ischecked; }
            set
            {
                this.SetValue(value, null);
            }
        }
        //public override string CurrentValue { get { return this.ControlText; } }
        public override string CurrentValue
        {
            get
            {
                if (this.IsChecked == true) { return this._valTrue; }
                else { return this._valFalse; }
            }
        }
        public bool IsValid { get { return this.Validate(); } }
        public override Variable Variable
        {
            get
            {
                if ((this.IsActive == false) && (PurgeInactive == true))
                { return null; }
                else
                { return new Variable(this.VariableName, this.CurrentValue, this.Path); }
            }
        }

        public Thickness CbBorderMargin
        {
            get { return this._cbBorderMargin; }
            set { this._cbBorderMargin = value; this.OnPropertyChanged(this, "CbBorderMargin"); }
        }

        protected string _validationtext;
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }

        //Constructor
        public TsCheckBox(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            this.UserControl.DataContext = this;
            TsCheckBoxUI cbui = new TsCheckBoxUI();
            this.Control = cbui;
            this._checkboxui = cbui;
            this.InteractiveControl = cbui.CheckBox;
            this.Label = new TsLabelUI();
            this.ValidationHandler = new ValidationHandler(this);
            this._validationtooltiphandler = new ValidationToolTipHandler(this);

            this.SetDefaults();
            this._querylist = new QueryPriorityList(this);          
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            XElement x;

            this.LoadLegacyXml(InputXml);

            //load the xml for the base class stuff
            base.LoadXml(InputXml);

            this._valTrue = XmlHandler.GetStringFromXElement(InputXml, "TrueValue", this._valTrue);
            this._valFalse = XmlHandler.GetStringFromXElement(InputXml, "FalseValue", this._valFalse);

            this.ValidationHandler.LoadLegacyXml(InputXml);
            this.ValidationHandler.AddValidations(InputXml.Elements("Validation"));

            x = InputXml.Element("Checked");
            if (x != null)
            { this.SetValue(true, null); }
        }

        public override void UpdateValue(Message message)
        {
            string newvalue = this._querylist.GetResultWrangler(message)?.GetString();

            if (newvalue != this.CurrentValue)
            {
                if (newvalue == this._valTrue) { this.SetValue(true, message); }
                else if (newvalue == this._valFalse) { this.SetValue(false, message); }
                else { newvalue = null; }
            }
        }

        public void OnSourceValueUpdated(Message message)
        {
            this.UpdateValue(message);
        }

        private void SetValue(bool value, Message message)
        {
            this._ischecked = value;
            this.OnPropertyChanged(this, "IsChecked");
            this.NotifyViewUpdate();
            this.InvokeToggleEvent();
            this.Validate();
            LinkingHub.Instance.SendUpdateMessage(this, message);
        }

        private void SetDefaults()
        {
            if (Director.Instance.UseTouchDefaults == true)
            {
                this.CbBorderMargin = new Thickness(1,2,1,2);
                this.ControlFormatting.Margin = new Thickness(5);
                this._checkboxui.CbBorder.TouchDown += this.OnBorderTouched;
                this._checkboxui.CbBorder.MouseLeftButtonDown += this.OnBorderTouched;
                this._checkboxui.CbBorder.BorderThickness = new Thickness(1);
                this._checkboxui.CbBorder.BorderBrush = Brushes.LightGray;
                this._checkboxui.CbBorder.Background = Brushes.Transparent;
            }
            else
            {
                this.ControlFormatting.Margin = new Thickness(1);
            }
            this.ControlFormatting.Padding = new Thickness(0);
            this.ControlFormatting.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.ControlFormatting.VerticalContentAlignment = VerticalAlignment.Center;
            this.ControlFormatting.VerticalAlignment = VerticalAlignment.Center;
        }

        private void LoadLegacyXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("HAlign");
            if (x != null)
            {
                string s = x.Value.ToUpper();
                switch (s)
                {
                    case "RIGHT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "LEFT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "CENTER":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "STRETCH":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnBorderTouched(object sender, RoutedEventArgs e)
        {
            this.IsChecked = !this._ischecked;
        }

        public bool Validate()
        {
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }

            bool newvalid = this.ValidationHandler.IsValid(this.CurrentValue);

            if (newvalid == false)
            {
                string validationmessage = this.ValidationHandler.ValidationMessage;
                string s;
                if (string.IsNullOrEmpty(validationmessage)) { s = ValidationHandler.FailedValidationMessage; }
                else { s = validationmessage; }

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
    }
}
