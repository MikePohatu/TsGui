#region license
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
using System.Collections.Generic;
using TsGui.View.GuiOptions;


namespace TsGui.Validation
{
    public static class ResultValidator
    {

        /// <summary>
        /// Truncate a string to the specified length
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string Truncate (string StringValue, int Length)
        {
            if (StringValue == null ) { return null; }
            if (StringValue.Length > Length) { return StringValue.Substring(0, Length); }
            else { return StringValue; }
        }

        /// <summary>
        /// Remove invalid characters from a string
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="InvalidChars"></param>
        /// <returns></returns>
        public static string RemoveInvalid (string StringValue, string InvalidChars)
        {
            if ((string.IsNullOrEmpty(StringValue)) || (string.IsNullOrEmpty(InvalidChars)))
                { return StringValue; }

            //Debug.WriteLine("RemoveInvalid called");
            char[] invalidchars = InvalidChars.ToCharArray();
            string newstring = StringValue;

            foreach (char c in invalidchars)
            {
                newstring = newstring.Replace(c.ToString(),string.Empty);
            }

            return newstring;
        }

        /// <summary>
        /// Find if StringValue contains specified characters. 
        /// Returns false if StringValue contains no invalid characters
        /// Returns true if String value is null, or if contains invalid characters
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Characters"></param>
        /// <param name="CaseSensitive"></param>
        /// <returns></returns>
        public static bool DoesStringContainCharacters(string SourceString, string Characters, bool CaseSensitive)
        {
            if (SourceString == null) { return false; }
            if (string.IsNullOrEmpty(Characters)) { return true; }
            
            //Debug.WriteLine("RemoveInvalid called");
            char[] invalidchars = Characters.ToCharArray();

            foreach (char c in invalidchars)
            {
                if (CaseSensitive == false)
                {
                    if (SourceString.ToUpper().Contains(c.ToString().ToUpper())) { return true; }
                }
                else
                {
                    if (SourceString.Contains(c.ToString())) { return true; }
                }             
            }

            return false;
        }

        /// <summary>
        /// Check if StringValue is too long. If StringValue is null,
        /// will return false. If MaxLength is zero, will return true.
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool ValidMaxLength(string StringValue,int MaxLength)
        {
            if (StringValue == null) { return false; }

            //if max length is 0, maxlength is undefined 
            if (MaxLength == 0 ) { return true; }
            if (StringValue.Length <= MaxLength) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Check if StringValue is too short. Will return false if StringValue
        /// is null.
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="MinLength"></param>
        /// <returns></returns>
        public static bool ValidMinLength(string StringValue, int MinLength)
        {
            if (StringValue == null) { return false; }
            //Debug.WriteLine(StringValue.Length);
            if (StringValue.Length >= MinLength) { return true; }
            else { return false; }
        } 

        public static bool AllOptionsValid(List<IValidationGuiOption> OptionList)
        {
            foreach (IValidationGuiOption option in OptionList)
            {
                if (option.IsActive == true && option.IsValid == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
