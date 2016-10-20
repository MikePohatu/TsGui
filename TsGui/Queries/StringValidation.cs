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

namespace TsGui.Queries
{
    public class StringValidation
    {
        private bool _validateempty;
        private bool _caseSensValidate;
        private int _maxlength;
        private int _minlength;

        //Properties

        #region
        public string ValidationMessage { get; set; }
        public string DisallowedCharacters { get; set; }
        public bool CaseSensitive
        {
            get { return this._caseSensValidate; }
            set { this._caseSensValidate = value; }
        }
        protected int MinLength
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

        public bool IsValidString(string Text)
        {
            //Debug.WriteLine("Validate started. Text: " + this._control.Text);
            string input = Text;
            string s = "";
            bool valid = true;

            if ((this._validateempty == false) && (string.IsNullOrEmpty(input)))
            { valid = true; }
            else
            {
                if (ResultValidator.ValidCharacters(input, this.DisallowedCharacters, this.CaseSensitive) != true)
                {
                    s = "Invalid characters: " + this.DisallowedCharacters + Environment.NewLine;
                    valid = false;
                }

                if (ResultValidator.ValidMinLength(input, this.MinLength) == false)
                {
                    string charWord;
                    if (this.MinLength == 1) { charWord = " character"; }
                    else { charWord = " characters"; }

                    s = s + "Minimum length";
                    if (this._validateempty == false) { s = s + " if entered"; }
                    s = s + ": " + this.MinLength + charWord + Environment.NewLine;
                    valid = false;
                }
            }

            if (valid == false)
            {
                if (input.Length == 0)
                {
                    s = "Required" + Environment.NewLine + Environment.NewLine + s;
                }
                else
                {
                    s = "\"" + input + "\" is invalid:" + Environment.NewLine + Environment.NewLine + s;
                }
            }

            this.ValidationMessage = s;
            return valid;
        }
    }
}
