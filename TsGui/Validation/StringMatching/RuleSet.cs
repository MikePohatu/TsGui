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

// RuleSet.cs - List of rules for AND/OR

using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Validation.StringMatching
{
    public class RuleSet: IStringMatchingRule
    {
        
        private List<IStringMatchingRule> _rules = new List<IStringMatchingRule>();
        private AndOr _ruletype = AndOr.OR;
        private string _message = null;
        private bool _ischild = true;

        public List<IStringMatchingRule> Rules { get { return this._rules; } }
        public string Message
        {
            get
            {
                if (this._message == null) { return this.BuildMessage(); }
                else { return this._message; }
            }
        }
        public bool IsChild
        {
            get { return this._ischild; }
            set { this._ischild = value; }
        }
        public int Count { get { return this._rules.Count; } }

        public RuleSet(XElement inputxml)
        { this.LoadXml(inputxml); }

        public RuleSet() { }

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
        }

        public void Add(IStringMatchingRule rule)
        {
            if (rule != null) { this._rules.Add(rule); }
        }

        public bool DoesMatch(string input)
        {
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
            if (this._rules.Count == 0) { return true; }

            foreach (IStringMatchingRule rule in this._rules)
            {
                if (rule.DoesMatch(input) == true) { return true; }
            }
            return false;
        }

        private string BuildMessage()
        {
            string s = string.Empty;

            foreach (IStringMatchingRule rule in this._rules)
            {
                if (string.IsNullOrEmpty(s) == true) { s = rule.Message; }
                else { s = s + " " + this._ruletype.ToString() + " " + rule.Message; }
            }

            if (this._ischild == true) { s = "(" + s + ")"; }

            this._message = s;
            return s;
        }        
    }
}
