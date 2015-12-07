//factory class just to generate the right class from the xml based on type

using System;
using System.Xml.Linq;

namespace TsGui
{
    public static class GuiFactory
    {
        public static IGuiOption CreateGuiOption(XElement OptionXml)
        {
            //need to update with factory
            if (OptionXml.Attribute("Type").Value == "DropDownList")
            {
                return new TsDropDownList(OptionXml);
            }

            else if (OptionXml.Attribute("Type").Value == "FreeText")
            {
                return new TsFreeText(OptionXml);
            }

            else if (OptionXml.Attribute("Type").Value == "CheckBox")
            {
                return new TsCheckBox(OptionXml);
            }

            else { return null; }
        }
    }
}
