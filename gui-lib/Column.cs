using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace gui_lib
{
    public class Column
    {
        private List<IGuiOption> options = new List<IGuiOption>();

        public List<IGuiOption> Options { get { return this.options; } }

        //constructor
        public Column (XElement SourceXml)
        {
            this.LoadXml(SourceXml);
        }

        private void LoadXml(XElement SourceXml)
        {
            IEnumerable<XElement> optionsXml;
            //now read in the options and add to a dictionary for later use
            optionsXml = SourceXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    //need to update with factory
                    if (xOption.Attribute("Type").Value == "DropDownList")
                    {
                        this.options.Add(new TsDropDownList(xOption));
                    }

                    else if (xOption.Attribute("Type").Value == "FreeText")
                    {
                        this.options.Add(new TsFreeText(xOption));
                    }

                    //else if (xOption.Attribute("Type").Value == "FreeText")
                    //{
                    //    this.options.Add(new TsFreeText(xOption));
                    //}
                }
            }
        }
    }
}
