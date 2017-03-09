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

// TsDropDownList.cs - combobox control for user input

using System.Collections.Generic;
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
        private string _defaultvalue;
        private TsDropDownListItem _currentitem;
        private List<TsDropDownListItem> _options = new List<TsDropDownListItem>();
        private bool _istoggle = false;
        private string _validationtext;
        private ValidationToolTipHandler _validationtooltiphandler;
        private ValidationHandler _validationhandler;
        private bool _nodefaultvalue;
        private string _noselectionmessage;


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
        public override string CurrentValue
        {
            get { return this._currentitem?.Value; }
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
            this._controller = MainController;

            this._dropdownlistui = new TsDropDownListUI();
            this.Control = this._dropdownlistui;
            this.Label = new TsLabelUI();

            this._validationhandler = new ValidationHandler(this, MainController);
            this._validationtooltiphandler = new ValidationToolTipHandler(this, this._controller);

            this.UserControl.DataContext = this;
            this.SetDefaults();
            this.LoadXml(InputXml);
            this.SetComboBoxDefault();

            this._controller.WindowLoaded += this.OnLoadReload;
            this._dropdownlistui.Control.SelectionChanged += this.OnSelectionChanged;
            this.UserControl.IsEnabledChanged += this.OnActiveChanged;
            this.UserControl.IsVisibleChanged += this.OnActiveChanged;
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            IEnumerable<XElement> inputElements = InputXml.Elements();

            this._validationhandler.AddValidations(InputXml.Elements("Validation"));
            this._defaultvalue = XmlHandler.GetStringFromXElement(InputXml, "DefaultValue", this._defaultvalue);
            this._nodefaultvalue = XmlHandler.GetBoolFromXAttribute(InputXml, "NoDefaultValue", this._nodefaultvalue);
            this._noselectionmessage = XmlHandler.GetStringFromXElement(InputXml, "NoSelectionMessage", this._noselectionmessage);

            foreach (XElement x in inputElements)
            {
                //now read in an option and add to a dictionary for later use
                if (x.Name == "Option")
                {
                    //string optval = x.Element("Value").Value;
                    //string opttext = x.Element("Text").Value;
                    TsDropDownListItem newoption = new TsDropDownListItem(x, this.ControlFormatting,this._controller);
                    this._options.Add(newoption);


                    IEnumerable<XElement> togglexlist = x.Elements("Toggle");
                    foreach (XElement togglex in togglexlist)
                    {
                        //XElement togglex = x.Element("Toggle");
                        //if (togglex != null)
                        //{
                            togglex.Add(new XElement("Enabled", newoption.Value));
                            Toggle t = new Toggle(this, this._controller, togglex);
                            this._istoggle = true;
                        //}
                    }
                }

                if (x.Name == "Query")
                {
                    List<KeyValuePair<string, string>> kvlist = this._controller.GetKeyValueListFromList(x);
                    foreach (KeyValuePair<string, string> kv in kvlist)
                    {
                        TsDropDownListItem item = new TsDropDownListItem(kv.Key, kv.Value, this.ControlFormatting,this._controller);
                        this._options.Add(item);
                    }
                }

                IEnumerable<XElement> xlist;

                xlist = InputXml.Elements("Toggle");
                if (xlist != null)
                {
                    //this._controller.AddToggleControl(this);

                    foreach (XElement subx in xlist)
                    {
                        Toggle t = new Toggle(this, this._controller, subx);
                    }
                }

                //if (x.Name == "Toggle")
                //{
                //    Toggle t = new Toggle(this, this._controller, x);
                //    this._istoggle = true;
                //}
            }
            if (this._istoggle == true) { this._controller.AddToggleControl(this); }
        }

        private void SetComboBoxDefault()
        {
            if (this._nodefaultvalue == false)
            {
                int index = 0;

                foreach (TsDropDownListItem entry in this._options)
                {
                    if ((entry.Value == this._defaultvalue) || (index == 0))
                    { this.CurrentItem = entry; }

                    index++;
                }
            }
        }

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        { this.ToggleEvent?.Invoke(); }

        private void OnSelectionChanged(object o, RoutedEventArgs e)
        {
            this.Validate(false);
            this.NotifyUpdate();
            this.ToggleEvent?.Invoke();
        }

        private void OnActiveChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.ToggleEvent?.Invoke();
        }

        //Method to work around an issue where dropdown doesn't grey the text if disabled. This opens
        //and closes the dropdown so it initialises proeprly
        public void OnLoadReload(object o, RoutedEventArgs e)
        {
            this._dropdownlistui.Control.IsDropDownOpen = true;
            this._dropdownlistui.Control.IsDropDownOpen = false;
        }

        private void SetDefaults()
        {
            this._nodefaultvalue = false;
            this._noselectionmessage = "Please select a value";
            this.ControlFormatting.Padding = new Thickness(6, 2, 2, 3);
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public void OnValidationChange()
        { this.Validate(false); }

        public bool Validate()
        { return this.Validate(true); }

        private bool Validate(bool CheckSelectionMade)
        {
            if (this._controller.StartupFinished == false) { return true; }
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }
            if ((CheckSelectionMade == true) && (this._dropdownlistui.Control.SelectedItem == null))
            {
                this.ValidationText = _noselectionmessage;
                this._validationtooltiphandler.ShowError();
                return false;
            }

            bool newvalid = this._validationhandler.IsValid(this.CurrentValue);

            if (newvalid == false)
            {
                string validationmessage = this._validationhandler.ValidationMessage;
                string s = "\"" + this.CurrentItem.Text + "\" is invalid" + Environment.NewLine;
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
    }
}
