using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows;

namespace TsGui
{
    public class TsDropDownList: TsBaseOption, IGuiOption, INotifyPropertyChanged
    {
        new private ComboBox _control;
        
        //dictionary in format text description,value
        private Dictionary<string, string> _options = new Dictionary<string,string>();

        public TsDropDownList(XElement InputXml): base()
        {
            this._control = new ComboBox();
            base._control = this._control;

            //this._control.DataContext = this;
            //this._control.SetBinding(ComboBox.HeightProperty, new Binding("Height"));
            //this._control.SetBinding(ComboBox.WidthProperty, new Binding("ControlWidth"));

            //this._control.
            this._padding = new Thickness(6, 3, 5, 3);
            this._control.DisplayMemberPath = "Key";
            this._control.SelectedValuePath = "Value";

            this.LoadXml(InputXml);
            this.Height = 25;
        }

        //properties
        public TsVariable Variable 
        { 
                get 
                {
                    //get the current value from the combobox
                    KeyValuePair<string, string> selected = (KeyValuePair<string, string>)this._control.SelectedItem;
                    this._value = selected.Value;

                    return new TsVariable(this.VariableName, this._value); 
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
                        Debug.WriteLine("LoadXml default: " + xdefoption.Value);
                        this._value = xdefoption.Value;
                        break;
                    }
                    else if (xdefoption.Name == "Query")
                    {

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
                    this._options.Add(xOption.Element("Text").Value, xOption.Element("Value").Value);
                }         
            }

            //finished reading xml now build the control
            this.Build();
            #endregion
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
                    if (index == 0) { Debug.WriteLine("0 index"); }
                    else { Debug.WriteLine("Default value found " + entry.Value); }

                    this._control.SelectedItem = entry;
                }

                //figure out if this is the longest item in the dropdownlist. 
                if (entry.Key.Length > longeststring.Length) { longeststring = entry.Key; }
                index++;
            }

            Label templabel = new Label();
            templabel.Content = longeststring;

            //this._control.MinWidth = templabel.

            this.OnPropertyChanged(this,"Control");
        }
    }
}
