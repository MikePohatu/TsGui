using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Controls;

namespace gui_lib
{
    public class Column
    {
        private List<IGuiOption> options = new List<IGuiOption>();
        private StackPanel columnpanel;

        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this.options; } }
        public Panel Panel { get { return this.columnpanel; } }
        public int ControlWidth { get; set; }
        public int LabelWidth { get; set; }

        //constructor
        public Column (XElement SourceXml,int PageIndex)
        {
            this.Index = PageIndex;
            this.LoadXml(SourceXml);
            this.Build();
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

        public void Build()
        {
            StackPanel labelpanel = new StackPanel { Orientation = Orientation.Vertical };
            StackPanel controlspanel = new StackPanel { Orientation = Orientation.Vertical };
            this.columnpanel = new StackPanel { Orientation = Orientation.Horizontal };

            Grid colGrid = new Grid();
            colGrid.ColumnDefinitions.Add(new ColumnDefinition());
            foreach (IGuiOption option in this.options)
            {
                //colGrid.Children.Add
                //labelpanel.Children.Add(option.Label);
                //controlspanel.Children.Add(option.Control);
            }

            this.columnpanel.Children.Add(labelpanel);
            this.columnpanel.Children.Add(controlspanel);
        }
    }
}
