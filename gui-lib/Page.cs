using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace gui_lib
{
    public class Page: ITsGuiElement
    {
        private int height;
        private int width;
        private int padding;
        private List<Column> columns = new List<Column>();
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid pagepanel;
        private PageWindow window;
        private Page prevPage;
        private Page nextPage;
        //private Button nextButton = new Button();
        //private Button prevButton = new Button();
        //private Button cancelButton = new Button();
        private int buttonHeight = 20;
        private int buttonWidth = 75;

        public List<IGuiOption> Options { get { return this.options; } }

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
            this.window = new PageWindow(this.height,this.width,this.padding);
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

            //create a last row for the buttons
            //RowDefinition butrowdef = new RowDefinition();
            //this.pagepanel.RowDefinitions.Add(butrowdef);

            //this.cancelButton.Width = this.buttonWidth;
            //this.nextButton.Width = this.buttonWidth;
            //this.prevButton.Width = this.buttonWidth;

            //this.cancelButton.Height = this.buttonHeight;
            //this.nextButton.Height = this.buttonHeight;
            //this.prevButton.Height = this.buttonHeight;

            //this.cancelButton.HorizontalAlignment = HorizontalAlignment.Left;
            //this.nextButton.HorizontalAlignment = HorizontalAlignment.Right;

            //this.prevButton.Content = "Back";
            //this.cancelButton.Content = "Cancel";

            //if (this.nextPage == null) { this.nextButton.Content = "Finish"; }
            //else { this.nextButton.Content = "Next"; }

            //if (this.prevPage == null) { this.prevButton.IsEnabled = false; }

            //now place the buttons
            //Grid.SetColumn(this.cancelButton, 0);
            //Grid.SetColumn(this.prevButton, this.pagepanel.ColumnDefinitions.Count);
            //Grid.SetColumn(this.nextButton, this.pagepanel.ColumnDefinitions.Count);
            //Grid.SetRow(this.cancelButton, this.pagepanel.RowDefinitions.Count);
            //Grid.SetRow(this.prevButton, this.pagepanel.RowDefinitions.Count);
            //Grid.SetRow(this.nextButton, this.pagepanel.RowDefinitions.Count);
            
            //this.pagepanel.Children.Add(this.cancelButton);
            //this.pagepanel.Children.Add(this.prevButton);
            //this.pagepanel.Children.Add(this.nextButton);

            this.window.AddPanel(this.pagepanel);
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


        public void Show()
        {
            this.window.Show();
        }

        //code to catch when a window is closed. warn the user, if ok, continue close and set
        //a variable to record the cancel. if mistake, open the last page
        public void CatchClose()
        {

        }
    }
}
