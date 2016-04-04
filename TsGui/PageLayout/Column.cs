using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace TsGui
{
    public class Column
    {
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid columnpanel;
        private Thickness _margin = new Thickness(2,2,2,2);
        private bool _gridlines;

        private MainController _controller;
        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this.columnpanel.ShowGridLines = value;
                this._gridlines = value;
            }
        }
        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this.options; } }
        public Panel Panel { get { return this.columnpanel; } }
        public int ControlWidth { get; set; }
        public int LabelWidth { get; set; }

        //constructor
        public Column (XElement SourceXml,int PageIndex, MainController RootController)
        {
            this._controller = RootController;
            this.Index = PageIndex;
            this._gridlines = false;
            this.LoadXml(SourceXml);
            this.Build();
        }

        private void LoadXml(XElement SourceXml)
        {
            IEnumerable<XElement> optionsXml;
            IGuiOption newOption;
            //now read in the options and add to a dictionary for later use
            optionsXml = SourceXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    newOption = GuiFactory.CreateGuiOption(xOption,this._controller);
                    this.options.Add(newOption);
                    this._controller.AddOptionToLibary(newOption);
                }
            }

            GuiFactory.LoadMargins(SourceXml, this._margin);
        }


        public void Build()
        {
            int rowindex = 0;
            Grid colGrid = new Grid();

            //colGrid.ShowGridLines = this.ShowGridLines;

            ColumnDefinition coldefControls = new ColumnDefinition();
            ColumnDefinition coldefLabels = new ColumnDefinition();

            colGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            colGrid.ColumnDefinitions.Add(coldefLabels);
            colGrid.ColumnDefinitions.Add(coldefControls);
            
            foreach (IGuiOption option in this.options)
            {
                //if ((option.Label.DataContext == null ) || (option.Control == null)) { Debug.WriteLine("null option or control"); }
                option.Control.Margin = this._margin;
                option.Label.Margin = this._margin;

                RowDefinition coldefRow = new RowDefinition();
                coldefRow.Height = new GridLength(option.Height + this._margin.Top + this._margin.Bottom) ;
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
