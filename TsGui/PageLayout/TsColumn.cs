using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System;

namespace TsGui
{
    public class TsColumn
    {
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid _columnpanel;
        private Thickness _margin = new Thickness(2,2,2,2);
        private bool _gridlines;
        private MainController _controller;
        private ColumnDefinition _coldefControls;
        private ColumnDefinition _coldefLabels;

        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this._columnpanel.ShowGridLines = value;
                this._gridlines = value;
            }
        }
        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this.options; } }
        public Panel Panel { get { return this._columnpanel; } }


        

        //constructor
        public TsColumn (XElement SourceXml,int PageIndex, MainController RootController)
        {
            
            this._controller = RootController;
            this.Index = PageIndex;
            

            this._columnpanel = new Grid();
            this._coldefControls = new ColumnDefinition();
            this._coldefLabels = new ColumnDefinition();

            //Set defaults
            this._gridlines = false;
            this._columnpanel.VerticalAlignment = VerticalAlignment.Top;

            this._columnpanel.ColumnDefinitions.Add(this._coldefLabels);
            this._columnpanel.ColumnDefinitions.Add(this._coldefControls);

            //Setup bindings
            //this._columnpanel.SetBinding(Grid.WidthProperty, new Binding("Width"));
            //{Binding Source={x:Reference TextCol01}, Path=ActualWidth}
            //this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            //this._coldefControls.SetBinding(ColumnDefinition.WidthProperty, new Binding("ControlWidth"));

            this.LoadXml(SourceXml);
            this.Build();
        }

        private void LoadXml(XElement InputXml)
        {
            XElement x;
            IEnumerable<XElement> optionsXml;
            IGuiOption newOption;
            //now read in the options and add to a dictionary for later use
            optionsXml = InputXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    newOption = GuiFactory.CreateGuiOption(xOption,this._controller);
                    this.options.Add(newOption);
                    this._controller.AddOptionToLibary(newOption);
                }
            }

            x = InputXml.Element("LabelWidth");
            if (x != null)
            { this.SetLabelWidth(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("ControlWidth");
            if (x != null)
            { this.SetControlWidth(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("Width");
            if (x != null)
            { this.SetWidth(Convert.ToDouble(x.Value)); }

            GuiFactory.LoadMargins(InputXml, this._margin);
        }


        public void Build()
        {
            int rowindex = 0;

            foreach (IGuiOption option in this.options)
            {
                option.Control.Margin = this._margin;
                option.Label.Margin = this._margin;

                RowDefinition coldefRow = new RowDefinition();
                coldefRow.Height = new GridLength(option.Height + this._margin.Top + this._margin.Bottom) ;
                this._columnpanel.RowDefinitions.Add(coldefRow);

                Grid.SetColumn(option.Label, 0);
                Grid.SetColumn(option.Control, 1);
                Grid.SetRow(option.Label, rowindex);
                Grid.SetRow(option.Control, rowindex);

                this._columnpanel.Children.Add(option.Label);
                this._columnpanel.Children.Add(option.Control);
                
                rowindex++;
            }
        }

        //View control methods
        private void SetWidth(double Width)
        {
            //get { return this._width; }
            //set
            //{
            _columnpanel.Width = Width;
            //this._width = Width;
            //this.OnPropertyChanged(this, "Width");
            //}
        }
        private void SetControlWidth(double Width)
        {
            //get { return this._coldefControls.Width.Value; }
            //set
            //{
            //this._controlwidth = value;
            this._coldefControls.Width = new GridLength(Width);
            //this.OnPropertyChanged(this, "ControlWidth");
            //this._coldefControls.Width = this._controlwidth;
            //this._coldefControls
            //}
        }
        private void SetLabelWidth(double Width)
        {
            //get { return this._coldefLabels.Width.Value; }
            //set
            //{
            //this._labelwidth = value;
            this._coldefLabels.Width = new GridLength(Width);
            //this.OnPropertyChanged(this, "LabelWidth");
            //}
        }
    }
}
