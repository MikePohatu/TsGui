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
    public class LessThanOrEqualTo : BaseNumberMatchingRule, IStringMatchingRule
    {
        public string Message { get { return "LessThanOrEqualTo: " + this.Content; } }

        public LessThanOrEqualTo(XElement inputxml, ILinkTarget linktarget) :base(inputxml, linktarget)
        { }

        protected override bool Compare(double input, double rulecontent)
        { return input <= rulecontent; }
    }
}
