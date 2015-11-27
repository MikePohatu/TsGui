using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace gui_lib
{
    public interface ITsGuiElement
    {
        void LoadXml(XElement Xml);
    }
}
