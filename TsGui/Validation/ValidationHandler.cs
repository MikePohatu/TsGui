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
using TsGui.Grouping;

namespace TsGui.Validation
{
    public class ValidationHandler
    {
        private MainController _controller;
        private List<StringValidation> _validations = new List<StringValidation>();
        private XElement _legacyxml = new XElement("Legacy");
        //Properties

        #region
        public string ValidationMessage { get { return this.GetActiveValidation()?.ValidationMessage; } }
        public string FailedValidationMessage { get; set; }
        public int MinLength { get { return this.GetMinLength(); } }
        public int MaxLength { get { return this.GetMaxLength(); } }
        #endregion

        public ValidationHandler(MainController MainController)
        {
            this._controller = MainController;
        }

        public void AddValidation(XElement InputXml)
        {
            if (InputXml == null) { return; }
            StringValidation sv = new StringValidation(this._controller);
            sv.LoadLegacyXml(this._legacyxml);
            sv.LoadXml(InputXml);
            this._validations.Add(sv);
        }

        public void LoadLegacyXml(XElement InputXml)
        {
            XElement x;
            //load legacy options
            x = InputXml.Element("Disallowed");
            if (x != null) { this._legacyxml.Add(x); }

            x = InputXml.Element("MinLength");
            if (x != null) { this._legacyxml.Add(x); }

            x = InputXml.Element("MaxLength");
            if (x != null) { this._legacyxml.Add(x); }
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
            StringValidation sv = this.GetActiveValidation();
            if (sv != null) { return sv.IsShorterThanMinLength(Input); }
            else { return false; }
        }

        private bool IsLongerThanMaxLength(string Input)
        {
            StringValidation sv = this.GetActiveValidation();
            if (sv != null) { return sv.IsLongerThanMaxLength(Input); }
            else { return false; }
        }

        private bool IsInvalidMatched(string Input)
        {
            StringValidation sv = this.GetActiveValidation();
            if (sv != null) { return sv.IsInvalidMatched(Input); }
            else { return false; }
        }

        private bool IsValidMatched(string Input)
        {
            StringValidation sv = this.GetActiveValidation();
            if (sv != null) { return sv.IsValidMatched(Input); }
            else { return true; }
        }

        public string GetAllInvalidCharacters()
        {
            StringValidation sv = this.GetActiveValidation();
            if (sv != null) { return sv.GetAllInvalidCharacters(); }
            else { return null; }
        }

        private int GetMaxLength()
        {
            StringValidation sv = this.GetActiveValidation();
            if (sv != null) { return sv.MaxLength; }
            else { return 32760; }
        }

        private int GetMinLength()
        {
            StringValidation sv = this.GetActiveValidation();
            if (sv != null) { return sv.MinLength; }
            else { return 0; }
        }

        private StringValidation GetActiveValidation()
        {
            foreach (StringValidation sv in this._validations)
            {
                if (sv.IsActive == true) { return sv; }
            }
            return null;
        }

        
    }
}