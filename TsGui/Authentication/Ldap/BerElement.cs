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
using System.Text;

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// A single decoded BER TLV (tag, length, value) element.
    /// </summary>
    internal class BerElement
    {
        public byte   Tag   { get; }
        public byte[] Value { get; }

        public BerElement(byte tag, byte[] value)
        {
            Tag   = tag;
            Value = value ?? new byte[0];
        }

        /// <summary>Returns a <see cref="BerReader"/> positioned at the start of this element's value.</summary>
        public BerReader GetReader() => new BerReader(Value);

        public string ValueAsString()  => Encoding.UTF8.GetString(Value);
        public int    ValueAsInteger() => BerReader.DecodeInteger(Value);
    }
}
