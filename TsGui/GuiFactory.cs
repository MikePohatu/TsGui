//factory class just to generate the right class from the xml based on type

using System;
using System.Xml.Linq;
using System.Windows;

namespace TsGui
{
    public static class GuiFactory
    {
        public static IGuiOption CreateGuiOption(XElement OptionXml,Controller RootController)
        {
            #region
            //need to update with factory
            if (OptionXml.Attribute("Type").Value == "DropDownList")
            {
                return new TsDropDownList(OptionXml);
            }

            else if (OptionXml.Attribute("Type").Value == "FreeText")
            {
                return new TsFreeText(OptionXml,RootController);
            }

            else if (OptionXml.Attribute("Type").Value == "CheckBox")
            {
                return new TsCheckBox(OptionXml);
            }

            else if (OptionXml.Attribute("Type").Value == "Heading")
            {
                return new TsHeading(OptionXml);
            }

            else
            { return null; }
            #endregion
        }


        //pass in the xml and set the thickness according to the xml values
        public static void LoadMargins(XElement InputXml, Thickness Margin)
        {
            #region
            XElement x;

            //all-in-one margin settings
            x = InputXml.Element("Margin");
            if (x != null)
            {
                int i = Convert.ToInt32(x.Value);
                Margin.Top = i;
                Margin.Bottom = i;
                Margin.Right = i;
                Margin.Left = i;
            }

            x = InputXml.Element("LMargin");
            if (x != null)
            {
                int i = Convert.ToInt32(x.Value);
                Margin.Left = i;
            }

            x = InputXml.Element("RMargin");
            if (x != null)
            {
                int i = Convert.ToInt32(x.Value);
                Margin.Right = i;
            }

            x = InputXml.Element("TMargin");
            if (x != null)
            {
                int i = Convert.ToInt32(x.Value);
                Margin.Top = i;
            }

            x = InputXml.Element("BMargin");
            if (x != null)
            {
                int i = Convert.ToInt32(x.Value);
                Margin.Bottom = i;
            }
            #endregion
        }


        public static void LoadHAlignment(XElement InputXml, ref HorizontalAlignment HAlign)
        {
            #region
            XElement x;
            x = InputXml.Element("HAlign");
            if (x != null)
            {
                if (x.Value.ToUpper() == "LEFT")
                {
                    HAlign = HorizontalAlignment.Left;
                }
                else if (x.Value.ToUpper() == "RIGHT")
                {
                    HAlign = HorizontalAlignment.Right;
                }
                else if (x.Value.ToUpper() == "CENTER")
                {
                    HAlign = HorizontalAlignment.Center;
                }
            }
            #endregion
        }

        public static void LoadVAlignment(XElement InputXml, ref VerticalAlignment VAlign)
        {
            #region
            XElement x;
            x = InputXml.Element("VAlign");
            if (x != null)
            {
                if (x.Value.ToUpper() == "TOP")
                {
                    VAlign = VerticalAlignment.Top;
                }
                else if (x.Value.ToUpper() == "BOTTOM")
                {
                    VAlign = VerticalAlignment.Bottom;
                }
                else if (x.Value.ToUpper() == "CENTER")
                {
                    VAlign = VerticalAlignment.Center;
                }
            }
            #endregion
        }
    }
}
