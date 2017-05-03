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
using System.Linq;
using System.Windows;
using System;

using TsGui.Grouping;
using TsGui.Validation;
using TsGui.Queries;
using TsGui.Linking;

namespace TsGui.View.GuiOptions
{
    public class TsDropDownList : GuiOptionBase, IGuiOption, IToggleControl, IValidationGuiOption, ILinkTarget
    {
        public event ToggleEvent ToggleEvent;

        private TsDropDownListUI _dropdownlistui;
        private TsDropDownListItem _currentitem;
        private List<TsDropDownListItem> _items = new List<TsDropDownListItem>();
        private Dictionary<string, Group> _itemGroups = new Dictionary<string, Group>();
        private bool _istoggle = false;
        private string _validationtext;
        private ValidationToolTipHandler _validationtooltiphandler;
        private ValidationHandler _validationhandler;
        private bool _nodefaultvalue;
        private string _noselectionmessage;


        //properties
        public List<TsDropDownListItem> VisibleOptions { get { return this._items.Where(x => x.IsEnabled == true).ToList(); } }
        public override TsVariable Variable
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
        public TsDropDownList(XElement InputXml, TsColumn Parent, IDirector MainController) : base(Parent, MainController)
        {
            this._director = MainController;
            this._querylist = new QueryList(this, this._director);

            this._dropdownlistui = new TsDropDownListUI();
            this.Control = this._dropdownlistui;
            this.Label = new TsLabelUI();

            this._validationhandler = new ValidationHandler(this, MainController);
            this._validationtooltiphandler = new ValidationToolTipHandler(this, this._director);

            this.UserControl.DataContext = this;
            this.SetDefaults();
            this.LoadXml(InputXml);
            this.RegisterForItemGroupEvents();
            this.SetComboBoxDefault();

            this._director.WindowLoaded += this.OnLoadReload;
            this._dropdownlistui.Control.SelectionChanged += this.OnSelectionChanged;
            this.UserControl.IsEnabledChanged += this.OnActiveChanged;
            this.UserControl.IsVisibleChanged += this.OnActiveChanged;
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            int optionindex = 0;
            base.LoadXml(InputXml);

            IEnumerable<XElement> inputElements = InputXml.Elements();

            this._validationhandler.AddValidations(InputXml.Elements("Validation"));
            this._nodefaultvalue = XmlHandler.GetBoolFromXAttribute(InputXml, "NoDefaultValue", this._nodefaultvalue);
            this._noselectionmessage = XmlHandler.GetStringFromXElement(InputXml, "NoSelectionMessage", this._noselectionmessage);

            foreach (XElement x in inputElements)
            {
                //the base loadxml will create queries before this so will win
                if (x.Name == "DefaultValue")
                {
                    IQuery defquery = QueryFactory.GetQueryObject(new XElement("Value", x.Value), this._director, this);
                    this._querylist.AddQuery(defquery);
                }
                
                //read in an option and add to a dictionary for later use
                else if (x.Name == "Option")
                {
                    TsDropDownListItem newoption = new TsDropDownListItem(optionindex, x, this.ControlFormatting, this, this._director);
                    this.AddOption(newoption);
                    optionindex++;

                    IEnumerable<XElement> togglexlist = x.Elements("Toggle");
                    foreach (XElement togglex in togglexlist)
                    {
                        togglex.Add(new XElement("Enabled", newoption.Value));
                        Toggle t = new Toggle(this, this._director, togglex);
                        this._istoggle = true;
                    }
                }

                else if (x.Name == "Query")
                {
                    XElement wrapx = new XElement("wrapx");
                    wrapx.Add(x);
                    QueryList newlist = new QueryList(this._director);
                    newlist.LoadXml(wrapx);

                    ResultWrangler wrangler = newlist.GetResultWrangler();
                    if (wrangler != null)
                    {
                        List<KeyValuePair<string, string>> kvlist = wrangler.GetKeyValueList();
                        foreach (KeyValuePair<string, string> kv in kvlist)
                        {
                            TsDropDownListItem newoption = new TsDropDownListItem(optionindex, kv.Key, kv.Value, this.ControlFormatting, this, this._director);
                            this.AddOption(newoption);
                            optionindex++;
                        }
                    }
                }

                else if (x.Name == "Toggle")
                {
                    Toggle t = new Toggle(this, this._director, x);
                    this._istoggle = true;
                }
            }

            if (this._istoggle == true) { this._director.AddToggleControl(this); }
        }

        private void SetComboBoxDefault()
        {
            TsDropDownListItem newdefault = null;

            if (this._nodefaultvalue == false)
            {
                int index = 0;
                string defaultval = this._querylist.GetResultWrangler()?.GetString();
                foreach (TsDropDownListItem item in this.VisibleOptions)
                {
                    if ((item.Value == defaultval) || (index == 0))
                    {
                        newdefault = item;
                        if (index > 0) { break; }
                    }
                    index++;
                }
            }
            this.CurrentItem = newdefault;
            this.NotifyUpdate();
        }

        private void SetSelected(string value)
        {
            TsDropDownListItem newdefault = null;
            bool changed = false;

            foreach (TsDropDownListItem item in this.VisibleOptions)
            {
                if ((item.Value == value))
                {
                    newdefault = item;
                    changed = true;
                    break;
                }
            }

            if (changed == true)
            {
                this.CurrentItem = newdefault;
            }
        }

        public void AddItemGroup(Group NewGroup)
        {
            Group g;
            this._itemGroups.TryGetValue(NewGroup.ID, out g);
            if (g == null) { this._itemGroups.Add(NewGroup.ID, NewGroup); }
        }


        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

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

        public void OnDropDownListItemGroupEvent()
        {
            this.OnOptionsListUpdated();
        }

        //Method to work around an issue where dropdown doesn't grey the text if disabled. This opens
        //and closes the dropdown so it initialises proeprly
        public void OnLoadReload(object o, RoutedEventArgs e)
        {
            this._dropdownlistui.Control.IsDropDownOpen = true;
            this._dropdownlistui.Control.IsDropDownOpen = false;
        }

        public void RefreshValue()
        {
            this.SetSelected(this._querylist.GetResultWrangler().GetString());
        }

        private void OnOptionsListUpdated()
        {
            this.OnPropertyChanged(this, "VisibleOptions");
            this.SetComboBoxDefault();
        }

        private void AddOption(TsDropDownListItem Item)
        {
            this._items.Add(Item);
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
            if (this._director.StartupFinished == false) { return true; }
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

        private void RegisterForItemGroupEvents()
        {
            foreach (Group g in this._itemGroups.Values)
            {
                g.StateEvent += this.OnDropDownListItemGroupEvent;
            }
        }
    }
}
