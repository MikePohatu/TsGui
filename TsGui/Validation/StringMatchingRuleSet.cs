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

// LinkingRuleProcesssor.cs - stores and processes rules for a match 

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Validation
{
    public class StringMatchingRuleSet
    {
        
        private List<StringMatchingRule> _rules = new List<StringMatchingRule>();
        private AndOr _ruletype = AndOr.OR;

        public List<StringMatchingRule> Rules { get { return this._rules; } }
        public string LastFailedMatchMessage { get; set; }
        public int Count { get { return this._rules.Count; } }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }
            foreach (XElement subx in InputXml.Elements("Rule"))
            {
                StringMatchingRule newrule = new StringMatchingRule(subx);
                this._rules.Add(newrule);
            }

            XAttribute xa = InputXml.Attribute("Type");
            if (xa != null)
            {
                if (xa.Value == "AND") { this._ruletype = AndOr.AND; }
            }
        }

        public void Add(StringMatchingRule rule)
        { if (rule != null) { this._rules.Add(rule); } }

        public bool DoesStringMatch(string input)
        {
            this.LastFailedMatchMessage = string.Empty;

            if (this._ruletype == AndOr.AND) { return this.AndComparison(input); }
            else { return this.OrComparison(input); }
        }

        private bool AndComparison(string input)
        {
            string s = string.Empty;
            foreach (StringMatchingRule rule in this._rules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, input) == false)
                { return false; }
                else { s = s + rule.Message + Environment.NewLine; }
            }
            this.LastFailedMatchMessage = s;
            return true;
        }

        private bool OrComparison(string input)
        {
            string s = string.Empty;
            foreach (StringMatchingRule rule in this._rules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, input) == true)
                { return true; }
                else { s = s + rule.Message + Environment.NewLine; }
            }
            this.LastFailedMatchMessage = s;
            return false;
        }
    }
}
