using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System;
//using System.Diagnostics;
using System.ComponentModel;

namespace TsGui
{
    public class TsPage: ITsGuiElement, INotifyPropertyChanged, IGroupable
    {
        private bool _enabled = true;
        private bool _hidden = false;
        private MainController _controller;
        private double _height;
        private double _width;
        private double _headingHeight;
        private string _headingTitle;
        private string _headingText;
        private SolidColorBrush _headingBgColor;
        private SolidColorBrush _headingFontColor;
        private Thickness _margin = new Thickness(0, 0, 0, 0);
        private List<TsColumn> _columns = new List<TsColumn>();
        private List<IGuiOption> _options = new List<IGuiOption>();
        private List<IEditableGuiOption> _editables = new List<IEditableGuiOption>();
        private Grid _pagepanel;
        private PageLayout _pagelayout;
        private TsPage _previouspage;
        private TsPage _nextpage;

        //private bool _islast = false;
        private bool _isfirst = false;
        private bool _gridlines = false;

        //Properties
        #region
        public bool IsEnabled
        {
            get { return this._enabled; }
            set
            {
                this._enabled = value;
                this.OnPropertyChanged(this, "IsEnabled");
            }
        }
        public bool IsHidden
        {
            get { return this._hidden; }
            set
            {
                this._hidden = value;
                this.OnPropertyChanged(this, "IsHidden");
            }
        }
        public TsPage NextActivePage
        {
            get
            {
                if (this.NextPage == null) { return null; }
                else
                {
                    if (this.NextPage.IsHidden == false) { return this.NextPage; }
                    else { return this.NextPage.NextActivePage; }                    
                }
            }
        }
        public TsPage PreviousActivePage
        {
            get
            {
                if (this.PreviousPage == null) { return null; }
                else
                {
                    if (this.PreviousPage.IsHidden == false) { return this.PreviousPage; }
                    else { return this.PreviousPage.PreviousActivePage; }
                }
            }
        }
        public double Width
        {
            get { return this._width; }
            set
            {
                this._width = value;
                this.OnPropertyChanged(this, "Width");
            }
        }
        public double Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "Height");
            }
        }
        public string HeadingTitle
        {
            get { return this._headingTitle; }
            set
            {
                this._headingTitle = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }
        public string HeadingText
        {
            get { return this._headingText; }
            set
            {
                this._headingText = value;
                this.OnPropertyChanged(this, "HeadingText");
            }
        }
        public double HeadingHeight
        {
            get { return this._headingHeight; }
            set
            {
                this._headingHeight = value;
                this.OnPropertyChanged(this, "HeadingHeight");
            }
        }

        public SolidColorBrush HeadingBgColor
        {
            get { return this._headingBgColor; }
            set
            {
                this._headingBgColor = value;
                this.OnPropertyChanged(this, "HeadingBgColor");
            }
        }
        public SolidColorBrush HeadinFontColor
        {
            get { return this._headingFontColor; }
            set
            {
                this._headingFontColor = value;
                this.OnPropertyChanged(this, "HeadinFontColor");
            }
        }

        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this._gridlines = value;
                this.OnPropertyChanged(this, "ShowGridLines");
                foreach (TsColumn c in this._columns) { c.ShowGridLines = value; }
            }
        }
        public TsPage PreviousPage
        {
            get { return this._previouspage; }
            set
            {
                this._previouspage = value;
                this.Update();
            }
        }
        public TsPage NextPage
        {
            get { return this._nextpage; }
            set
            {
                this._nextpage = value;
                this.Update();
            }
        }
        public List<IGuiOption> Options { get { return this._options; } }
        public PageLayout Page { get { return this._pagelayout; } }
        public bool IsFirst
        {
            get { return this._isfirst; }
            set { this._isfirst = value; }
        }
        #endregion

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

        //Constructors
        public TsPage(XElement SourceXml, string HeadingTitle, string HeadingText, double Height,double Width, Thickness Margin, SolidColorBrush HeadingBgColor, SolidColorBrush HeadingTextColor, MainController RootController)
        {
            //Debug.WriteLine("New page constructor");
            //Debug.WriteLine(SourceXml);
            this._controller = RootController;
            this._pagelayout = new PageLayout(this);
            this._pagepanel = this._pagelayout.MainGrid;
            this.Height = Height;
            this.Width = Width;
            this._margin = Margin;
            this.HeadingHeight = 40;
            this.HeadingTitle = HeadingTitle;
            this.HeadingText = HeadingText;
            this.HeadinFontColor = HeadingTextColor;
            this.HeadingBgColor = HeadingBgColor;

            this._pagelayout.DataContext = this;
            this._pagepanel.SetBinding(Grid.IsEnabledProperty, new Binding("Enabled"));

            this.LoadXml(SourceXml);
            this.Build();
        }


        //Methods
        public void LoadXml(XElement InputXml)
        {
            IEnumerable<XElement> columnsXml;
            XElement x;
            string groupID = null;
            int colIndex = 0;

            x = InputXml.Element("Group");
            if (x != null)
            {
                groupID = x.Value;
                this._controller.AddToGroup(groupID, this);
            }

            //now read in the options and add to a dictionary for later use
            columnsXml = InputXml.Elements("Column");
            if (columnsXml != null)
            {
                foreach (XElement xColumn in columnsXml)
                {
                    TsColumn c = new TsColumn(xColumn, colIndex,this._controller);
                    this._columns.Add(c);
                    if (!string.IsNullOrEmpty(groupID)) { this._controller.AddToGroup(groupID, c); }
                    colIndex++;
                }
            }

            XElement headingX = InputXml.Element("Heading");
            if (headingX != null)
            {
                x = headingX.Element("Title");
                if (x != null) { this._headingTitle = x.Value; }

                x = headingX.Element("Text");
                if (x != null) { this._headingText = x.Value; }

                x = headingX.Element("Height");
                if (x != null) { this._headingHeight = Convert.ToInt32(x.Value); }

                x = headingX.Element("BgColor");
                if (x != null)
                {
                    this.HeadingBgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(x.Value));
                }

                x = headingX.Element("TextColor");
                if (x != null)
                {
                    this.HeadinFontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(x.Value));
                }
            }

            x = InputXml.Element("Width");
            if (x != null)
            { this.Width = Convert.ToInt32(x.Value); }

            x = InputXml.Element("Height");
            if (x != null)
            { this.Height = Convert.ToInt32(x.Value); }

            x = InputXml.Element("Enabled");
            if (x != null)
            { this.IsEnabled = Convert.ToBoolean(x.Value); }

            x = InputXml.Element("Hidden");
            if (x != null)
            { this.IsHidden = Convert.ToBoolean(x.Value); }

            this.PopulateOptions();
        }

        //build the gui controls.
        public void Build()
        {
            int colindex = 0;

            this._pagepanel.VerticalAlignment = VerticalAlignment.Top;
            this._pagepanel.HorizontalAlignment = HorizontalAlignment.Left;

            foreach (TsColumn col in this._columns)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                coldef.DataContext = col;
                coldef.SetBinding(ColumnDefinition.WidthProperty, new Binding("Width"));

                this._pagepanel.ColumnDefinitions.Add(coldef);

                Grid.SetColumn(col.Panel, colindex);
                this._pagepanel.Children.Add(col.Panel);
                colindex++;
            }

            this.Update();
        }

        //get all the options from the sub columns. this is parsed up the chain to generate the final
        //list of ts variables to set at the end. 
        private void PopulateOptions()
        {
            foreach (TsColumn col in this._columns)
            {
                this._options.AddRange(col.Options);
            }

            foreach (IGuiOption option in this._options)
            {
                if (option is IEditableGuiOption) { this._editables.Add((IEditableGuiOption)option); }
            }
        }

        public bool OptionsValid()
        {
            //Debug.WriteLine("OptionsValid called");
            foreach (IEditableGuiOption option in this._editables)
            {
                if (option.IsValid == false)
                {
                    //Debug.WriteLine("invalid option found");
                    return false;
                }
            }
            return true;
        }

        public void Cancel()
        {
            this._controller.Cancel();
        }

        public void MovePrevious()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.MovePrevious();
            }
        }

        public void MoveNext()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.MoveNext();
            }
        }

        public void Finish()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.Finish();
            }
        }

        //Update the prev, next, finish buttons according to the current pages 
        //place in the world
        public void Update()
        {
            if (this.NextActivePage != null)
            {
                this._pagelayout.buttonNext.Visibility = Visibility.Visible;
                this._pagelayout.buttonNext.IsEnabled = true;
                this._pagelayout.buttonFinish.Visibility = Visibility.Hidden;
                this._pagelayout.buttonFinish.IsEnabled = false;
            }

            if (this.PreviousActivePage != null)
            {
                this._pagelayout.buttonPrev.Visibility = Visibility.Visible;
                this._pagelayout.buttonPrev.IsEnabled = true;
            }
        }
    }
}
