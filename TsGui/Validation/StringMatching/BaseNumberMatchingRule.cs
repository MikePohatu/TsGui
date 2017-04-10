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

namespace TsGui.Validation.StringMatching
{
    public abstract class BaseNumberMatchingRule : BaseMatchingRule
    {
        public BaseNumberMatchingRule(XElement inputxml) : base(inputxml) { }

        public bool DoesMatch(string input)
        {
            double inputnum;
            double rulenum;

            if (!double.TryParse(input, out inputnum)) { return false; }
            if (!double.TryParse(this.Content, out rulenum)) { return false; }

            return this.Compare(inputnum, rulenum);
        }

        protected abstract bool Compare(double input, double rulenumber);
    }
}
