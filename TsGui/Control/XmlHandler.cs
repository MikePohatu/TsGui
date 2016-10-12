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
            catch { throw new InvalidOperationException("Unable to read xml file: " + pPath); }

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
            if (x != null) { return Convert.ToDouble(x.Value); }
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

        public static Thickness GetThicknessFromXElement(XElement InputXml, string XName, int DefaultValue)
        {
            XElement x;
            int i;

            x = InputXml.Element(XName);
            if (x != null) { i = Convert.ToInt32(x.Value); }
            else { i = DefaultValue; }

            return new Thickness(i, i, i, i);
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
            if (x != null) { return Convert.ToDouble(x.Value); }
            else { return DefaultValue; }
        }

        public static bool GetBoolFromXAttribute(XElement InputXml, string XName, bool DefaultValue)
        {
            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null) { return Convert.ToBoolean(x.Value); }
            else { return DefaultValue; }
        }
    }
}
