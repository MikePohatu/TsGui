using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace core
{
    public class Page: ITsGuiElement
    {
        public Page(XElement SourceXml)
        {
            this.LoadXml(SourceXml);
        }

        public void LoadXml(XElement SourceXml)
        {

        }


    }
}
