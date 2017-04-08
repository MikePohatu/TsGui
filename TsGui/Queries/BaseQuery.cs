//    Copyright (C) 2017 Mike Pohatu

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
using System.Collections.Generic;
using TsGui.Validation;

namespace TsGui.Queries
{
    public abstract class BaseQuery
    {
        protected List<StringMatchingRule> _ignorerules = new List<StringMatchingRule>();
        protected bool _reprocess = false;
        protected bool _processed = false;
        protected ResultWrangler _returnwrangler = new ResultWrangler();

        public virtual ResultWrangler GetResultWrangler()
        {
            if ((this._reprocess == true) || (this._processed == false)) { return this.ProcessQuery(); }
            else { return this._returnwrangler; }
        }

        public abstract ResultWrangler ProcessQuery();

        protected void LoadXml(XElement InputXml)
        {
            this._reprocess = XmlHandler.GetBoolFromXAttribute(InputXml, "Reprocess", this._reprocess);
            foreach (XElement xignorerule in InputXml.Elements("Ignore"))
            {
                this._ignorerules.Add(new StringMatchingRule(xignorerule));
            }
        }

        protected bool ShouldIgnore(string input)
        {
            foreach (StringMatchingRule rule in this._ignorerules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, input) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
