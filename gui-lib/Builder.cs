//'Root' class from which to spin everything off. TsGui or TsGui-Tester create this
//class to do the actual work. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Windows;

using System.Diagnostics;

namespace gui_lib
{
    public class Builder
    {
        private string configpath;
        private int pageWidth;
        private int pageHeight;
        private int pagePadding;

        private XmlHandler handler = new XmlHandler();
        private List<Page> pages = new List<Page>();
        private List<IGuiOption> options = new List<IGuiOption>();

        //properties
        public Window ParentWindow { get; set; }

        //constructors
        public Builder()
        {
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            this.configpath = @"c:\Config.xml";
        }

        public Builder(string ConfigPath)
        {
            this.configpath = ConfigPath;
        }

        public void Start()
        {
            //code to be added to make sure config file exists
            XElement x = handler.Read(this.configpath);
            this.LoadXml(x);
            this.PopulateOptions();

            //now show the first page in the list
            Page firstpage = this.pages.First();
            firstpage.Show();
        }

        private void LoadXml(XElement SourceXml)
        {
            XElement x;
            Page currPage = null;
            Page prevPage = null;

            IEnumerable<XElement> pagesXml;
            //Debug.WriteLine("Source: " + SourceXml);

            Debug.WriteLine("Starting xml load in builder");

            if (SourceXml != null)
            {

                x = SourceXml.Element("Width");
                if (x != null)
                { this.pageWidth = Convert.ToInt32(x.Value); }

                x = SourceXml.Element("Height");
                if (x != null)
                { this.pageHeight = Convert.ToInt32(x.Value); }

                x = SourceXml.Element("Padding");
                if (x != null)
                { this.pagePadding = Convert.ToInt32(x.Value); }

                //now read in the options and add to a dictionary for later use
                pagesXml = SourceXml.Elements("Page");
                if (pagesXml != null)
                {
                    Debug.WriteLine("pagesXml not null");
                    foreach (XElement xPage in pagesXml)
                    {
                        Debug.WriteLine("creating new page");
                        if (currPage == null)
                        {
                            currPage = new Page(xPage, this.pageHeight, this.pageWidth, this.pagePadding);
                            currPage.IsFirst();
                        }
                        else
                        {
                            //record the last page as the prevPage
                            prevPage = currPage;
                            currPage = new Page(xPage, this.pageHeight, this.pageWidth, this.pagePadding);                 
                        }
                        
                        //create the new page and assign the next page/prev page links
                        currPage.PreviousPage = prevPage;
                        if (prevPage != null) { prevPage.NextPage = currPage; }

                        currPage.Window.buttonCancel.Click += this.buttonCancel_Click;

                        this.pages.Add(currPage);
                    }

                    currPage.IsLast();
                    currPage.Window.buttonNext.Click += this.buttonFinish_Click;
                }
            }
        }

        private void PopulateOptions()
        {
            foreach (Page pg in this.pages)
            {
                this.options.AddRange(pg.Options);
            }
        }

        //method to deal with the cancel button being pressed on any page
        public void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            foreach (Page pg in this.pages)
            { pg.Window.Close(); }
            this.ParentWindow.Close();
        }

        //method to deal with the finish button being pressed on the last page
        public void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.MessageBox.Show("Finish clicked");
            foreach (Page pg in this.pages)
            { pg.Window.Close(); }

            this.ParentWindow.Close();
        }
    }
}
