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

// ResultFormatter.cs - provides additional formatting/processing for a query result e.g.
// processing math on the result, prefix and postfix

using TsGui.Math;
using System.Xml.Linq;
using System;

namespace TsGui
{
    public class ResultFormatter
    {
        public string Name { get; set; }
        public string Input { get; set; }
        public int DecimalPlaces { get; set; }
        public string Calculation { get; set; }
        public string Append { get; set; }
        public string Prefix { get; set; }
        public string Value { get { return this.Process(); } }

        public ResultFormatter()
        {
            this.DecimalPlaces = -1;
        }

        public ResultFormatter(XElement InputXml)
        {
            this.DecimalPlaces = -1;
            this.LoadXml(InputXml);
        }


        private void LoadXml(XElement InputXml)
        {
            XElement x;
            XAttribute xattrib;

            xattrib = InputXml.Attribute("Name");
            if (xattrib != null) { this.Name = xattrib.Value; }

            x = InputXml.Element("Calculate");
            if (x != null)
            {
                this.Calculation = x.Value;

                xattrib = x.Attribute("DecimalPlaces");
                if (xattrib != null) { this.DecimalPlaces = Convert.ToInt32(xattrib.Value); }
            }

            x = InputXml.Element("Append");
            if (x != null) { this.Append = x.Value; }

            x = InputXml.Element("Prefix");
            if (x != null) { this.Prefix = x.Value; }
        }

        private string Process()
        {
            string s = this.Input;

            //if the input is empty, return 
            if (string.IsNullOrEmpty(s)) { return s; }

            //try any calculations
            try
            {
                if (!string.IsNullOrEmpty(this.Calculation))
                {
                    double result;
                    s = Calculation.Replace("VALUE", this.Input);
                    result = Calculator.CalculateString(s);

                    if (this.DecimalPlaces != -1)
                    { result = System.Math.Round(result, this.DecimalPlaces); }

                    s = result.ToString();
                }
            }
            // if there is an error in the calculation e.g. if a non-numeric string is the input, set
            // s to the input value
            catch { s = this.Input; }

            s = this.Prefix + s + this.Append;

            return s;
        }
    }
}
