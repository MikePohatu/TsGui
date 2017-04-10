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

using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using TsGui.Diagnostics;

namespace TsGui.Validation.StringMatching
{
    public class RegEx : BaseMatchingRule, IStringMatchingRule
    {
        public string Message { get { return "RegEx: " + this.Content ; } }

        public RegEx(XElement inputxml) : base(inputxml) { }

        public bool DoesMatch(string input)
        {
            try
            {
                if (this.IsCaseSensitive == true) { return Regex.IsMatch(input, this.Content); }
                else { return Regex.IsMatch(input, this.Content, RegexOptions.IgnoreCase); }
            }
            catch (Exception e) { throw new TsGuiKnownException("Error processing RegEx: " + this.Content, e.Message); }
        }
    }
}
