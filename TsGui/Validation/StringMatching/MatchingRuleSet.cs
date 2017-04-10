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

// MatchingRuleSet.cs - stores and processes rules for a match 

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Validation.StringMatching
{
    public class MatchingRuleSet: IStringMatchingRule
    {
        
        private List<IStringMatchingRule> _rules = new List<IStringMatchingRule>();
        private AndOr _ruletype = AndOr.OR;

        public List<IStringMatchingRule> Rules { get { return this._rules; } }
        public string Message { get; set; }
        public int Count { get { return this._rules.Count; } }

        public MatchingRuleSet(XElement inputxml)
        { this.LoadXml(inputxml); }

        public MatchingRuleSet() { }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }
            foreach (XElement subx in InputXml.Elements())
            {
                IStringMatchingRule newrule = MatchingRuleFactory.GetRuleObject(subx);
                if (newrule != null) { this._rules.Add(newrule); }
            }

            XAttribute xa = InputXml.Attribute("Type");
            if (xa != null)
            {
                if (xa.Value == "AND") { this._ruletype = AndOr.AND; }
            }

            this.BuildMessage();
        }

        public void Add(IStringMatchingRule rule)
        {
            if (rule != null) { this._rules.Add(rule); }
            this.BuildMessage();
        }

        public bool DoesMatch(string input)
        {
            this.Message = string.Empty;

            if (this._ruletype == AndOr.AND) { return this.AndComparison(input); }
            else { return this.OrComparison(input); }
        }

        private bool AndComparison(string input)
        {
            foreach (IStringMatchingRule rule in this._rules)
            {
                if (rule.DoesMatch(input) == false) { return false; }
            }
            return true;
        }

        private bool OrComparison(string input)
        {
            foreach (IStringMatchingRule rule in this._rules)
            {
                if (rule.DoesMatch(input) == true) { return true; }
            }
            return false;
        }

        private void BuildMessage()
        {
            string s = string.Empty;
            if (this.Count == 0) { s = string.Empty; }
            else { s = "("; }

            foreach (IStringMatchingRule rule in this._rules)
            {
                if (s == "(") { s = s + rule.Message + Environment.NewLine; }
                else { s = s + this._ruletype.ToString() + " " + rule.Message + Environment.NewLine; }
            }

            if (this.Count != 0) { s = s+ ")"; }

            this.Message = s;
        }        
    }
}
