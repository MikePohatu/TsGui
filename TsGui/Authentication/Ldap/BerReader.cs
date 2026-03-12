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
using System;
using System.Text;

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Reads BER-encoded TLV elements sequentially from a byte buffer.
    /// </summary>
    internal class BerReader
    {
        private readonly byte[] _data;
        private int _pos;

        public BerReader(byte[] data, int offset = 0)
        {
            _data = data;
            _pos  = offset;
        }

        public bool HasMore  => _pos < _data.Length;
        public int  Position => _pos;

        public BerElement ReadElement()
        {
            if (_pos >= _data.Length)
                throw new LdapException("BER read past end of buffer");

            byte   tag    = _data[_pos++];
            int    length = ReadLength();
            byte[] value  = new byte[length];
            Array.Copy(_data, _pos, value, 0, length);
            _pos += length;
            return new BerElement(tag, value);
        }

        public int    ReadInteger()      { var el = ReadElement(); return DecodeInteger(el.Value); }
        public int    ReadEnumerated()   { var el = ReadElement(); return DecodeInteger(el.Value); }
        public string ReadOctetString()  { var el = ReadElement(); return Encoding.UTF8.GetString(el.Value); }
        public byte[] ReadOctetStringBytes() { var el = ReadElement(); return el.Value; }

        private int ReadLength()
        {
            byte first = _data[_pos++];
            if ((first & 0x80) == 0) return first;

            int numBytes = first & 0x7F;
            int length   = 0;
            for (int i = 0; i < numBytes; i++)
                length = (length << 8) | _data[_pos++];
            return length;
        }

        /// <summary>Decodes a big-endian, two's-complement BER integer from raw bytes.</summary>
        public static int DecodeInteger(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return 0;
            int result = (bytes[0] & 0x80) != 0 ? -1 : 0;
            foreach (byte b in bytes)
                result = (result << 8) | b;
            return result;
        }
    }
}
