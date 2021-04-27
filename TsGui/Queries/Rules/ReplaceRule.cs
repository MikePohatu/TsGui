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


using System.Xml.Linq;

namespace TsGui.Queries.Rules
{
    public class ReplaceRule: IQueryRule
    {
        private string _searchstring;
        private string _replacestring;

        public ReplaceRule(XElement InputXml)
        {
            //XAttribute presearvespace = new XAttribute("xml:space", "preserve");
            //InputXml.Add(presearvespace);


            this._searchstring = XmlHandler.GetStringFromXElement(InputXml, "Search", this._searchstring);
            this._replacestring = XmlHandler.GetStringFromXElement(InputXml, "Replace", this._replacestring);
            //this._replacestring = InputXml.Value;

            if (this._replacestring == null) { this._replacestring = string.Empty; }
        }

        public string Process(string Input)
        {
            if (string.IsNullOrEmpty(this._searchstring)) { return Input; }
            return Input.Replace(this._searchstring, this._replacestring);
        }
    }
}
