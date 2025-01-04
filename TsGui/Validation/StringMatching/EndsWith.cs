#region license
// Copyright (c) 2025 Mike Pohatu
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
    public class EndsWith : BaseStringMatchingRule, IStringMatchingRule
    {
        public string Message { get { return "EndsWith: " + this.Content; } }

        public EndsWith(XElement inputxml, ILinkTarget linktarget) :base(inputxml, linktarget) { }

        protected override bool Compare(string input)
        {
            if (this.IsCaseSensitive == false)
            {
                return this.ReplaceNullWithEmpty(input).EndsWith(this.Content.ToUpper());
            }
            return this.ReplaceNullWithEmpty(input).EndsWith(this.Content);
        }
    }
}
