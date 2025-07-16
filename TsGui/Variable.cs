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

// TsVariable.cs - the mapping of a variable name and value

using Core.Logging;
using System;
using System.Linq;
using System.Text;
using TsGui.Config;

namespace TsGui
{
    public class Variable
    {
        public bool Hide { get; set; } = false;
        public string Path { get; set; }

        private string _value;
        public string Value
        {
            get { return this._value; }
            set
            {
                if (value == null) { this._value = string.Empty; }
                else { this._value = value; }
            }
        }

        private string _name;
        public string Name 
        {
            get { return this._name; } 
            set
            {
                if (value == null) { throw new InvalidOperationException("Variable name cannot be null"); }
                else { this._name = value; }
            }
        }

        public Variable (string Name, string Value, string Path)
        {
            string confirmedName;
            if (ConfirmValidName(Name, out confirmedName) == false)
            {
                Log.Warn($"Invalid TS variable name specified: {Name}. Valid name would be {confirmedName}");
            }
            this.Name = Name;
            this.Value = Value;
            this.Path = Path == null ? TsGuiRootConfig.DefaultPath : Path;
        }


        private static char[] _allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-1234567890".ToCharArray();
        private static char[] _allowedFirstCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// Confirrm a string is a valid variable name. 
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static bool ConfirmValidName(string variableName, out string validName)
        {
            //variable name rules
            //https://learn.microsoft.com/en-us/intune/configmgr/osd/understand/using-task-sequence-variables#bkmk_custom

            bool wasInputValid = true;
            if (string.IsNullOrWhiteSpace(variableName))
            {
                Log.Error("Variable name is empty");
                validName = string.Empty;
                return false;
            }

            var builder = new StringBuilder(variableName.Length);

            var nameArray = variableName.ToCharArray();
            for (int i = 0; i < nameArray.Length; i++)
            {
                char c = nameArray[i];

                //first letter rules:
                if (builder.Length == 0 && _allowedFirstCharacters.Contains(c) == false)
                {
                    wasInputValid = false;
                }
                //allowed character rules
                else if (_allowedCharacters.Contains(c))
                { builder.Append(c); }
                else
                { wasInputValid = false; }
            }

            validName = builder.ToString();

            if (string.IsNullOrEmpty(validName))
            {
                Log.Error($"Unable to create a valid variable name from '{variableName}'");
            }

            return wasInputValid;
        }
    }
}
