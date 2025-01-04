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

// Prefix.cs - used to add a string to the start of a string result

using System.Xml.Linq;

namespace TsGui.Queries.Rules
{
    public class PrefixRule: IQueryRule
    {
        private string _prefixstring;

        public PrefixRule(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        private void LoadXml(XElement InputXml)
        {
            this._prefixstring = InputXml.Value;
        }

        public string Process(string Input)
        {
            return _prefixstring + Input;
        }
    }
}
