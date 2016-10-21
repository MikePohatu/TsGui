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
            //Load the XML
            XElement x;

            #region
            this.ValidationMessage = XmlHandler.GetStringFromXElement(InputXml, "Message", string.Empty);
            this._validateempty = XmlHandler.GetBoolFromXAttribute(InputXml, "ValidateEmpty", true);
            this._maxlength = XmlHandler.GetIntFromXElement(InputXml, "MaxLength", int.MaxValue);
            this._minlength = XmlHandler.GetIntFromXElement(InputXml, "MinLength", 0);

            x = InputXml.Element("Valid");
            if (x != null)
            {
                foreach (XElement subx in x.Elements())
                {
                    StringValidationRule newrule = new StringValidationRule();
                    newrule.LoadXml(subx);
                    this._validrules.Add(newrule);
                }
            }

            x = InputXml.Element("Invalid");
            if (x != null)
            {
                foreach (XElement subx in x.Elements())
                {
                    StringValidationRule newrule = new StringValidationRule();
                    newrule.LoadXml(subx);
                    this._invalidrules.Add(newrule);
                }
            }
            #endregion
        }


        public bool IsValid(string Input)
        {
            int length = Input.Length;
            if ((Input.Length == 0) && (this._validateempty == false)) { return true; }
            if ((Input.Length < this.MinLength)) { return false; }
            if (Input.Length > this.MaxLength) { return false; }
            if (IsValidMatched(Input) == false) { return false; }
            if (IsInvalidMatched(Input) == true) { return false; }            
            
            return true;
        }


        private bool IsInvalidMatched(string Input)
        {
            foreach (StringValidationRule rule in this._invalidrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true) { return true; }
            }
            
            return false;
        }


        private bool IsValidMatched(string Input)
        {
            if (this._validrules.Count == 0) { return true; }

            foreach (StringValidationRule rule in this._validrules)
            {
                if (ResultValidator.DoesStringMatchRule(rule, Input) == true) { return true; }
            }
            return false;
        }
    }
}
