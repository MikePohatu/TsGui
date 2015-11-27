using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Diagnostics;

namespace core
{
    public class Page: ITsGuiElement
    {
        private int height;
        private int width;
        private int padding;
        private List<Column> columns = new List<Column>();
        private List<IGuiOption> options = new List<IGuiOption>();
        private PageWindow window = new PageWindow();

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
            //now read in the options and add to a dictionary for later use
            columnsXml = SourceXml.Elements("Column");
            if (columnsXml != null)
            {
                foreach (XElement xColumn in columnsXml)
                {
                    this.columns.Add(new Column(xColumn));
                }
            }

            this.PopulateOptions();
        }

        private void Build()
        {
            //this.window.
            foreach (IGuiOption guiopt in this.options)
            {
                window.AddControl(guiopt.Control);
            }
        }

        private void PopulateOptions()
        {
            foreach (Column col in this.columns)
            {
                this.options.AddRange(col.Options);
            }
        }

        public void Show()
        {
            //this.window.
        }
    }
}
