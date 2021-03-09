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

// TsVariable.cs - the mapping of a variable name and value

using System;

namespace TsGui
{
    public class Variable
    {
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
            this.Name = Name;
            this.Value = Value;
            this.Path = Path;
        }
    }
}
