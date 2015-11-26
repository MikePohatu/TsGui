using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace core
{
    public class Page: ITsGuiElement
    {
        private int height;
        private int width;
        private int padding;
        private List<Column> columns = new List<Column>();
        private PageWindow window = new PageWindow();

        public Page(XElement SourceXml,int Height,int Width,int Padding)
        {
            this.height = Height;
            this.width = Width;
            this.padding = Padding;

            this.LoadXml(SourceXml);
        }

        public void LoadXml(XElement SourceXml)
        {
            IEnumerable<XElement> columnsXml;
            //now read in the options and add to a dictionary for later use
            columnsXml = SourceXml.Elements("Column");
            if (columnsXml != null)
            {
                foreach (XElement xColumn in columnsXml)
                {
                    this.columns.Add(new Column(xColumn));
                }
            }
        }

        private void Build()
        {
            //this.window.
        }


    }
}
