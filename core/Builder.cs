//'Root' class from which to spin everything off. TsGui or TsGui-Tester create this
//class to do the actual work. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;

using System.Diagnostics;

namespace core
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

        //constructors
        public Builder()
        {
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            this.configpath = @"c:\Config.xml";
            StartBuilder();
        }

        public Builder(string ConfigPath)
        {
            this.configpath = ConfigPath;
            StartBuilder();
        }

        private void StartBuilder()
        {
            //code to be added to make sure config file exists
            XElement x = handler.Read(this.configpath);
            this.LoadXml(x);
            this.PopulateOptions();
            Page firstpage = this.pages.First();
            firstpage.Show();
        }

        private void LoadXml(XElement SourceXml)
        {
            XElement x;

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
                        Debug.WriteLine("Creating page");
                        this.pages.Add(new Page(xPage, this.pageHeight, this.pageWidth, this.pagePadding));
                    }
                }

                //Debug.WriteLine(this.pageHeight);
                //Debug.WriteLine(this.pageWidth);
                //Debug.WriteLine("Done");
            }
        }

        private void PopulateOptions()
        {
            foreach (Page pg in this.pages)
            {
                this.options.AddRange(pg.Options);
            }
        }
    }
}
