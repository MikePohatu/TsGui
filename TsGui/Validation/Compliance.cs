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

// Compliance.cs - responsible for storing rules that a string can be checked for compliance against
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using TsGui.Grouping;

namespace TsGui.Validation
{
    public class Compliance: GroupableValidationBase
    {

        private List<StringValidationRule> _okrules = new List<StringValidationRule>();
        private List<StringValidationRule> _warningrules = new List<StringValidationRule>();
        private List<StringValidationRule> _errorrules = new List<StringValidationRule>();
        private List<StringValidationRule> _invalidrules = new List<StringValidationRule>();
        private int _defaultstate = ComplianceStateValues.Invalid;
        private MainController _controller;

        //properties
        public string Message { get; set; }
        public string FailedComplianceMessage { get; set; }

        public Compliance(MainController MainController)
        {
            this._controller = MainController;
            this.IsActive = true;
        }

        public void LoadXml(XElement InputXml)
        {
            XElement x;
            IEnumerable<XElement> xlist;

            this.Message = XmlHandler.GetStringFromXElement(InputXml, "Message", this.Message);
            this._defaultstate = XmlHandler.GetComplianceStateValueFromXElement(InputXml, "DefaultState", this._defaultstate);
            x = InputXml.Element("OK");
            if (x != null)
            {
                foreach (XElement subx in x.Elements("Rule"))
                {
                    StringValidationRule newrule = new StringValidationRule(subx);
                    this._okrules.Add(newrule);
                }
            }

            x = InputXml.Element("Warning");
            if (x != null)
            {
                foreach (XElement subx in x.Elements("Rule"))
                {
                    StringValidationRule newrule = new StringValidationRule(subx);
                    this._warningrules.Add(newrule);
                }
            }

            x = InputXml.Element("Error");
            if (x != null)
            {
                foreach (XElement subx in x.Elements("Rule"))
                {
                    StringValidationRule newrule = new StringValidationRule(subx);
                    this._errorrules.Add(newrule);
                }
            }

            x = InputXml.Element("Invalid");
            if (x != null)
            {
                foreach (XElement subx in x.Elements("Rule"))
                {
                    StringValidationRule newrule = new StringValidationRule(subx);
                    this._invalidrules.Add(newrule);
                }
            }

            xlist = InputXml.Elements("Group");
            if (xlist != null)
            {
                foreach (XElement groupx in xlist)
                {
                    Group g = this._controller.GetGroupFromID(groupx.Value);
                    this._groups.Add(g);
                    g.StateEvent += this.OnGroupStateChange;
                    ;
                }
            }
        }

        public bool IsValid(string Input)
        {
            if (this.EvaluateState(Input) == ComplianceStateValues.Invalid) { return false; }
            else { return true; }
        }

        public int EvaluateState(string Input)
        {
            foreach (StringValidationRule rule in this._invalidrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true)
                { return ComplianceStateValues.Invalid; }
            }

            foreach (StringValidationRule rule in this._errorrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true)
                { return ComplianceStateValues.Error; }
            }

            foreach (StringValidationRule rule in this._warningrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true)
                { return ComplianceStateValues.Warning; }
            }

            if (this._okrules.Count == 0 ) { return ComplianceStateValues.OK; }

            foreach (StringValidationRule rule in this._okrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true)
                { return ComplianceStateValues.OK; }
            }

            return this._defaultstate;
        }
    }
}
