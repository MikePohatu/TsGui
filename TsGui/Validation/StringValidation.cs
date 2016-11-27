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

namespace TsGui.Validation
{
    public class StringValidation
    {
        private bool _validateempty = true;
        private int _maxlength = int.MaxValue;
        private int _minlength = 0;
        private List<StringValidationRule> _validrules = new List<StringValidationRule>();
        private List<StringValidationRule> _invalidrules = new List<StringValidationRule>();
        private MainController _controller;
        private List<Group> _groups = new List<Group>();

        //Properties

        #region
        public bool IsActive { get; set; }
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
                foreach (XElement subx in x.Elements("Rule"))
                {
                    StringValidationRule newrule = new StringValidationRule(subx);
                    this._validrules.Add(newrule);
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
;               }
            }
        }

        //load options from old Xml e.g. pre-0.9.5.0
        #region
        public void LoadLegacyXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("Disallowed");
            if (x != null)
            {
                x = x.Element("Characters");
                if (x != null)
                {
                    StringValidationRule newrule = new StringValidationRule(StringValidationRuleType.Characters,x.Value);
                    this._invalidrules.Add(newrule);
                }
            }
        }
        #endregion

        public bool IsValid(string Input)
        {
            bool result = true;
            this.FailedValidationMessage = string.Empty;

            if (this.IsActive == false) { return result; }

            if (string.IsNullOrEmpty(Input) && (this._validateempty == false)) { return true; }
            if (IsShorterThanMinLength(Input)) { result = false; }
            if (IsLongerThanMaxLength(Input)) { result = false; }
            if (IsValidMatched(Input) == false) { result = false; }
            if (IsInvalidMatched(Input) == true) { result = false; }            

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

            foreach (StringValidationRule rule in this._invalidrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true)
                {
                    s = s + rule.Message + Environment.NewLine;
                    result = true;
                }
            }

            if (result == true)
            {
                s = this.FailedValidationMessage + Environment.NewLine +  "Must not match any of: " + Environment.NewLine + s;
                this.FailedValidationMessage = s;
            }

            return result;
        }

        public bool IsValidMatched(string Input)
        {
            if (this.IsActive == false) { return true; }
            if (this._validrules.Count == 0) { return true; }

            string s = string.Empty;
            foreach (StringValidationRule rule in this._validrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true)
                { return true; }
                else { s = s + rule.Message + Environment.NewLine; }               
            }

            this.FailedValidationMessage = this.FailedValidationMessage + Environment.NewLine + "Must match one of: " + Environment.NewLine + s;
            return false;
        }

        public string GetAllInvalidCharacters()
        {
            if (this.IsActive == false) { return string.Empty; }
            string s = string.Empty;
            foreach (StringValidationRule rule in this._invalidrules)
            {
                if (rule.Type == StringValidationRuleType.Characters) { s = s + rule.Content; }
            }
            return s;
        }

        //Grouping 
        public GroupStateChange RevalidationRequired;
        public void OnGroupStateChange()
        {
            bool result = false;
            foreach (Group g in this._groups)
            { if (g.State == GroupState.Enabled) { result = true; break; } }

            if (this.IsActive != result)
            {
                this.IsActive = result;
                this.RevalidationRequired?.Invoke();
            }
        }
    }
}
