﻿#region license
// Copyright (c) 2025 Mike Pohatu
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

// Compliance.cs - responsible for storing rules that a string can be checked for compliance against

using System.Xml.Linq;
using System.Collections.Generic;
using TsGui.Grouping;
using TsGui.Validation.StringMatching;
using TsGui.Linking;

namespace TsGui.Validation
{
    public class Compliance: GroupableValidationBase
    {

        private MatchingRuleLibrary _okrules;
        private MatchingRuleLibrary _warningrules;
        private MatchingRuleLibrary _errorrules;
        private MatchingRuleLibrary _invalidrules;
        private int _defaultstate = ComplianceStateValues.Invalid;

        //properties
        public string Message { get; set; }
        public string FailedComplianceMessage { get; set; }

        public Compliance(ILinkTarget linktarget)
        {
            this.IsActive = true;
            this._okrules = new MatchingRuleLibrary(linktarget);
            this._warningrules = new MatchingRuleLibrary(linktarget);
            this._errorrules = new MatchingRuleLibrary(linktarget);
            this._invalidrules = new MatchingRuleLibrary(linktarget);
    }

        public void LoadXml(XElement InputXml)
        {
            IEnumerable<XElement> xlist;

            this.Message = XmlHandler.GetStringFromXml(InputXml, "Message", this.Message);
            this._defaultstate = XmlHandler.GetComplianceStateValueFromXml(InputXml, "DefaultState", this._defaultstate);

            this._okrules.LoadXml(InputXml.Element("OK"));
            this._warningrules.LoadXml(InputXml.Element("Warning"));
            this._errorrules.LoadXml(InputXml.Element("Error"));
            this._invalidrules.LoadXml(InputXml.Element("Invalid"));

            xlist = InputXml.Elements("Group");
            if (xlist != null)
            {
                foreach (XElement groupx in xlist)
                {
                    Group g = GroupLibrary.GetGroupFromID(groupx.Value);
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
            if (this._invalidrules.DoesMatch(Input) == true) { return ComplianceStateValues.Invalid; }
            if (this._errorrules.DoesMatch(Input) == true) { return ComplianceStateValues.Error; }
            if (this._warningrules.DoesMatch(Input) == true)  { return ComplianceStateValues.Warning; }
            if (this._okrules.Count == 0 ) { return ComplianceStateValues.OK; }
            if (this._okrules.DoesMatch(Input) == true) { return ComplianceStateValues.OK; }

            return this._defaultstate;
        }
    }
}
