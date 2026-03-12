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
using System.IO;
using System.Text;

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Writes BER-encoded data to an in-memory byte buffer.
    /// All Write methods return <c>this</c> to support chaining.
    /// </summary>
    internal class BerWriter
    {
        private readonly MemoryStream _ms = new MemoryStream();

        public byte[] ToArray() => _ms.ToArray();

        public BerWriter WriteRaw(byte[] data)
        {
            _ms.Write(data, 0, data.Length);
            return this;
        }

        public BerWriter WriteTlv(byte tag, byte[] value)
        {
            _ms.WriteByte(tag);
            WriteLength(value.Length);
            _ms.Write(value, 0, value.Length);
            return this;
        }

        public BerWriter WriteInteger(int value)
        {
            return WriteTlv(BerTag.Integer, EncodeInteger(value));
        }

        public BerWriter WriteEnumerated(int value)
        {
            return WriteTlv(BerTag.Enumerated, EncodeInteger(value));
        }

        public BerWriter WriteBoolean(bool value)
        {
            return WriteTlv(BerTag.Boolean, new byte[] { value ? (byte)0xFF : (byte)0x00 });
        }

        public BerWriter WriteOctetString(string value)
        {
            var bytes = value == null ? new byte[0] : Encoding.UTF8.GetBytes(value);
            return WriteTlv(BerTag.OctetString, bytes);
        }

        public BerWriter WriteOctetString(byte[] value)
        {
            return WriteTlv(BerTag.OctetString, value ?? new byte[0]);
        }

        public BerWriter WriteNull()
        {
            _ms.WriteByte(BerTag.Null);
            _ms.WriteByte(0x00);
            return this;
        }

        public BerWriter WriteContextOctetString(byte contextTag, string value)
        {
            var bytes = value == null ? new byte[0] : Encoding.UTF8.GetBytes(value);
            return WriteTlv(contextTag, bytes);
        }

        public BerWriter WriteSequence(byte[] innerBytes)
        {
            return WriteTlv(BerTag.Sequence, innerBytes);
        }

        public BerWriter WriteConstructed(byte tag, byte[] innerBytes)
        {
            return WriteTlv(tag, innerBytes);
        }

        private void WriteLength(int length)
        {
            if (length < 0x80)
            {
                _ms.WriteByte((byte)length);
            }
            else if (length <= 0xFF)
            {
                _ms.WriteByte(0x81);
                _ms.WriteByte((byte)length);
            }
            else if (length <= 0xFFFF)
            {
                _ms.WriteByte(0x82);
                _ms.WriteByte((byte)(length >> 8));
                _ms.WriteByte((byte)(length & 0xFF));
            }
            else
            {
                _ms.WriteByte(0x83);
                _ms.WriteByte((byte)(length >> 16));
                _ms.WriteByte((byte)(length >> 8));
                _ms.WriteByte((byte)(length & 0xFF));
            }
        }

        private static byte[] EncodeInteger(int value)
        {
            if (value == 0) return new byte[] { 0x00 };
            var list = new List<byte>();
            int v = value;
            while (v != 0 && v != -1)
            {
                list.Insert(0, (byte)(v & 0xFF));
                v >>= 8;
            }
            bool negative = (value < 0);
            if (negative && (list[0] & 0x80) == 0) list.Insert(0, 0xFF);
            if (!negative && (list[0] & 0x80) != 0) list.Insert(0, 0x00);
            return list.ToArray();
        }
    }
}
