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
using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using TsGui.Diagnostics;
using TsGui.Linking;

namespace TsGui.Validation.StringMatching
{
    public class RegEx : BaseMatchingRule, IStringMatchingRule
    {
        public string Message { get { return "RegEx: " + this.Content ; } }

        public RegEx(XElement inputxml, ILinkTarget linktarget) : base(inputxml, linktarget) { }

        public bool DoesMatch(string input)
        {
            string s;
            if (input == null) { s = string.Empty; }
            else { s = input; }
            try
            {
                if (this.IsCaseSensitive == true) { return Regex.IsMatch(s, this.Content); }
                else { return Regex.IsMatch(s, this.Content, RegexOptions.IgnoreCase); }
            }
            catch (Exception e) { throw new TsGuiKnownException("Error processing RegEx: " + this.Content, e.Message); }
        }
    }
}
