using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsDropDownList: IGuiOption
    {
        private string name;
        private string value;
        private string label;
        private int height = 25;
        private Thickness padding = new Thickness(6,3,5,3);
        private Label labelcontrol;
        private ComboBox control;
        
        //dictionary in format text description,value
        private Dictionary<string, string> options = new Dictionary<string,string>();

        public TsDropDownList(XElement pXml)
        {
            this.LoadXml(pXml);
            this.Build();
        }

        //properties
        public TsVariable Variable 
        { 
                get 
                {
                    //get the current value from the combobox
                    KeyValuePair<string, string> selected = (KeyValuePair<string, string>)this.control.SelectedItem;
                    this.value = selected.Value;

                    return new TsVariable(this.name, this.value); 
                } 
        }

        public Label Label { get { return this.labelcontrol; } }
        public Control Control { get { return this.control; } }


        public void LoadXml(XElement pXml)
        {
            #region
            XElement x;
            IEnumerable<XElement> optionsXml;

            x = pXml.Element("Variable");
            if (x != null)
            { this.name = x.Value; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            { this.value = x.Value; }

            x = pXml.Element("Label");
            if (x != null)
            { this.label = x.Value; }

            x = pXml.Element("Height");
            if (x != null)
            { this.height = Convert.ToInt32(x.Value); }

            x = pXml.Element("Padding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this.padding = new System.Windows.Thickness(padInt,padInt,padInt,padInt);
            }

            //now read in the options and add to a dictionary for later use
            optionsXml = pXml.Elements("Option");        
            if (optionsXml != null)
            {  
                foreach (XElement xOption in optionsXml)
                {
                    this.options.Add(xOption.Element("Text").Value, xOption.Element("Value").Value);
                }         
            }

            //finished reading xml now build the control
            this.Build();
            #endregion
        }


        //build the actual display control
        private void Build()
        {
            int index = 0;
            this.control = new ComboBox();
            this.control.DisplayMemberPath = "Key";
            this.control.SelectedValuePath = "Value";
            this.control.Height = this.height;
            //this.control.Padding = this.padding;

            this.labelcontrol = new Label();
            this.labelcontrol.Content = this.label;
            this.labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            //this.labelcontrol.Padding = this.padding;

            foreach (KeyValuePair<string, string> entry in this.options)
            {
                this.control.Items.Add(entry);
                //if this entry is the default, or is the first in the list (in case there is no
                //default, select it by default in the list
                if ((entry.Value == this.value) || (index == 0))
                {
                    this.control.SelectedItem = entry;
                }

                index++;
            }
        }
    }
}
