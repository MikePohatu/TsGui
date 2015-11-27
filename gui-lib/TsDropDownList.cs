using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;

namespace gui_lib
{
    public class TsDropDownList: IGuiOption
    {
        private string name;
        private string value;
        private string label;
        private ComboBox control;
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
        public string Label { get { return this.label; } }
        public Control Control { get { return this.control; } }


        public void LoadXml(XElement pXml)
        {
            XElement x;
            IEnumerable<XElement> optionsXml;

            x = pXml.Element("Variable");
            if (x != null)
            { this.name = pXml.Element("Variable").Value; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            { this.value = pXml.Element("DefaultValue").Value; }

            x = pXml.Element("Label");
            if (x != null)
            { this.label = pXml.Element("Label").Value; }

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
        }


        //build the actual display control
        private void Build()
        {
            this.control = new ComboBox();
            this.control.DisplayMemberPath = "Key";
            this.control.SelectedValuePath = "Value";

            foreach (KeyValuePair<string, string> entry in this.options)
            {
                this.control.Items.Add(entry);
            }
        }
    }
}
