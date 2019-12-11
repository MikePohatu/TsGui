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

using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Validation.StringMatching
{
    public abstract class BaseStringMatchingRule : BaseMatchingRule
    {
        protected string _rulestring;

        public BaseStringMatchingRule(XElement inputxml, ILinkTarget owner) : base(inputxml, owner)
        {
            if (this.IsCaseSensitive == false) { this._rulestring = this.Content.ToUpper(); }
            else { this._rulestring = this.Content; }
        }

        public bool DoesMatch(string input)
        {
            string inputstring;

            inputstring = input;
            if (this.IsCaseSensitive == false)
            {
                inputstring = inputstring?.ToUpper();
            }
            return Compare(inputstring);
        }

        protected abstract bool Compare(string input);

        protected string ReplaceNullWithEmpty(string input)
        {
            if (input == null) { return string.Empty; }
            else { return input; }
        }
    }
}
