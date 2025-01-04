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
    public class Characters : BaseStringMatchingRule, IStringMatchingRule
    {
        private char[] _invalidchars;
        public string Message { get { return "Contains: " + this.Content; } }

        public Characters(XElement inputxml, ILinkTarget linktarget) :base(inputxml, linktarget)
        {
            this._invalidchars = this.Content.ToCharArray();
        }

        protected override bool Compare(string input)
        {
            if (input == null) { return false; }
            if (string.IsNullOrEmpty(this.Content)) { return true; }

            foreach (char c in this._invalidchars)
            {
                if (input.Contains(c.ToString())) { return true; }
            }

            return false;
        }
    }
}
