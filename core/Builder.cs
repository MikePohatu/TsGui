//'Root' class from which to spin everything off. TsGui or TsGui-Tester create this
//class to do the actual work. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

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

            XElement x = handler.Read(exefolder + @"\Config.xml");
        }

        public Builder(string ConfigPath)
        {
            //code to be added to make sure config file exists
            XElement x = handler.Read(ConfigPath);
        }


        private void LoadXml(XElement SourceXml)
        {
            XElement x;
            IEnumerable<XElement> pagesXml;

            x = SourceXml.Element("Width");
            if (x != null)
            { this.pageWidth = x.Element("Width").Value; }

            x = SourceXml.Element("Height");
            if (x != null)
            { this.pageHeight = x.Element("Height").Value; }

            //now read in the options and add to a dictionary for later use
            pagesXml = SourceXml.Elements("Page");
            if (pagesXml != null)
            {
                foreach (XElement xPage in pagesXml)
                {
                    this.pages.Add(new Page(xPage));
                }
            }
        }
    }
}
