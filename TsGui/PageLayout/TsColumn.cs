using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
//using System.Diagnostics;
using System;

namespace TsGui
{
    public class TsColumn: INotifyPropertyChanged,IGroupable
    {
        private bool _enabled;
        private bool _hidden;
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid _columngrid;
        //private Thickness _margin = new Thickness(0,0,0,0);
        private bool _gridlines;
        private GridLength _labelwidth;
        private GridLength _controlwidth;
        private GridLength _fullwidth;
        private MainController _controller;
        private ColumnDefinition _coldefControls;
        private ColumnDefinition _coldefLabels;

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
        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this._gridlines = value;
                this.OnPropertyChanged(this, "ShowGridLines");
            }
        }
        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this.options; } }
        public Panel Panel { get { return this._columngrid; } }
        public bool IsEnabled
        {
            get { return this._enabled; }
            set
            {
                this.EnableDisable(value);
                OnPropertyChanged(this, "IsEnabled");
            }
        }
        public bool IsHidden
        {
            get { return this._hidden; }
            set
            {
                this.HideUnhide(value);
                OnPropertyChanged(this, "IsHidden");
            }
        }

        //constructor
        public TsColumn (XElement SourceXml,int PageIndex, MainController RootController)
        {            
            this._controller = RootController;
            this.Index = PageIndex;
            
            this._columngrid = new Grid();

            this._columngrid.Margin = new Thickness(0);
            this._coldefControls = new ColumnDefinition();
            this._coldefLabels = new ColumnDefinition();

            this._columngrid.DataContext = this;
            //this._columngrid.SetBinding(Grid.IsEnabledProperty, new Binding("IsEnabled"));
            this._columngrid.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));

            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefControls.SetBinding(ColumnDefinition.WidthProperty, new Binding("ControlWidth"));

            //Set defaults
            this._columngrid.VerticalAlignment = VerticalAlignment.Top;

            this._columngrid.ColumnDefinitions.Add(this._coldefLabels);
            this._columngrid.ColumnDefinitions.Add(this._coldefControls);
            
            this.LoadXml(SourceXml);
            this.Build();
        }

        //Setup the INotifyPropertyChanged interface 
        #region
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
        #endregion

        private void LoadXml(XElement InputXml)
        {
            XElement x;
            string groupID = null;
            IEnumerable<XElement> optionsXml;
            IGuiOption newOption;

            x = InputXml.Element("Group");
            if (x != null)
            {
                groupID = x.Value;
                this._controller.AddToGroup(groupID, this);
            }

            //now read in the options and add to a dictionary for later use
            optionsXml = InputXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    newOption = GuiFactory.CreateGuiOption(xOption,this._controller);
                    this.options.Add(newOption);
                    this._controller.AddOptionToLibary(newOption);
                    if (!string.IsNullOrEmpty(groupID)) { this._controller.AddToGroup(groupID, newOption); }
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

            x = InputXml.Element("Enabled");
            if (x != null)
            { this.IsEnabled = Convert.ToBoolean(x.Value); }

            x = InputXml.Element("Hidden");
            if (x != null)
            { this.IsHidden = Convert.ToBoolean(x.Value); }
        }


        public void Build()
        {
            int rowindex = 0;
            double width =0;

            foreach (IGuiOption option in this.options)
            {
                //option.Control.Margin = this._margin;
                //option.Label.Margin = this._margin;

                RowDefinition coldefRow = new RowDefinition();
                //coldefRow.Height = new GridLength(option.Height + option.Margin.Top + option.Margin.Bottom) ;
                this._columngrid.RowDefinitions.Add(coldefRow);

                Grid.SetColumn(option.Label, 0);
                Grid.SetColumn(option.Control, 1);
                Grid.SetRow(option.Label, rowindex);
                Grid.SetRow(option.Control, rowindex);

                this._columngrid.Children.Add(option.Label);
                this._columngrid.Children.Add(option.Control);

                //Debug.WriteLine("Control width (" + option.Label.Content + "): " + width);
                if (width < option.Control.Width)
                {                   
                    width = option.Control.Width;
                }

                rowindex++;
            }
        }

        protected void HideUnhide(bool Hidden)
        {
            this._hidden = Hidden;
            if (Hidden == true)
            {
                this._columngrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                this._columngrid.Visibility = Visibility.Visible;
            }
        }

        protected void EnableDisable(bool Enabled)
        {
            this._enabled = Enabled;
            this._columngrid.IsEnabled = Enabled;
        }
    }
}
