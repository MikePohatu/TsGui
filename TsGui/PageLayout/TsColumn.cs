using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System;

namespace TsGui
{
    public class TsColumn: INotifyPropertyChanged
    {
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid _columnpanel;
        private Thickness _margin = new Thickness(2,2,2,2);
        private bool _gridlines;
        private GridLength _labelwidth;
        private GridLength _controlwidth;
        private GridLength _fullwidth;
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
        public GridLength LabelWidth
        {
            get { return this._labelwidth; }
            set
            {
                this._labelwidth = value;
                this.OnPropertyChanged(this, "LabelWidth");
            }
        }
        public GridLength ControlWidth
        {
            get { return this._controlwidth; }
            set
            {
                this._controlwidth = value;
                this.OnPropertyChanged(this, "ControlWidth");
            }
        }
        public GridLength Width
        {
            get { return this._fullwidth; }
            set
            {
                this._fullwidth = value;
                this.OnPropertyChanged(this, "Width");
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

            this._columnpanel.DataContext = this;
            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefControls.SetBinding(ColumnDefinition.WidthProperty, new Binding("ControlWidth"));

            //Set defaults
            this._gridlines = false;
            this._columnpanel.VerticalAlignment = VerticalAlignment.Top;

            this._columnpanel.ColumnDefinitions.Add(this._coldefLabels);
            this._columnpanel.ColumnDefinitions.Add(this._coldefControls);
            
            this.LoadXml(SourceXml);
            this.Build();
        }

        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
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
            { this.LabelWidth = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("ControlWidth");
            if (x != null)
            { this.ControlWidth = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("Width");
            if (x != null)
            { this.Width = new GridLength(Convert.ToDouble(x.Value)); }

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
    }
}
