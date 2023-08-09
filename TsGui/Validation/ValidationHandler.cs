#region license
// Copyright (c) 2020 Mike Pohatu
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

// ValidationHandler.cs - maintains a list of stringvalidation objects. returns correct values taking
// into account group membership

using System;
using System.Xml.Linq;
using System.Collections.Generic;
using TsGui.Linking;
using Core.Logging;
using MessageCrap;
using System.Threading.Tasks;
using TsGui.View.GuiOptions;

namespace TsGui.Validation
{
    public class ValidationHandler: ILinkTarget
    {
        private List<StringValidation> _validations = new List<StringValidation>();
        private XElement _legacyxml = new XElement("Legacy");
        private IValidationOwner _owner;
        //Properties
        #region
        /// <summary>
        /// Is the validation handler enabled i.e. will validation be carried out
        /// </summary>
        public bool Enabled { get; set; } = true;
        public string ValidationMessage { get { return this.GetActiveValidationMessages(); } }
        public string FailedValidationMessage { get; set; }
        public int MinLength { get { return this.GetMinLength(); } }
        public int MaxLength { get { return this.GetMaxLength(); } }
        #endregion

        public ValidationToolTipHandler ToolTipHandler { get; private set; }
        public ValidationHandler(IValidationOwner Owner)
        {
            this._owner = Owner;
            this.ToolTipHandler = new ValidationToolTipHandler(Owner as GuiOptionBase);
        }

        private void AddValidation(XElement InputXml)
        {
            if (InputXml == null) { return; }
            StringValidation sv = new StringValidation(this);
            sv.LoadXml(InputXml);
            this.AddValidation(sv);
        }

        private void AddValidation(StringValidation Validation)
        {
            this._validations.Add(Validation);
            Validation.RevalidationRequired += this._owner.OnValidationChange;
        }

        private void AddValidations(IEnumerable<XElement> InputXmlList)
        {
            if (InputXmlList == null) { return; }
            foreach (XElement xval in InputXmlList)
            { this.AddValidation(xval); }   
        }

        public void LoadXml(XElement InputXml)
        {
            //region load legacy
            XElement x;
            XElement result = new XElement("Legacy");
            //load legacy options
            x = InputXml.Element("Disallowed");
            if (x != null) { result.Add(x); }

            x = InputXml.Element("MinLength");
            if (x != null) { result.Add(x); }

            StringValidation sv = new StringValidation(this);
            sv.LoadLegacyXml(result);
            this.AddValidation(sv);
            //endregion

            this.AddValidations(InputXml.Elements("Validation"));
        }

        public bool IsValid(string Input)
        {
            if (this.Enabled == false) { return true; }

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
            if (this.Enabled == false) { return true; }
            StringValidation sv = this.GetFirstActiveValidation();
            if (sv != null) { return sv.IsShorterThanMinLength(Input); }
            else { return false; }
        }

        private bool IsLongerThanMaxLength(string Input)
        {
            if (this.Enabled == false) { return true; }
            StringValidation sv = this.GetFirstActiveValidation();
            if (sv != null) { return sv.IsLongerThanMaxLength(Input); }
            else { return false; }
        }

        private bool IsInvalidMatched(string Input)
        {
            if (this.Enabled == false) { return true; }
            StringValidation sv = this.GetFirstActiveValidation();
            if (sv != null) { return sv.IsInvalidMatched(Input); }
            else { return false; }
        }

        private bool IsValidMatched(string Input)
        {
            if (this.Enabled == false) { return true; }
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

        public async Task OnSourceValueUpdatedAsync(Message message) 
        {
            Log.Info("Validation refresh requested");
            await Task.CompletedTask;
            this._owner.OnValidationChange();
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