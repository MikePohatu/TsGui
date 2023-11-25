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

// ReplaceRule.cs - replace text in a string result


using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TsGui.Queries.Rules
{
    public class RegexReplaceRule: IQueryRule
    {
        private string _pattern;
        private string _replacestring;

        public RegexReplaceRule(XElement InputXml)
        {
            this._pattern = XmlHandler.GetStringFromXml(InputXml, "Regex", this._pattern);
            this._replacestring = XmlHandler.GetStringFromXml(InputXml, "Replace", this._replacestring);

            if (this._replacestring == null) { this._replacestring = string.Empty; }
        }

        public string Process(string Input)
        {
            if (string.IsNullOrEmpty(this._pattern)) { return Input; }
            return Regex.Replace(Input, this._pattern, this._replacestring);
        }
    }
}
