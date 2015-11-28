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
        private Grid columnpanel;

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
            //StackPanel labelpanel = new StackPanel { Orientation = Orientation.Vertical };
            //StackPanel controlspanel = new StackPanel { Orientation = Orientation.Vertical };
            //this.columnpanel = new StackPanel { Orientation = Orientation.Horizontal };

            Grid colGrid = new Grid();
            ColumnDefinition coldefControls = new ColumnDefinition();
            ColumnDefinition coldefLabels = new ColumnDefinition();

            //coldefControls.
            colGrid.ColumnDefinitions.Add(coldefLabels);
            colGrid.ColumnDefinitions.Add(coldefControls);
            
            //colGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = "1*" });
            foreach (IGuiOption option in this.options)
            {
                RowDefinition coldefRow = new RowDefinition();
                colGrid.RowDefinitions.Add(coldefRow);
                //coldefRow.Height = new gridl 20;
                //colGrid.Children.Add
                //labelpanel.Children.Add(option.Label);
                //controlspanel.Children.Add(option.Control);
                coldefRow.
                Grid.SetColumn(option.Label, 0);
                Grid.SetColumn(option.Control, 1);

                colGrid.Children.Add(option.Label);
                colGrid.Children.Add(option.Control);
            }

            this.columnpanel = colGrid;
            //this.columnpanel.Children.Add(labelpanel);
            //this.columnpanel.Children.Add(controlspanel);
        }
    }
}
