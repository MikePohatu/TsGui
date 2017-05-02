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
using TsGui.Diagnostics.Logging;

namespace TsGui.View.GuiOptions
{
    public static class GuiFactory
    {
        public static IGuiOption CreateGuiOption(XElement OptionXml, TsColumn Parent, IDirector RootController)
        {
            XAttribute xtype = OptionXml.Attribute("Type");
            if (xtype == null) { throw new ArgumentException("Missing Type attribute on GuiOption" + Environment.NewLine); }

            LoggerFacade.Info("Creating GuiOption, type: " + xtype.Value);

            #region
            if (xtype.Value == "DropDownList")
            {
                TsDropDownList ddl = new TsDropDownList(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(ddl);
                return ddl;
            }

            else if (xtype.Value == "CheckBox")
            {
                TsCheckBox cb = new TsCheckBox(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(cb);
                return cb;
            }
            else if (xtype.Value == "FreeText")
            {
                TsFreeText ft = new TsFreeText(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(ft);
                return ft;
            }

            else if (xtype.Value == "ComputerName")
            {
                TsComputerName cn = new TsComputerName(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(cn);
                return cn;
            }
            else if (xtype.Value == "Heading")
            {
                TsHeading h = new TsHeading(OptionXml, Parent, RootController);
                return h;
            }
            else if (xtype.Value == "InfoBox")
            {
                TsInfoBox ib = new TsInfoBox(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(ib);
                return ib;
            }
            else if (xtype.Value == "TrafficLight")
            {
                TsTrafficLight tl = new TsTrafficLight(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(tl);
                return tl;
            }
            else if (xtype.Value == "TickCross")
            {
                TsTickCross option = new TsTickCross(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(option);
                return option;
            }
            else if (xtype.Value == "Image")
            {
                TsImage option = new TsImage(OptionXml, Parent, RootController);
                return option;
            }
            else if (xtype.Value == "ComplianceRefreshButton")
            {
                TsComplianceRefreshButton option = new TsComplianceRefreshButton(OptionXml, Parent, RootController);
                return option;
            }
            else if (xtype.Value == "PasswordBox")
            {
                TsPasswordBox option = new TsPasswordBox(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(option);
                return option;
            }
            else if (xtype.Value == "UsernameBox")
            {
                TsUsernameBox option = new TsUsernameBox(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(option);
                return option;
            }
            else if (xtype.Value == "ActionButton")
            {
                TsActionButton option = new TsActionButton(OptionXml, Parent, RootController);
                RootController.AddOptionToLibary(option);
                return option;
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
