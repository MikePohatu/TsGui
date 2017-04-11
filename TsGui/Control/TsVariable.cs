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

// TsVariable.cs - the mapping of a variable name and value

using System;

namespace TsGui
{
    public class TsVariable
    {
        private string _name;
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
        public string Name 
        {
            get { return this._name; } 
            set
            {
                if (value == null) { throw new InvalidOperationException("TsVariable name cannot be null"); }
                else { this._name = value; }
            }
        }

        public TsVariable (string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
