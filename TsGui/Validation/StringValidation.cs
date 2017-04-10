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

// Validation.cs - responsible for storing rules that a string can be validated against
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using TsGui.Grouping;
using TsGui.Validation.StringMatching;


namespace TsGui.Validation
{
    public class StringValidation: GroupableValidationBase
    {
        private bool _validateempty = true;
        private int _maxlength = int.MaxValue;
        private int _minlength = 0;
        private MatchingRuleSet _validrules = new MatchingRuleSet();
        private MatchingRuleSet _invalidrules = new MatchingRuleSet();
        private MainController _controller;

        //Properties

        #region
        public string ValidationMessage { get; set; }
        public string FailedValidationMessage { get; set; }
        public int MinLength
        {
            get { return this._minlength; }
            set
            {
                if (value < 0) { this._minlength = 0; }
                else { this._minlength = value; }
            }
        }
        public int MaxLength
        {
            get { return this._maxlength; }
            set
            { this._maxlength = value; }
        }
        #endregion

        public StringValidation(MainController MainController)
        {
            this._controller = MainController;
            this.SetDefaults();

        }

        private void SetDefaults()
        {
            this.IsActive = true;
            this.MaxLength = 32760;
            this.MinLength = 0;
        }

        public void LoadXml(XElement InputXml)
        {
            XElement x;
            IEnumerable<XElement> xlist;

            this.ValidationMessage = XmlHandler.GetStringFromXElement(InputXml, "Message", this.ValidationMessage);
            this._validateempty = XmlHandler.GetBoolFromXAttribute(InputXml, "ValidateEmpty", this._validateempty);
            this._maxlength = XmlHandler.GetIntFromXElement(InputXml, "MaxLength", this._maxlength);
            this._minlength = XmlHandler.GetIntFromXElement(InputXml, "MinLength", this._minlength);

            x = InputXml.Element("Valid");
            if (x != null)
            {
                this._validrules.LoadXml(x);
            }

            x = InputXml.Element("Invalid");
            if (x != null)
            {
                this._invalidrules.LoadXml(x);
            }

            xlist = InputXml.Elements("Group");
            if (xlist != null)
            {
                foreach (XElement groupx in xlist)
                {
                    Group g = this._controller.GroupLibrary.GetGroupFromID(groupx.Value);
                    this._groups.Add(g);
                    g.StateEvent += this.OnGroupStateChange;
                }
            }
        }

        //load options from old Xml e.g. pre-0.9.5.0
        public void LoadLegacyXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("Disallowed");
            if (x != null)
            {
                x = x.Element("Characters");
                if (x != null)
                {
                    //StringMatchingRule newrule = new StringMatchingRule(StringMatchingRuleType.Characters,x.Value);
                    //this._invalidrules.Add(new MatchingRuleFactory.GetRuleObject(x.Value));
                    Characters newrule = new Characters(new XElement("Rule", x.Value));
                    this._invalidrules.Add(newrule);
                }
            }
        }

        public bool IsValid(string Input)
        {
            bool result = true;

            string input;
            if (Input == null) { input = string.Empty; }
            else { input = Input; }

            this.FailedValidationMessage = string.Empty;

            if (this.IsActive == false) { return result; }

            if (string.IsNullOrEmpty(input) && (this._validateempty == false)) { return true; }
            if (IsShorterThanMinLength(input)) { result = false; }
            if (IsLongerThanMaxLength(input)) { result = false; }
            if (IsValidMatched(input) == false) { result = false; }
            if (IsInvalidMatched(input) == true) { result = false; }            

            return result;
        }

        public bool IsShorterThanMinLength(string Input)
        {
            if (this.IsActive == false) { return false; }
            if (string.IsNullOrEmpty(Input))
            { if (this.MinLength == 0 ) { return false; } }
            

            if ((Input.Length < this.MinLength))
            {
                this.FailedValidationMessage = this.FailedValidationMessage + Environment.NewLine + "Minimum length: " + this.MinLength + " characters" + Environment.NewLine;
                return true;
            }
            else { return false; }
        }

        public bool IsLongerThanMaxLength(string Input)
        {
            if (this.IsActive == false) { return false; }
            if (string.IsNullOrEmpty(Input)) { return false; }

            if ((Input.Length > this.MaxLength))
            {
                this.FailedValidationMessage = this.FailedValidationMessage + Environment.NewLine + "Maximum length: " + this.MaxLength + " characters" + Environment.NewLine;
                return true;
            }
            else { return false; }
        }

        public bool IsInvalidMatched(string Input)
        {
            if (this.IsActive == false) { return false; }
            bool result = false;
            string s = string.Empty;

            if (this._invalidrules.DoesStringMatch(Input))
            {
                s = this._invalidrules.LastFailedMatchMessage;
                result = true;
            }

            if (result == true)
            {
                this.FailedValidationMessage = this.FailedValidationMessage + Environment.NewLine +  "Must not match any of: " + Environment.NewLine + s;
            }

            return result;
        }

        public bool IsValidMatched(string Input)
        {
            if (this.IsActive == false) { return true; }
            if (this._validrules.Count == 0) { return true; }

            string s = string.Empty;

            if (this._validrules.DoesStringMatch(Input))
            { return true; }
            else
            { s = this._validrules.LastFailedMatchMessage; }

            this.FailedValidationMessage = this.FailedValidationMessage + Environment.NewLine + "Must match one of: " + Environment.NewLine + s;
            return false;
        }

        public string GetAllInvalidCharacters()
        {
            if (this.IsActive == false) { return string.Empty; }
            string s = string.Empty;
            foreach (IStringMatchingRule rule in this._invalidrules.Rules)
            {
                if (rule is Characters) { s = s + rule.Content; }
            }
            return s;
        }
    }
}
