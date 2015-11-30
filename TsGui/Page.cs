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
        private int height;
        private int width;
        private int padding;
        private List<Column> columns = new List<Column>();
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid pagepanel;
        //private MainWindow window;
        private bool islast = false;
        private bool isfirst = false;

        public List<IGuiOption> Options { get { return this.options; } }
        public Page PreviousPage { get; set; }
        public Page NextPage { get; set; }
        public Grid Panel { get { return this.pagepanel; } }
        public bool IsLast
        {
            get { return this.islast; }
            set { this.islast = value; }
        }
        public bool IsFirst
        {
            get { return this.isfirst; }
            set { this.isfirst = value; }
        }
        //public PageWindow Window { get { return this.window; } }

        public Page(XElement SourceXml,int Height,int Width,int Padding)
        {
            Debug.WriteLine("New page constructor");
            //Debug.WriteLine(SourceXml);

            this.height = Height;
            this.width = Width;
            this.padding = Padding;

            this.LoadXml(SourceXml);
            this.Build();
        }

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
                    this.columns.Add(new Column(xColumn, colIndex));
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

            this.pagepanel = new Grid();
            this.pagepanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            //this.pagepanel.ShowGridLines = true;

            //create a last row for the buttons
            RowDefinition colrowdef = new RowDefinition();
            this.pagepanel.RowDefinitions.Add(colrowdef);

            foreach (Column col in this.columns)
            {
                ColumnDefinition coldef = new ColumnDefinition();
                this.pagepanel.ColumnDefinitions.Add(coldef);

                Grid.SetColumn(col.Panel, colindex);
                this.pagepanel.Children.Add(col.Panel);
                colindex++;
            }
        }

        //get all the options from the sub columns. this is parsed up the chain to generate the final
        //list of ts variables to set at the end. 
        private void PopulateOptions()
        {
            foreach (Column col in this.columns)
            {
                this.options.AddRange(col.Options);
            }
        }

        //code to catch when a window is closed. warn the user, if ok, continue close and set
        //a variable to record the cancel. if mistake, open the last page
        public void CatchClose()
        {

        }
    }
}
