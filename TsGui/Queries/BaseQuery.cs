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

        protected void LoadXml(XElement InputXml)
        {
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
