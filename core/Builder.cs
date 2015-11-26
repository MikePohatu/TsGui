//'Root' class from which to spin everything off. TsGui or TsGui-Tester create this
//class to do the actual work. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;
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

        //constructors
        public Builder()
        {
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            configpath = @"c:\Config.xml";
            //configpath = exefolder + @"\Config.xml"

            XElement x = handler.Read(configpath);
            this.LoadXml(x);
        }

        public Builder(string ConfigPath)
        {
            //code to be added to make sure config file exists
            XElement x = handler.Read(ConfigPath);
            this.LoadXml(x);
        }


        private void LoadXml(XElement SourceXml)
        {
            XElement x;
            XElement tsguiXml;
            IEnumerable<XElement> pagesXml;

            tsguiXml = SourceXml.Element("TsGui");
            if (tsguiXml != null)
            {

                x = tsguiXml.Element("Width");
                if (x != null)
                { this.pageWidth = Convert.ToInt32(x.Element("Width").Value); }

                x = tsguiXml.Element("Height");
                if (x != null)
                { this.pageHeight = Convert.ToInt32(x.Element("Height").Value); }

                x = tsguiXml.Element("Padding");
                if (x != null)
                { this.pagePadding = Convert.ToInt32(x.Element("Padding").Value); }

                //now read in the options and add to a dictionary for later use
                pagesXml = tsguiXml.Elements("Page");
                if (pagesXml != null)
                {
                    foreach (XElement xPage in pagesXml)
                    {
                        Debug.WriteLine("Creating page");
                        this.pages.Add(new Page(xPage, this.pageHeight, this.pageWidth, this.pagePadding));
                    }
                }
            }
        }
    }
}
