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

namespace TsGui.Validation
{
    public class StringValidation
    {
        private bool _validateempty;
        private int _maxlength;
        private int _minlength;
        private List<StringValidationRule> _validrules = new List<StringValidationRule>();
        private List<StringValidationRule> _invalidrules = new List<StringValidationRule>();


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

        public void LoadXml(XElement InputXml)
        {
            XElement x;

            #region
            this.ValidationMessage = XmlHandler.GetStringFromXElement(InputXml, "Message", string.Empty);
            this._validateempty = XmlHandler.GetBoolFromXAttribute(InputXml, "ValidateEmpty", true);
            this._maxlength = XmlHandler.GetIntFromXElement(InputXml, "MaxLength", int.MaxValue);
            this._minlength = XmlHandler.GetIntFromXElement(InputXml, "MinLength", 0);

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
            #endregion
        }

        //load options from old Xml e.g. pre-0.9.5.0
        public void LoadLegacyXml(XElement InputXml)
        {
            XElement x;
            if (InputXml.Name == "Disallowed")
            {
                x = InputXml.Element("Characters");
                if (x != null)
                {
                    StringValidationRule newrule = new StringValidationRule(StringValidationRuleType.Characters,x.Value);
                    this._invalidrules.Add(newrule);
                }
            }
        }

        public bool IsValid(string Input)
        {
            bool result = true;
            this.FailedValidationMessage = string.Empty;

            if ((Input.Length == 0) && (this._validateempty == false)) { return true; }
            if (IsShorterThanMinLength(Input)) { result = false; }
            if (IsLongerThanMaxLength(Input)) { result = false; }
            if (IsValidMatched(Input) == false) { result = false; }
            if (IsInvalidMatched(Input) == true) { result = false; }            

            return result;
        }

        private bool IsShorterThanMinLength(string Input)
        {
            if ((Input.Length < this.MinLength))
            {
                this.FailedValidationMessage = this.FailedValidationMessage + "Minimum length: " + this.MinLength + " characters" + Environment.NewLine + Environment.NewLine;
                return true;
            }
            else { return false; }
        }

        private bool IsLongerThanMaxLength(string Input)
        {
            if ((Input.Length > this.MaxLength))
            {
                this.FailedValidationMessage = this.FailedValidationMessage + "Maximum length: " + this.MaxLength + " characters" + Environment.NewLine + Environment.NewLine;
                return true;
            }
            else { return false; }
        }

        private bool IsInvalidMatched(string Input)
        {
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
                s = this.FailedValidationMessage +  "Invalid text rule matched: " + Environment.NewLine + s;
                this.FailedValidationMessage = s;
            }
            

            return result;
        }


        private bool IsValidMatched(string Input)
        {
            if (this._validrules.Count == 0) { return true; }

            string s = string.Empty;
            foreach (StringValidationRule rule in this._validrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true)
                { return true; }
                else { s = s + rule.Message + Environment.NewLine; }               
            }

            this.FailedValidationMessage = this.FailedValidationMessage + "Must match one of: " + Environment.NewLine +  s + Environment.NewLine;
            return false;
        }
    }
}
