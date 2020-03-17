#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

//  GuiFactory.cs
//  Factory class just to generate the right class from the xml based on type

using System;
using System.Xml.Linq;
using System.Windows;
using TsGui.Diagnostics.Logging;
using TsGui.Diagnostics;
using TsGui.View.GuiOptions.CollectionViews;
using TsGui.Prebuilt;

namespace TsGui.View.GuiOptions
{
    public static class GuiFactory
    {
        public static IGuiOption CreateGuiOption(XElement OptionXml, TsColumn Parent)
        {
            
            XElement prebuiltx = PrebuiltFactory.GetPrebuiltXElement(OptionXml);
            if (prebuiltx == null)
            {
                return GetGuiOption(OptionXml, Parent);
            }
            else
            {
                XAttribute xtype = OptionXml.Attribute("Type");
                prebuiltx.Add(xtype);
                IGuiOption g = GetGuiOption(prebuiltx, Parent);
                g.LoadXml(OptionXml);
                return g;
            }
        }


        private static IGuiOption GetGuiOption(XElement OptionXml, TsColumn Parent)
        {
            XAttribute xtype = OptionXml.Attribute("Type");
            if (xtype == null) { throw new ArgumentException("Missing Type attribute on GuiOption" + Environment.NewLine); }

            LoggerFacade.Info("Creating GuiOption, type: " + xtype.Value);

            IGuiOption newoption = null;

            #region
            if (xtype.Value == "DropDownList")
            {
                newoption = new TsDropDownList(OptionXml, Parent);
            }

            else if (xtype.Value == "CheckBox")
            {
                newoption = new TsCheckBox(OptionXml, Parent);
            }
            else if (xtype.Value == "FreeText")
            {
                newoption = new TsFreeText(OptionXml, Parent);
            }

            else if (xtype.Value == "ComputerName")
            {
                newoption = new TsComputerName(OptionXml, Parent);
            }
            else if (xtype.Value == "Heading")
            {
                newoption = new TsHeading(OptionXml, Parent);
                return newoption;
            }
            else if (xtype.Value == "InfoBox")
            {
                TsInfoBox ib = new TsInfoBox(OptionXml, Parent);
            }
            else if (xtype.Value == "TrafficLight")
            {
                newoption = new TsTrafficLight(OptionXml, Parent);
            }
            else if (xtype.Value == "TickCross")
            {
                newoption = new TsTickCross(OptionXml, Parent);
            }
            else if (xtype.Value == "Image")
            {
                newoption = new TsImage(OptionXml, Parent);
                return newoption;
            }
            else if (xtype.Value == "ComplianceRefreshButton")
            {
                newoption = new TsComplianceRefreshButton(OptionXml, Parent);
                return newoption;
            }
            else if (xtype.Value == "PasswordBox")
            {
                newoption = new TsPasswordBox(OptionXml, Parent);
            }
            else if (xtype.Value == "UsernameBox")
            {
                newoption = new TsUsernameBox(OptionXml, Parent);
            }
            else if (xtype.Value == "ActionButton")
            {
                newoption = new TsActionButton(OptionXml, Parent);
            }
            else if (xtype.Value == "TreeView")
            {
                newoption = new TsTreeView(OptionXml, Parent);
            }
            else if (xtype.Value == "Timeout")
            {
                if (GuiTimeout.Instance == null) { throw new TsGuiKnownException("No Timeout section defined in config. Timeout GuiOption type not available", string.Empty); }
                newoption = new TsTimeout(OptionXml, Parent);
            }
            else
            { return null; }
            #endregion

            Director.Instance.AddOptionToLibary(newoption);
            return newoption;
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
