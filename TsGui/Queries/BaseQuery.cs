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
using System.Collections.Generic;
using TsGui.Validation.StringMatching;
using TsGui.Linking;
using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.Queries
{
    public abstract class BaseQuery: IQuery
    {
        protected List<IStringMatchingRule> _ignorerules = new List<IStringMatchingRule>();
        protected bool _reprocess = false;
        protected bool _processed = false;
        protected ResultWrangler _processingwrangler = new ResultWrangler();
        protected ResultWrangler _returnwrangler;
        protected bool _ignoreempty = true;
        protected ILinkTarget _linktarget;

        public virtual async Task<ResultWrangler> GetResultWrangler(Message message)
        {
            if ((this._reprocess == true) || (this._processed == false)) { return await this.ProcessQuery(message); }
            else { return this._returnwrangler; }
        }

        public abstract Task<ResultWrangler> ProcessQuery(Message message);

        public BaseQuery(ILinkTarget linktarget)
        {
            this._linktarget = linktarget;
        }

        protected void LoadXml(XElement InputXml)
        {
            this._reprocess = XmlHandler.GetBoolFromXAttribute(InputXml, "Reprocess", this._reprocess);
            this._ignoreempty = XmlHandler.GetBoolFromXAttribute(InputXml, "IgnoreEmpty", this._ignoreempty);
            foreach (XElement xignorerule in InputXml.Elements("Ignore"))
            {
                this._ignorerules.Add(MatchingRuleFactory.GetRuleObject(xignorerule, this._linktarget));
            }
        }

        protected bool ShouldIgnore(string input)
        {
            if ((this._ignoreempty == true) && (string.IsNullOrWhiteSpace(input) == true)) { return true; }

            foreach (IStringMatchingRule rule in this._ignorerules)
            {
                if (rule.DoesMatch(input) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
