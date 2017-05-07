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

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class TsDropDownList : CollectionViewGuiOptionBase, IValidationGuiOption
    {
        private TsDropDownListUI _dropdownlistui;
        private ListItem _currentitem;

        
        //private List<QueryPriorityList> _querylists = new List<QueryPriorityList>();


        //properties
        public List<ListItem> VisibleOptions { get { return this._builder.Items.Where(x => x.IsEnabled == true).ToList(); } }
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
        public ListItem CurrentItem
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
        public TsDropDownList(XElement InputXml, TsColumn Parent, IDirector director) : base(InputXml,Parent, director)
        {
            this._setvaluequerylist = new QueryPriorityList(this, this._director);
            this._dropdownlistui = new TsDropDownListUI();
            this.Control = this._dropdownlistui;
            this.Label = new TsLabelUI();

            this._validationhandler = new ValidationHandler(this, director);
            this._validationtooltiphandler = new ValidationToolTipHandler(this, this._director);

            this.UserControl.DataContext = this;
            this.SetDefaults();
            this.LoadXml(InputXml);
            this._builder.Rebuild();
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
                    this._setvaluequerylist.AddQuery(defquery);
                }
                
                //read in an option and add to a dictionary for later use
                else if (x.Name == "Option")
                {
                    ListItem newoption = new ListItem(x, this.ControlFormatting, this, this._director);
                    this._builder.Add(newoption);

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
                    QueryPriorityList newlist = new QueryPriorityList(this,this._director);
                    newlist.LoadXml(wrapx);

                    this._builder.Add(newlist);
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
            ListItem newdefault = null;

            if (this._nodefaultvalue == false)
            {
                int index = 0;
                string defaultval = this._setvaluequerylist.GetResultWrangler()?.GetString();
                foreach (ListItem item in this.VisibleOptions)
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
            ListItem newdefault = null;
            bool changed = false;

            foreach (ListItem item in this.VisibleOptions)
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



        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

        

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

        public override void RefreshValue()
        {
            this.SetSelected(this._setvaluequerylist.GetResultWrangler()?.GetString());
        }

        public override void RefreshAll()
        {
            this._builder.Rebuild();
            this.OnPropertyChanged(this, "VisibleOptions");
            this.SetSelected(this._setvaluequerylist.GetResultWrangler()?.GetString());
        }

        private void OnOptionsListUpdated()
        {
            this.OnPropertyChanged(this, "VisibleOptions");
            this.SetComboBoxDefault();
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



        public override bool Validate(bool CheckSelectionMade)
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
