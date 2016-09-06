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

// TsDropDownList.cs - dropdownlist. code to be added to be able to use this as a toggle

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace TsGui
{
    public class TsDropDownList: TsBaseOption, IGuiOption, IToggleControl
    {
        public event ToggleEvent ToggleEvent;

        new private ComboBox _control;
        private bool _istoggle = false;

        //dictionary in format text description,value
        //private List<KeyValuePair<string, string>> _options = new List<KeyValuePair<string, string>>();
        private List<TsDropDownListItem> _options = new List<TsDropDownListItem>();

        public TsDropDownList(XElement SourceXml, MainController RootController) : base()
        {
            //Debug.WriteLine("TsDropDownList constructor called");
            this._controller = RootController;

            this._control = new ComboBox();
            base._control = this._control;

            this._control.DataContext = this;
            
            this._control.SetBinding(ComboBox.IsEnabledProperty, new Binding("IsEnabled"));
            this._control.SetBinding(ComboBox.PaddingProperty, new Binding("Padding"));
            this._control.SetBinding(ComboBox.MarginProperty, new Binding("Margin"));
            this._control.SetBinding(ComboBox.HeightProperty, new Binding("Height"));

            this._control.VerticalAlignment = VerticalAlignment.Bottom;
            this._visiblepadding = new Thickness(6, 2, 2, 3);
            this.Padding = this._visiblepadding;

            this._visiblemargin = new Thickness(2, 2, 2, 2);
            this.Margin = this._visiblemargin;

            this._control.ItemsSource = this._options;
            //this._control.DisplayMemberPath = "Value";
            //this._control.SelectedValuePath = "Key";

            this.Height = 20;

            this.LoadXml(SourceXml);       
            this.SetDefault();

            this._control.SelectionChanged += this.OnChanged;
        }

        //properties
        public TsVariable Variable 
        { 
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                {
                    //get the current value from the combobox
                    this.UpdateSelected();
                    return new TsVariable(this.VariableName, this._value);
                }
            }
        }
        public string CurrentValue
        {
            get
            {
                this.UpdateSelected();
                return this._value;
            }
        }

        public void LoadXml(XElement InputXml)
        {
            #region
            //load the xml for the base class stuff
            this.LoadBaseXml(InputXml);

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
                            //Debug.WriteLine("LoadXml default: " + xdefoption.Value);
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
                    //this._options.Add(new KeyValuePair<string, string>(optval, opttext));
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
                        this._options.Add(new TsDropDownListItem(kv.Key,kv.Value));
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

        private void UpdateSelected()
        {
            //KeyValuePair<string, string> selected = (KeyValuePair<string, string>)this._control.SelectedItem;
            TsDropDownListItem selected = (TsDropDownListItem)this._control.SelectedItem;
            this._value = selected.Value;
        }

        //build the actual display control
        private void SetDefault()
        {
            int index = 0;

            foreach (TsDropDownListItem entry in this._options)
            {
                //if this entry is the default, or is the first in the list (in case there is no
                //default, select it by default in the list
                if ((entry.Value == this._value) || (index == 0))
                { this._control.SelectedItem = entry; }

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

        private void OnIsEnabledChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            //this.ToggleEvent?.Invoke();
            //if (this._control.IsEnabled ==true
            ((ComboBoxItem)this._control.SelectedItem).IsEnabled = this._control.IsEnabled;
        }
    }
}
