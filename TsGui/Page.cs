using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace TsGui
{
    public class Page: ITsGuiElement
    {
        private Controller _controller;
        private int _height;
        private int _width;

        private Thickness _margin = new Thickness(0, 0, 0, 0);
        private List<Column> _columns = new List<Column>();
        private List<IGuiOption> _options = new List<IGuiOption>();
        private Grid _pagepanel;

        private bool _islast = false;
        private bool _isfirst = false;

        public bool ShowGridlines { get; set; }
        public List<IGuiOption> Options { get { return this._options; } }
        public Page PreviousPage { get; set; }
        public Page NextPage { get; set; }
        public Grid Panel { get { return this._pagepanel; } }
        public bool IsLast
        {
            get { return this._islast; }
            set { this._islast = value; }
        }
        public bool IsFirst
        {
            get { return this._isfirst; }
            set { this._isfirst = value; }
        }


        //Constructors
        public Page(XElement SourceXml,int Height,int Width,Thickness Margin,Controller RootController)
        {
            Debug.WriteLine("New page constructor");
            //Debug.WriteLine(SourceXml);
            this._controller = RootController;
            this._height = Height;
            this._width = Width;
            this._margin = Margin;
            this.ShowGridlines = false;

            this.LoadXml(SourceXml);
            this.Build();
        }


        //Methods
        public void LoadXml(XElement SourceXml)
        {
            IEnumerable<XElement> columnsXml;
            int colIndex = 0;

            //now read in the options and add to a dictionary for later use
            columnsXml = SourceXml.Elements("Column");
            if (columnsXml != null)
            {
                foreach (XElement xColumn in columnsXml)
                {
                    Column c = new Column(xColumn, colIndex,this._controller);
                    c.ShowGridLines = this.ShowGridlines;
                    this._columns.Add(c);
                    colIndex++;
                }
            }

            this.PopulateOptions();
        }

        //build the gui controls.
        private void Build()
        {
            int colindex = 0;
            //this.window = new PageWindow(this.height,this.width,this.padding);

            this._pagepanel = new Grid();
            this._pagepanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this._pagepanel.ShowGridLines = this.ShowGridlines;
            //this.pagepanel.ShowGridLines = true;

            //create a last row for the buttons
            RowDefinition colrowdef = new RowDefinition();
            this._pagepanel.RowDefinitions.Add(colrowdef);

            foreach (Column col in this._columns)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                this._pagepanel.ColumnDefinitions.Add(coldef);

                Grid.SetColumn(col.Panel, colindex);
                this._pagepanel.Children.Add(col.Panel);
                colindex++;
            }
        }

        //get all the options from the sub columns. this is parsed up the chain to generate the final
        //list of ts variables to set at the end. 
        private void PopulateOptions()
        {
            foreach (Column col in this._columns)
            {
                this._options.AddRange(col.Options);
            }
        }

        //code to catch when a window is closed. warn the user, if ok, continue close and set
        //a variable to record the cancel. if mistake, open the last page
        public void CatchClose()
        {

        }
    }
}
