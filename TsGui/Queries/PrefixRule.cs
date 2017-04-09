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

// Prefix.cs - used to add a string to the start of a string result

using System.Xml.Linq;

namespace TsGui.Queries
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
