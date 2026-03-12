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
namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// BER (Basic Encoding Rules) tag constants used in LDAP messages.
    /// </summary>
    internal static class BerTag
    {
        // Universal primitive
        public const byte Boolean         = 0x01;
        public const byte Integer         = 0x02;
        public const byte OctetString     = 0x04;
        public const byte Null            = 0x05;
        public const byte Enumerated      = 0x0A;

        // Universal constructed
        public const byte Sequence        = 0x30;
        public const byte Set             = 0x31;

        // Application (LDAP-specific constructed)
        public const byte BindRequest     = 0x60;  // [APPLICATION 0]
        public const byte BindResponse    = 0x61;  // [APPLICATION 1]
        public const byte UnbindRequest   = 0x42;  // [APPLICATION 2] primitive
        public const byte SearchRequest   = 0x63;  // [APPLICATION 3]
        public const byte SearchResEntry  = 0x64;  // [APPLICATION 4]
        public const byte SearchResDone   = 0x65;  // [APPLICATION 5]
        public const byte SearchResRef    = 0x73;  // [APPLICATION 19]
        public const byte ModifyRequest   = 0x66;  // [APPLICATION 6]
        public const byte ModifyResponse  = 0x67;  // [APPLICATION 7]
        public const byte AddRequest      = 0x68;  // [APPLICATION 8]
        public const byte AddResponse     = 0x69;  // [APPLICATION 9]
        public const byte DelRequest      = 0x4A;  // [APPLICATION 10] primitive
        public const byte DelResponse     = 0x6B;  // [APPLICATION 11]
        public const byte ExtendedRequest = 0x77;  // [APPLICATION 23]
        public const byte ExtendedResponse= 0x78;  // [APPLICATION 24]

        // Context-specific primitive
        public const byte Context0        = 0x80;
        public const byte Context1        = 0x81;
        public const byte Context2        = 0x82;
        public const byte Context3        = 0x83;

        /// <summary>Context-specific primitive [7] — used for present filter and SASL server creds.</summary>
        public const byte Context7        = 0x87;

        // Context-specific constructed
        public const byte ContextC0       = 0xA0;  // constructed context [0]
        public const byte ContextC1       = 0xA1;
        public const byte ContextC2       = 0xA2;
        public const byte ContextC3       = 0xA3;
        public const byte ContextC4       = 0xA4;
        public const byte ContextC5       = 0xA5;
        public const byte ContextC6       = 0xA6;
        public const byte ContextC7       = 0xA7;

        /// <summary>Context-specific constructed [8] — used for approxMatch filter.</summary>
        public const byte ContextC8       = 0xA8;

        public const byte ContextC9       = 0xA9;
    }
}
