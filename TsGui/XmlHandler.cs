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

//    XmlHandler.cs: Handles reading and writing of XML documents

using System;
using System.IO;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;

using TsGui.Validation;
using Core.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace TsGui
{
    public static class XmlHandler
    {
        private static readonly HttpClient _client = new HttpClient();

        private static string GetXmlValue(XElement InputXml, string XName)
        {
            XAttribute xa = InputXml.Attribute(XName);
            if (xa != null) { return xa.Value; }
            else
            {
                XElement x = InputXml.Element(XName);
                if (x != null) { return x.Value; }
            }
            return null;
        }

        public static void Write(string pPath, XElement pElement)
        {
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), pElement);
            string dirPath = Path.GetDirectoryName(pPath);
            if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); }
            xDoc.Save(pPath);
        }

        /// <summary>
        /// Sync read XML from a local file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static XElement Read(string filepath)
        {
            //LoadOptions options = new LoadOptions;
            //LoadOptions.PreserveWhitespace = true;
            XElement temp = null;
            if (!File.Exists(filepath)) 
            { throw new FileNotFoundException("File not found: " + filepath); }
            try { temp = XElement.Load(filepath); }
            catch (Exception e)
            { throw new KnownException("Unable to read xml file: " + filepath, e.Message); }

            return temp;
        }

        /// <summary>
        /// Sync read XML from a web URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static XElement ReadWeb(string url)
        {
            XElement xconfig = null;

            try
            {
                xconfig = XElement.Load(url);
            }
            catch (Exception e)
            {
                throw new KnownException("Error downloading web config: " + url, e.Message);
            }

            return xconfig;
        }

        /// <summary>
        /// Async read XML from a web URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<XElement> ReadWebAsync(string url)
        {
            XElement xconfig = null;

            try
            {
                    HttpResponseMessage response = await _client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    xconfig = XElement.Parse(responseBody);
            }
            catch (Exception e)
            {
                throw new KnownException("Error downloading web config: " + url, e.Message);
            }

            return xconfig;
        }

        //XElement functions
        public static string GetStringFromXml(XElement InputXml, string XName, string DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            return value;
        }

        public static int GetIntFromXml(XElement InputXml, string XName, int DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }
            
            return Convert.ToInt32(value);
        }

        public static int GetComplianceStateValueFromXml(XElement InputXml, string XName, int DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            switch (value)
            {
                case "OK":
                    return ComplianceStateValues.OK;
                case "Warning":
                    return ComplianceStateValues.Warning;
                case "Error":
                    return ComplianceStateValues.Error;
                case "Invalid":
                    return ComplianceStateValues.Invalid;
                default:
                    return DefaultValue;
            }
        }

        public static double GetDoubleFromXml(XElement InputXml, string XName, double DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            if (value.ToUpper() == "AUTO") { return Double.NaN; }
            else { return Convert.ToDouble(value); }
        }

        public static GridLength GetGridLengthFromXml(XElement InputXml, string XName, GridLength DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            return new GridLength(Convert.ToDouble(value));
        }

        /// <summary>
        /// Convert XML value to bool. If value is null or empty returns true (assumes element of form <Element />
        /// </summary>
        /// <param name="InputXml"></param>
        /// <param name="XName"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static bool GetBoolFromXml(XElement InputXml, string XName, bool DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            return Convert.ToBoolean(value);
        }

        public static Thickness GetThicknessFromXml(XElement InputXml, string XName, Thickness DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }


            string[] splitstring;
            splitstring = value.Split(',');
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
            else { throw new InvalidDataException("Invalid thickness in element " + XName + ": " + value); }
        }

        public static Color GetColorFromXml(XElement InputXml, string XName, Color DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            return (Color)ColorConverter.ConvertFromString(value);
        }

        public static SolidColorBrush GetSolidColorBrushFromXml(XElement InputXml, string XName, SolidColorBrush DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }
            
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
        }

        public static WindowStartupLocation GetWindowStartupLocationFromXml(XElement InputXml, string XName, WindowStartupLocation DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            if (value.ToUpper() == "MANUAL") { return WindowStartupLocation.Manual; }
            else if (value.ToUpper() == "CENTEROWNER") { return WindowStartupLocation.CenterOwner; }
            else { return WindowStartupLocation.CenterScreen; }
        }

        public static VerticalAlignment GetVerticalAlignmentFromXml(XElement InputXml, string XName, VerticalAlignment DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            switch (value.ToUpper())
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

        public static HorizontalAlignment GetHorizontalAlignmentFromXml(XElement InputXml, string XName, HorizontalAlignment DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            switch (value.ToUpper())
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

        public static TextAlignment GetTextAlignmentFromXml(XElement InputXml, string XName, TextAlignment DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            switch (value.ToUpper())
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

        public static Stretch GetStretchFromXml(XElement InputXml, string XName, Stretch DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }
            string value = GetXmlValue(InputXml, XName);
            if (string.IsNullOrEmpty(value)) { return DefaultValue; }

            switch (value.ToUpper())
            {
                case "FILL":
                    return Stretch.Fill;
                case "NONE":
                    return Stretch.None;
                case "UNIFORM":
                    return Stretch.Uniform;
                case "UNIFORMTOFILL":
                    return Stretch.UniformToFill;
                default:
                    return Stretch.None;
            }
        }
    }
}
