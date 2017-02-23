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

// ComplianceHandler.cs - maintains a list of compliance objects. returns correct values taking
// into account group membership

using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace TsGui.Validation
{
    public class ComplianceHandler
    {
        private MainController _controller;
        private List<Compliance> _compliances = new List<Compliance>();
        private IValidationOwner _owner;

        //Properties
        #region
        public string ValidationMessage { get { return this.GetActiveValidationMessages(); } }
        public string FailedValidationMessage { get; set; }
        #endregion

        public ComplianceHandler(IValidationOwner Owner, MainController MainController)
        {
            this._owner = Owner;
            this._controller = MainController;
        }

        public void AddCompliance(XElement InputXml)
        {
            if (InputXml == null) { return; }
            Compliance c = new Compliance(this._controller);
            c.LoadXml(InputXml);
            this.AddCompliance(c);
        }

        public void AddCompliance(Compliance Compliance)
        {
            this._compliances.Add(Compliance);
            Compliance.RevalidationRequired += this._owner.OnValidationChange;
        }

        public void AddCompliances(IEnumerable<XElement> InputXmlList)
        {
            if (InputXmlList == null) { return; }
            foreach (XElement xval in InputXmlList)
            { this.AddCompliance(xval); }
        }

        public int EvaluateComplianceState(string Input)
        {
            int state = ComplianceStateValues.OK;

            foreach (Compliance c in this._compliances)
            {
                if (c.IsActive == true)
                {
                    int currstate = c.EvaluateState(Input);
                    if (currstate == ComplianceStateValues.Invalid) { return currstate; }
                    else if (currstate > state) { state = currstate; }
                }
            }
            return state;
        }

        private string GetActiveValidationMessages()
        {
            string s = string.Empty;
            bool active = false;
            foreach (Compliance c in this._compliances)
            {
                if ((c.IsActive == true) && (!string.IsNullOrEmpty(c.Message)))
                {
                    s = s + Environment.NewLine + c.Message;
                    active = true;
                }
            }
            if (active == true) { return s + Environment.NewLine; }
            else { return string.Empty; }
        }
    }
}
