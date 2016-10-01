//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

//  GuiFactory.cs
//  Factory class just to generate the right class from the xml based on type

using System;
using System.Xml.Linq;
using System.Windows;

namespace TsGui
{
    public static class GuiFactory
    {
        public static IGuiOption CreateGuiOption(XElement OptionXml,TsPage ParentPage, MainController RootController)
        {
            #region
            if (OptionXml.Attribute("Type").Value == "DropDownList")
            {
                TsDropDownList ddl = new TsDropDownList(OptionXml, RootController);
                ParentPage.PageWindowLoaded += ddl.OnParentWindowLoaded;
                return ddl;

            }

            else if (OptionXml.Attribute("Type").Value == "ComputerName")
            {
                return new TsComputerName(OptionXml, RootController);
            }

            else if (OptionXml.Attribute("Type").Value == "FreeText")
            {
                return new TsFreeText(OptionXml,RootController);
            }

            else if (OptionXml.Attribute("Type").Value == "CheckBox")
            {
                return new TsCheckBox(OptionXml, RootController);
            }

            else if (OptionXml.Attribute("Type").Value == "Heading")
            {
                return new TsHeading(OptionXml, RootController);
            }

            else if (OptionXml.Attribute("Type").Value == "InfoBox")
            {
                return new TsInfoBox(OptionXml, RootController);
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
