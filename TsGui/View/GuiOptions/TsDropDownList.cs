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
using System.Windows.Controls;
using System.Windows;

namespace TsGui.View.GuiOptions
{
    public class TsDropDownList: GuiOptionBase, IGuiOption_2, IToggleControl
    {
        public event ToggleEvent ToggleEvent;

        private TsDropDownListUI _ui;
        private string _value;
        private List<TsDropDownListItem> _options = new List<TsDropDownListItem>();
        private bool _istoggle = false;

        //standard stuff
        public UserControl Control { get { return this._ui; } }


        //Custom stuff for control
        public List<TsDropDownListItem> Options { get { return this._options; } }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.UpdateSelected()); }
            }
        }
        public string CurrentValue { get { return this.UpdateSelected(); } }


        //Constructor
        public TsDropDownList(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent)
        {
            this._controller = MainController;
            this._ui = new TsDropDownListUI();
            this._ui.DataContext = this;
            this.LoadXml(InputXml);
            this.SetDefault();
        }


        //Methods
        public void LoadXml(XElement InputXml)
        {
            this.LoadBaseXml(InputXml);
            #region

            IEnumerable<XElement> inputElements = InputXml.Elements();

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
                            this._value = xdefoption.Value;
                            break;
                        }
                        else if (xdefoption.Name == "Query")
                        {
                            //code to be added
                        }
                    }

                    if (defxCount == 0) { this._value = x.Value.Trim(); }
                }

                //now read in an option and add to a dictionary for later use
                if (x.Name == "Option")
                {
                    string optval = x.Element("Value").Value;
                    string opttext = x.Element("Text").Value;
                    this._options.Add(new TsDropDownListItem(optval, opttext));

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
                        TsDropDownListItem item = new TsDropDownListItem(kv.Key, kv.Value);
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

        private string UpdateSelected()
        {
            TsDropDownListItem selected = (TsDropDownListItem)this._ui.Control.SelectedItem;
            this._value = selected.Value;
            return this._value;
        }

        //iterate through the list and set the default if found
        private void SetDefault()
        {
            int index = 0;

            foreach (TsDropDownListItem entry in this._options)
            {
                //if this entry is the default, or is the first in the list (in case there is no
                //default, select it by default in the list
                if ((entry.Value == this._value) || (index == 0))
                { this._ui.Control.SelectedItem = entry; }

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

        //Method to work around an issue where dropdown doesn't grey the text if disabled. This opens
        //and closes the dropdown so it initialises proeprly
        public void OnParentWindowLoaded()
        {
            this._ui.Control.IsDropDownOpen = true;
            this._ui.Control.IsDropDownOpen = false;
        }
    }
}
