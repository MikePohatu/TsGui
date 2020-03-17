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
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Validation.StringMatching
{
    public abstract class BaseStringMatchingRule : BaseMatchingRule
    {
        //protected string _rulestring;

        public BaseStringMatchingRule(XElement inputxml, ILinkTarget linktarget) : base(inputxml, linktarget) { }

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
