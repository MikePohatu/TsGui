﻿//    Copyright (C) 2016 Mike Pohatu

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

// TsDropDownList.cs - combobox control for user input

using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows;
using System;

using TsGui.Grouping;
using TsGui.Validation;

namespace TsGui.View.GuiOptions
{
    public class TsDropDownList: GuiOptionBase, IGuiOption, IToggleControl, IValidationGuiOption
    {
        public event ToggleEvent ToggleEvent;

        private TsDropDownListUI _dropdownlistui;
        private string _currentvalue;
        private string _defaultvalue;
        private TsDropDownListItem _currentitem;
        private List<TsDropDownListItem> _options = new List<TsDropDownListItem>();
        private bool _istoggle = false;
        private string _validationtext;
        private Color _bordercolor;
        private Color _mouseoverbordercolor;
        private Color _focusbordercolor;
        private ToolTip _controltooltip;
        private ValidationErrorToolTip _validationerrortooltip;
        private ValidationHandler _validationhandler;


        //properties
        public List<TsDropDownListItem> Options { get { return this._options; } }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.CurrentValue); }
            }
        }
        public string CurrentValue
        {
            get { return this._currentvalue; }
            set { this._currentvalue = value; this.OnPropertyChanged(this, "CurrentValue"); }
        }
        public TsDropDownListItem CurrentItem
        {
            get { return this._currentitem; }
            set { this._currentitem = value; this.OnPropertyChanged(this, "CurrentItem"); }
        }
        public bool IsValid { get { return this.Validate(); } }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }

        //Constructor
        public TsDropDownList(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this._controller.WindowLoaded += this.OnLoadReload;
            this._dropdownlistui = new TsDropDownListUI();
            this._dropdownlistui.Control.SelectionChanged += this.OnChanged;
  
            this.Control = this._dropdownlistui;
            this.Label = new TsLabelUI();
            
            this._validationerrortooltip = new ValidationErrorToolTip();
            this._controltooltip = new ToolTip();
            this._controltooltip.Background = Brushes.Transparent;
            this._controltooltip.BorderBrush = Brushes.Transparent;
            this._controltooltip.Content = _validationerrortooltip;
            this._validationhandler = new ValidationHandler(this, MainController);
            this._dropdownlistui.LostFocus += this.OnValidationEvent;
            this.UserControl.IsEnabledChanged += this.OnValidationEvent;

            this.UserControl.DataContext = this;
            this.UserControl.IsEnabledChanged += this.OnChanged;
            this.UserControl.IsVisibleChanged += this.OnChanged;
            this.SetDefaults();
            this.LoadXml(InputXml);
            this.SetComboBoxDefault();
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

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            #region

            IEnumerable<XElement> inputElements = InputXml.Elements();

            this._validationhandler.AddValidations(InputXml.Elements("Validation"));

            foreach (XElement x in inputElements)
            {
                if (x.Name == "DefaultValue")
                {
                    IEnumerable<XElement> defx = x.Elements();
                    int defxCount = 0;
                    foreach (XElement xdefoption in defx)
                    {
                        defxCount++;
                        if (xdefoption.Name == "Value")
                        {
                            this._defaultvalue = xdefoption.Value;
                            break;
                        }
                        else if (xdefoption.Name == "Query")
                        {
                            //code to be added
                        }
                    }

                    if (defxCount == 0) { this._defaultvalue = x.Value.Trim(); }
                }

                //now read in an option and add to a dictionary for later use
                if (x.Name == "Option")
                {
                    string optval = x.Element("Value").Value;
                    string opttext = x.Element("Text").Value;
                    TsDropDownListItem newoption = new TsDropDownListItem(optval, opttext, this.ControlFormatting);
                    this._options.Add(newoption);

                    XElement togglex = x.Element("Toggle");
                    if (togglex != null)
                    {
                        togglex.Add(new XElement("Enabled", optval));
                        Toggle t = new Toggle(this, this._controller, togglex);
                        this._istoggle = true;
                    }
                }

                if (x.Name == "Query")
                {
                    List<KeyValuePair<string, string>> kvlist = this._controller.GetKeyValueListFromList(x);
                    foreach (KeyValuePair<string, string> kv in kvlist)
                    {
                        TsDropDownListItem item = new TsDropDownListItem(kv.Key, kv.Value, this.ControlFormatting);
                        this._options.Add(item);
                    }
                }

                if (x.Name == "Toggle")
                {
                    Toggle t = new Toggle(this, this._controller, x);
                    this._istoggle = true;
                }

                if (this._istoggle == true) { this._controller.AddToggleControl(this); }
            }
            #endregion
        }

        private void SetComboBoxDefault()
        {
            int index = 0;

            foreach (TsDropDownListItem entry in this._options)
            {
                //if this entry is the default, or is the first in the list (in case there is no
                //default, select it by default in the list
                if ((entry.Value == this._defaultvalue) || (index == 0))
                { this.CurrentValue = entry.Value; }

                index++;
            }
        }

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        {
            this.ToggleEvent?.Invoke();
        }

        private void OnChanged(object o, RoutedEventArgs e)
        {
            this.ToggleEvent?.Invoke();
        }

        private void OnChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.ToggleEvent?.Invoke();
        }

        //Method to work around an issue where dropdown doesn't grey the text if disabled. This opens
        //and closes the dropdown so it initialises proeprly
        public void OnLoadReload()
        {
            this._dropdownlistui.Control.IsDropDownOpen = true;
            this._dropdownlistui.Control.IsDropDownOpen = false;
        }

        private void SetDefaults()
        {
            this.ControlFormatting.Padding = new Thickness(6, 2, 2, 3);
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;

            //record the default colors
            this._bordercolor = this.ControlFormatting.BorderBrush.Color;
            this._mouseoverbordercolor = this.ControlFormatting.MouseOverBorderBrush.Color;
            this._focusbordercolor = this.ControlFormatting.FocusedBorderBrush.Color;
        }

        public void ClearToolTips()
        {
            this._controltooltip.IsOpen = false;
            this._controltooltip.StaysOpen = false;
            this._dropdownlistui.ToolTip = null;
            this.ControlFormatting.BorderBrush.Color = this._bordercolor;
            this.ControlFormatting.MouseOverBorderBrush.Color = this._mouseoverbordercolor;
            this.ControlFormatting.FocusedBorderBrush.Color = this._focusbordercolor;
        }

        public void ShowInvalidToolTip()
        {
            this._dropdownlistui.ToolTip = this._controltooltip;
            this._controltooltip.PlacementTarget = this._dropdownlistui;
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

        public bool Validate()
        {
            if (this.IsActive == false) { this.ClearToolTips(); return true; }

            bool newvalid = this._validationhandler.IsValid(this.CurrentValue);

            if (newvalid == false)
            {
                string validationmessage = this._validationhandler.ValidationMessage;
                string s = "\"" + this.CurrentValue + "\" is invalid" + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + _validationhandler.FailedValidationMessage; }
                else { s = s + validationmessage; }

                this.ValidationText = s;
                this.ShowInvalidToolTip();
            }
            else { this.ClearToolTips(); }

            return newvalid;
        }
    }
}
