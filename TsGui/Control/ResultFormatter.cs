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

// ResultFormatter.cs - provides additional formatting/processing for a query e.g.
// processing math on the result, prefix and postfix
using TsGui.Math;
using System.Xml.Linq;

namespace TsGui
{
    public class ResultFormatter
    {
        public string Input { get; set; }
        public string Calculation { get; set; }
        public string Append { get; set; }
        public string Prefix { get; set; }
        public string Value { get { return this.Process(); } }

        public ResultFormatter(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }


        private void LoadXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("Calculate");
            if (x != null)
            {
                this.Calculation = x.Value;
            }

            x = InputXml.Element("Append");
            if (x != null)
            {
                this.Append = x.Value;
            }

            x = InputXml.Element("Prefix");
            if (x != null)
            {
                this.Prefix = x.Value;
            }
        }

        private string Process()
        {
            string s = null;

            if (string.IsNullOrEmpty(this.Input)) { s = null; }
            else
            {
                if (!string.IsNullOrEmpty(this.Calculation))
                {
                    s = Calculation.Replace("VALUE", this.Input);
                    s = Calculator.CalculateString(s).ToString();
                }
            }
            return this.Prefix + s + this.Append;
        }
    }
}
