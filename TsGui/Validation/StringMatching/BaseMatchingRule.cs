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
    public abstract class BaseMatchingRule
    {
        public string Content { get; set; }
        public bool IsCaseSensitive { get; set; }

        public BaseMatchingRule() { }

        public BaseMatchingRule(XElement inputxml)
        { this.LoadXml(inputxml); }

        protected void LoadXml(XElement inputxml)
        {
            if (inputxml == null) { return; }

            this.Content = inputxml.Value;
            this.IsCaseSensitive = XmlHandler.GetBoolFromXAttribute(inputxml, "CaseSensitive", false); 
        }
    }
}
