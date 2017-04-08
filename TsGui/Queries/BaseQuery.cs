using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
