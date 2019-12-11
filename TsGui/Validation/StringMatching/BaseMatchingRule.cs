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

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using TsGui.Queries;
using TsGui.Linking;

namespace TsGui.Validation.StringMatching
{
    public abstract class BaseMatchingRule
    {
        private QueryPriorityList _querylist;

        public string Content { 
            get {
                string s = this._querylist.GetResultWrangler()?.GetString();
                return s == null ? string.Empty : s; 
            } 
        }
        public bool IsCaseSensitive { get; set; }

        public BaseMatchingRule(ILinkTarget linktarget) 
        {
            this._querylist = new QueryPriorityList(linktarget);
        }

        public BaseMatchingRule(XElement inputxml, ILinkTarget linktarget)
        {
            this._querylist = new QueryPriorityList(linktarget);
            this.LoadXml(inputxml); 
        }

        protected void LoadXml(XElement inputxml)
        {
            if (inputxml == null) { return; }

            IEnumerable<XElement> xlist = inputxml.Elements();
            if (xlist.Count() > 0)
            {
                this._querylist.LoadXml(inputxml);
            }
            else
            {
                ValueOnlyQuery v = new ValueOnlyQuery(inputxml.Value);
                this._querylist.AddQuery(v);
            }

            this.IsCaseSensitive = XmlHandler.GetBoolFromXAttribute(inputxml, "CaseSensitive", this.IsCaseSensitive); 
        }
    }
}
