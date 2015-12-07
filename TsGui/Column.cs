using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class Column
    {
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid columnpanel;
        private Thickness margin = new Thickness(2,2,2,2);

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
            XElement x;
            IEnumerable<XElement> optionsXml;
            IGuiOption newOption;
            //now read in the options and add to a dictionary for later use
            optionsXml = SourceXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    newOption = GuiFactory.CreateGuiOption(xOption);
                    this.options.Add(newOption);
                }
            }

            x = SourceXml.Element("Margin");
            if (x != null)
            {
                int i = Convert.ToInt32(x.Value);
                this.margin = new Thickness(i,i,i,i);
            }
        }


        public void Build()
        {
            int rowindex = 0;
            Grid colGrid = new Grid();
            
            ColumnDefinition coldefControls = new ColumnDefinition();
            ColumnDefinition coldefLabels = new ColumnDefinition();

            colGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            colGrid.ColumnDefinitions.Add(coldefLabels);
            colGrid.ColumnDefinitions.Add(coldefControls);
            //colGrid.ShowGridLines = true;
            
            foreach (IGuiOption option in this.options)
            {
                option.Control.Margin = this.margin;
                option.Label.Margin = this.margin;

                RowDefinition coldefRow = new RowDefinition();
                coldefRow.Height = new GridLength(option.Height + this.margin.Top + this.margin.Bottom) ;
                colGrid.RowDefinitions.Add(coldefRow);

                Grid.SetColumn(option.Label, 0);
                Grid.SetColumn(option.Control, 1);
                Grid.SetRow(option.Label, rowindex);
                Grid.SetRow(option.Control, rowindex);

                colGrid.Children.Add(option.Label);
                colGrid.Children.Add(option.Control);
                
                rowindex++;
            }

            this.columnpanel = colGrid;
        }
    }
}
