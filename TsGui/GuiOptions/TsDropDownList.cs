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
        private Dictionary<string, string> _options = new Dictionary<string,string>();

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

            this._control.DisplayMemberPath = "Key";
            this._control.SelectedValuePath = "Value";

            this.Height = 20;

            this.LoadXml(SourceXml);
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
            XElement x;
            //load the xml for the base class stuff
            this.LoadBaseXml(InputXml);

            IEnumerable<XElement> optionsXml;

            x = InputXml.Element("DefaultValue");
            if (x != null)
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

            //now read in the options and add to a dictionary for later use
            optionsXml = InputXml.Elements("Option");        
            if (optionsXml != null)
            {  
                foreach (XElement xOption in optionsXml)
                {
                    string optval = xOption.Element("Value").Value;
                    this._options.Add(xOption.Element("Text").Value, optval);

                    x = xOption.Element("Toggle");
                    if (x != null)
                    {
                        x.Add(new XElement("Enabled", optval));
                        Toggle t = new Toggle(this, this._controller, x);
                        this._istoggle = true;
                    }
                }         
            }

            optionsXml = InputXml.Elements("MultiOption");
            if (optionsXml != null)
            {
                foreach (XElement mo in optionsXml)
                {
                    Dictionary<string, string> d = this._controller.GetDictionaryFromList(mo);

                    foreach (KeyValuePair<string, string> kv in d)
                    {
                        this._options.Add(kv.Value,kv.Key);
                    }
                }
            }

            x = InputXml.Element("Toggle");
            if (x != null)
            {
                Toggle t = new Toggle(this, this._controller, x);
                this._istoggle = true;
            }

            if (this._istoggle == true) { this._controller.AddToggleControl(this); }
            //finished reading xml now build the control
            this.Build();
            #endregion
        }

        private void UpdateSelected()
        {
            KeyValuePair<string, string> selected = (KeyValuePair<string, string>)this._control.SelectedItem;
            this._value = selected.Value;
        }

        //build the actual display control
        private void Build()
        {
            //Debug.WriteLine("TsDropDownList build started");
            int index = 0;
            string longeststring = "";

            foreach (KeyValuePair<string, string> entry in this._options)
            {
                //Debug.WriteLine(entry.Value);
                this._control.Items.Add(entry);

                //if this entry is the default, or is the first in the list (in case there is no
                //default, select it by default in the list
                if ((entry.Value == this._value) || (index == 0))
                {
                    //if (index == 0) { Debug.WriteLine("0 index"); }
                    //else { Debug.WriteLine("Default value found " + entry.Value); }

                    this._control.SelectedItem = entry;
                }

                //figure out if this is the longest item in the dropdownlist. 
                if (entry.Key.Length > longeststring.Length) { longeststring = entry.Key; }
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
    }
}
