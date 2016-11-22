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

//    XmlHandler.cs: Handles reading and writing of XML documents

using System;
using System.IO;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;

namespace TsGui
{
    public static class XmlHandler
    {
        public static void Write(string pPath, XElement pElement)
        {
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), pElement);
            string dirPath = Path.GetDirectoryName(pPath);
            if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); }
            xDoc.Save(pPath);
        }

        public static XElement Read(string pPath)
        {
            //LoadOptions options = new LoadOptions;
            //LoadOptions.PreserveWhitespace = true;
            XElement temp = null;
            if (!File.Exists(pPath)) 
            { throw new FileNotFoundException("File not found: " + pPath); }
            try { temp = XElement.Load(pPath); }
            catch (Exception e)
            { throw new InvalidOperationException("Unable to read xml file: " + pPath + Environment.NewLine + e.Message); }

            return temp;
        }


        //XElement functions
        public static string GetStringFromXElement(XElement InputXml, string XName, string DefaultValue)
        {
            XElement x;

            x = InputXml.Element(XName);
            if (x != null) { return x.Value; }
            else { return DefaultValue; }
        }

        public static int GetIntFromXElement(XElement InputXml, string XName, int DefaultValue)
        {
            XElement x;

            x = InputXml.Element(XName);
            if (x != null) { return Convert.ToInt32(x.Value); }
            else { return DefaultValue; }
        }

        public static double GetDoubleFromXElement(XElement InputXml, string XName, double DefaultValue)
        {
            XElement x;

            x = InputXml.Element(XName);
            if (x != null)
            {
                if (x.Value.ToUpper() == "AUTO") { return Double.NaN; }
                else { return Convert.ToDouble(x.Value); }
            }
            else { return DefaultValue; }
        }

        public static GridLength GetGridLengthFromXElement(XElement InputXml, string XName, GridLength DefaultValue)
        {
            XElement x;

            x = InputXml.Element(XName);
            if (x != null) { return new GridLength(Convert.ToDouble(x.Value)); }
            else { return DefaultValue; }
        }

        public static bool GetBoolFromXElement(XElement InputXml, string XName, bool DefaultValue)
        {
            XElement x;

            x = InputXml.Element(XName);
            if (x != null) { return Convert.ToBoolean(x.Value); }
            else { return DefaultValue; }
        }

        public static Thickness GetThicknessFromXElement(XElement InputXml, string XName, Thickness DefaultValue)
        {
            XElement x;
            string[] splitstring;

            x = InputXml.Element(XName);
            if (x != null)
            {
                splitstring = x.Value.Split(',');
                if (splitstring.Length == 1)
                {
                    return new Thickness(Convert.ToDouble(splitstring[0]));
                }
                else if (splitstring.Length == 4)
                {
                    double left = Convert.ToDouble(splitstring[0]);
                    double top = Convert.ToDouble(splitstring[1]);
                    double right = Convert.ToDouble(splitstring[2]);
                    double bottom = Convert.ToDouble(splitstring[3]);
                    return new Thickness(left,top,right,bottom);
                }
                else { throw new InvalidDataException("Invalid thickness in element " + XName + ": " + x.Value); }
            }
            else { return DefaultValue; }
        }

        public static Color GetColorFromXElement(XElement InputXml, string XName, Color DefaultValue)
        {
            XElement x = InputXml.Element(XName);
            if (x != null) { return (Color)ColorConverter.ConvertFromString(x.Value); }
            else { return DefaultValue; }            
        }

        public static SolidColorBrush GetSolidColorBrushFromXElement(XElement InputXml, string XName, SolidColorBrush DefaultValue)
        {
            XElement x = InputXml.Element(XName);
            if (x != null) { return new SolidColorBrush((Color)ColorConverter.ConvertFromString(x.Value)); }
            else { return DefaultValue; }
        }

        public static WindowStartupLocation GetWindowStartupLocationFromXElement(XElement InputXml, string XName, WindowStartupLocation DefaultValue)
        {
            XElement x = InputXml.Element(XName);
            if (x != null)
            {
                if (x.Value.ToUpper() == "MANUAL") { return WindowStartupLocation.Manual; }
                else if (x.Value.ToUpper() == "CENTEROWNER") { return WindowStartupLocation.CenterOwner; }
                else { return WindowStartupLocation.CenterScreen; }
            }
            else { return DefaultValue; }
        }

        //XAttribute functions
        public static string GetStringFromXAttribute(XElement InputXml, string XName, string DefaultValue)
        {
            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null) { return x.Value; }
            else { return DefaultValue; }
        }

        public static int GetIntFromXAttribute(XElement InputXml, string XName, int DefaultValue)
        {
            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null) { return Convert.ToInt32(x.Value); }
            else { return DefaultValue; }
        }

        public static double GetDoubleFromXAttribute(XElement InputXml, string XName, double DefaultValue)
        {
            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null)
            {
                if (x.Value.ToUpper() == "AUTO") { return Double.NaN; }
                else { return Convert.ToDouble(x.Value); }
            }
            else { return DefaultValue; }
        }

        public static bool GetBoolFromXAttribute(XElement InputXml, string XName, bool DefaultValue)
        {
            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null) { return Convert.ToBoolean(x.Value); }
            else { return DefaultValue; }
        }

        public static VerticalAlignment GetVerticalAlignmentFromXElement(XElement InputXml, string XName, VerticalAlignment DefaultValue)
        {
            XElement x;
            x = InputXml.Element(XName);
            if (x != null)
            {
                switch (x.Value.ToUpper())
                {
                    case "TOP":
                        return VerticalAlignment.Top;
                    case "BOTTOM":
                        return VerticalAlignment.Bottom;
                    case "CENTER":
                        return VerticalAlignment.Center;
                    case "STRETCH":
                        return VerticalAlignment.Stretch;
                    default:
                        return VerticalAlignment.Bottom;
                }
            }
            else { return DefaultValue; }
        }

        public static HorizontalAlignment GetHorizontalAlignmentFromXElement(XElement InputXml, string XName, HorizontalAlignment DefaultValue)
        {
            XElement x;
            x = InputXml.Element(XName);
            if (x != null)
            {
                switch (x.Value.ToUpper())
                {
                    case "LEFT":
                        return HorizontalAlignment.Left;
                    case "RIGHT":
                        return HorizontalAlignment.Right;
                    case "CENTER":
                        return HorizontalAlignment.Center;
                    case "STRETCH":
                        return HorizontalAlignment.Stretch;
                    default:
                        return HorizontalAlignment.Left;
                }
            }
            else { return DefaultValue; }
        }

        public static TextAlignment GetTextAlignmentFromXElement(XElement InputXml, string XName, TextAlignment DefaultValue)
        {
            XElement x;
            x = InputXml.Element(XName);
            if (x != null)
            {
                switch (x.Value.ToUpper())
                {
                    case "LEFT":
                        return TextAlignment.Left;
                    case "RIGHT":
                        return TextAlignment.Right;
                    case "CENTER":
                        return TextAlignment.Center;
                    default:
                        return TextAlignment.Left;
                }
            }
            else { return DefaultValue; }
        }
    }
}
