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

// ValidationHandler.cs - maintains a list of stringvalidation objects. returns correct values taking
// into account group membership

using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace TsGui.Validation
{
    public class ValidationHandler
    {
        private IDirector _controller;
        private List<StringValidation> _validations = new List<StringValidation>();
        private XElement _legacyxml = new XElement("Legacy");
        private IValidationOwner _owner;
        
        //Properties
        #region
        public string ValidationMessage { get { return this.GetActiveValidationMessages(); } }
        public string FailedValidationMessage { get; set; }
        public int MinLength { get { return this.GetMinLength(); } }
        public int MaxLength { get { return this.GetMaxLength(); } }
        #endregion

        public ValidationHandler(IValidationOwner Owner, IDirector MainController)
        {
            this._owner = Owner;
            this._controller = MainController;
        }

        public void AddValidation(XElement InputXml)
        {
            if (InputXml == null) { return; }
            StringValidation sv = new StringValidation(this._owner);
            sv.LoadXml(InputXml);
            this.AddValidation(sv);
        }

        public void AddValidation(StringValidation Validation)
        {
            this._validations.Add(Validation);
            Validation.RevalidationRequired += this._owner.OnValidationChange;
        }

        public void AddValidations(IEnumerable<XElement> InputXmlList)
        {
            if (InputXmlList == null) { return; }
            foreach (XElement xval in InputXmlList)
            { this.AddValidation(xval); }   
        }

        public void LoadLegacyXml(XElement InputXml)
        {
            XElement x;
            XElement result = new XElement("Legacy");
            //load legacy options
            x = InputXml.Element("Disallowed");
            if (x != null) { result.Add(x); }

            x = InputXml.Element("MinLength");
            if (x != null) { result.Add(x); }

            StringValidation sv = new StringValidation(this._owner);
            sv.LoadLegacyXml(result);
            this.AddValidation(sv);
        }

        public bool IsValid(string Input)
        {
            bool result = true;
            string s = string.Empty;

            foreach (StringValidation sv in this._validations)
            {
                if (sv.IsValid(Input) == false )
                {
                    s = s + sv.FailedValidationMessage;
                    result = false;
                }
            }
            this.FailedValidationMessage = s;
            return result;
        }

        private bool IsShorterThanMinLength(string Input)
        {
            StringValidation sv = this.GetFirstActiveValidation();
            if (sv != null) { return sv.IsShorterThanMinLength(Input); }
            else { return false; }
        }

        private bool IsLongerThanMaxLength(string Input)
        {
            StringValidation sv = this.GetFirstActiveValidation();
            if (sv != null) { return sv.IsLongerThanMaxLength(Input); }
            else { return false; }
        }

        private bool IsInvalidMatched(string Input)
        {
            StringValidation sv = this.GetFirstActiveValidation();
            if (sv != null) { return sv.IsInvalidMatched(Input); }
            else { return false; }
        }

        private bool IsValidMatched(string Input)
        {
            StringValidation sv = this.GetFirstActiveValidation();
            if (sv != null) { return sv.IsValidMatched(Input); }
            else { return true; }
        }

        public string GetAllInvalidCharacters()
        {
            string s = string.Empty;
            foreach (StringValidation sv in this._validations)
            {
                if (sv.IsActive == true) { s = s + sv.GetAllInvalidCharacters(); }
            }
            return s;
        }

        private int GetMaxLength()
        {
            int max= -1;
            foreach (StringValidation sv in this._validations)
            {
                if (sv.IsActive == true)
                { if (sv.MaxLength > max) { max = sv.MaxLength; } }
            }

            if (max == -1) { max = 32760; }
            return max;
        }

        private int GetMinLength()
        {
            int min = 32760;
            foreach (StringValidation sv in this._validations)
            {
                if (sv.IsActive == true)
                { if (sv.MinLength < min) { min = sv.MinLength; } }
            }

            if (min == 32760) { min = 0; }
            return min;
        }

        private StringValidation GetFirstActiveValidation()
        {
            foreach (StringValidation sv in this._validations)
            {
                if (sv.IsActive == true) { return sv; }
            }
            return null;
        }

        private string GetActiveValidationMessages()
        {
            string s = string.Empty;
            bool active = false;
            foreach (StringValidation sv in this._validations)
            {
                if ((sv.IsActive == true) && (!string.IsNullOrEmpty(sv.ValidationMessage)))
                {
                    s = s + Environment.NewLine + sv.ValidationMessage ;
                    active = true;
                }
            }
            if (active == true) { return s + Environment.NewLine; }
            else { return string.Empty; }
        }
    }
}