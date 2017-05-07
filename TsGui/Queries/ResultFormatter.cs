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

using System.Xml.Linq;
using System.Collections.Generic;

namespace TsGui.Queries
{
    public class ResultFormatter
    {
        private List<IQueryRule> _rules = new List<IQueryRule>();
        private string _value;
        private string _input;

        public string Name { get; set; }
        public string Input
        {
            get { return this._input; }
            set
            {
                this._input = value;
                this._value = this.Process();
            }
        }
        public string Value { get { return this._value; } }

        public ResultFormatter() { }

        public ResultFormatter(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        private void LoadXml(XElement InputXml)
        {
            this.Name = XmlHandler.GetStringFromXAttribute(InputXml, "Name", this.Name);

            foreach (XElement xsetting in InputXml.Elements())
            {
                switch(xsetting.Name.ToString())
                {
                    case "Prefix":
                        this._rules.Add(new PrefixRule(xsetting));
                        break;
                    case "Append":
                        this._rules.Add(new AppendRule(xsetting));
                        break;
                    case "Calculate":
                        this._rules.Add(new CalculateRule(xsetting));
                        break;
                    case "Truncate":
                        this._rules.Add(new TruncateRule(xsetting));
                        break;
                    default:
                        break;
                }
            }
        }

        private string Process()
        {
            string s = this._input;

            if (string.IsNullOrEmpty(s)) { return s; }

            foreach (IQueryRule rule in this._rules)
            { s = rule.Process(s); }

            return s;
        }
    }
}
