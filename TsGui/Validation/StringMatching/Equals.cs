﻿//    Copyright (C) 2016 Mike Pohatu

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
    public class Equals : BaseStringMatchingRule, IStringMatchingRule
    {
        public string Message { get { return "Equals: " + this.Content; } }

        public Equals(XElement inputxml, ILinkTarget linktarget) :base(inputxml, linktarget)
        { }

        protected override bool Compare(string input)
        {
            if (input == null)
            {
                if (this.Content?.ToUpper() == "*NULL") { return true; }
                else { return false; }
            }

            string s = this.Content;
            if (this.IsCaseSensitive)
            {
                return input.ToUpper().Equals(this.Content.ToUpper());
            }
            return input.Equals(this.Content);
        }
    }
}
