using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace core
{
    public interface ITsGuiElement
    {
        void LoadXml(XElement Xml);
    }
}
