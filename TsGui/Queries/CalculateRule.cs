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

// CalculateRule.cs - used to process math functions against a string number input

using System.Xml.Linq;
using System;

using TsGui.Math;

namespace TsGui.Queries
{
    class CalculateRule: IQueryRule
    {
        public int DecimalPlaces { get; set; }
        public string Calculation { get; set; }

        public CalculateRule(XElement InputXml)
        {
            this.DecimalPlaces = -1;
            this.LoadXml(InputXml);
        }

        private void LoadXml(XElement InputXml)
        {
            XAttribute xattrib;
            this.Calculation = InputXml.Value;

            xattrib = InputXml.Attribute("DecimalPlaces");
            if (xattrib != null) { this.DecimalPlaces = Convert.ToInt32(xattrib.Value); }
        }

        public string Process(string Input)
        {
            string s;

            if (!string.IsNullOrEmpty(Input))
            {
                try
                {
                    double result;
                    s = Calculation.Replace("VALUE", Input);
                    result = Calculator.CalculateString(s);

                    if (this.DecimalPlaces != -1)
                    { result = System.Math.Round(result, this.DecimalPlaces); }

                    return result.ToString();
                }
                catch { return Input; }
            }
            else { return Input; }
        }
    }
}
