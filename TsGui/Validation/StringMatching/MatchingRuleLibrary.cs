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

// MatchingRuleLibrary.cs - stores and processes rules for a match 

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Validation.StringMatching
{
    public class MatchingRuleLibrary
    {

        private List<IStringMatchingRule> _rules = new List<IStringMatchingRule>();
        private string _message = null;

        public List<IStringMatchingRule> Rules { get { return this._rules; } }
        public string Message
        {
            get
            {
                if (this._message == null) { return this.BuildMessage(); }
                else { return this._message; }
            }
        }

        public int Count { get { return this._rules.Count; } }

        public MatchingRuleLibrary(XElement inputxml)
        {
            this.LoadXml(inputxml); 
        }

        public MatchingRuleLibrary() { }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }
            foreach (XElement subx in InputXml.Elements())
            {
                IStringMatchingRule newrule = MatchingRuleFactory.GetRootRuleObject(subx);
                if (newrule != null) { this._rules.Add(newrule); }
            }
        }

        public void Add(IStringMatchingRule rule)
        {
            if (rule != null) { this._rules.Add(rule); }
        }

        public bool DoesMatch(string input)
        {
            foreach (IStringMatchingRule rule in this._rules)
            {
                if (rule.DoesMatch(input)) { return true; }
            }
            return false;
        }

        private string BuildMessage()
        {
            string s = string.Empty;

            foreach (IStringMatchingRule rule in this._rules)
            {
                if (string.IsNullOrEmpty(s) == true) { s = "• " + rule.Message; }
                else { s = s + Environment.NewLine + "• " + rule.Message; }
            }

            this._message = s;
            return s;
        }
    }
}
