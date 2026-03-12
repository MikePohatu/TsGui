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
using System.Text;

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Parses raw bytes arriving on the wire into typed LDAP response objects.
    /// </summary>
    internal static class LdapResponseParser
    {
        // ----------------------------------------------------------------
        // Top-level message framing
        // ----------------------------------------------------------------

        /// <summary>
        /// Attempts to decode one complete LDAPMessage from <paramref name="buf"/>.
        /// Returns false (and leaves the buffer untouched) if more bytes are needed.
        /// On success, sets <paramref name="consumed"/> to the total bytes read,
        /// <paramref name="messageId"/> to the message identifier, and
        /// <paramref name="protocolOp"/> to the inner protocol-operation element.
        /// </summary>
        public static bool TryReadMessage(
            byte[]        buf,
            int           available,
            out int       consumed,
            out int       messageId,
            out BerElement protocolOp)
        {
            consumed = 0; messageId = 0; protocolOp = null;
            if (available < 2) return false;

            if (buf[0] != BerTag.Sequence) return false;

            int headerLen;
            int valueLen = PeekLength(buf, 1, available, out headerLen);
            if (valueLen < 0) return false;

            int total = 1 + headerLen + valueLen;
            if (available < total) return false;

            consumed = total;
            var reader = new BerReader(buf, 1 + headerLen);

            var msgIdEl = reader.ReadElement();
            messageId   = BerReader.DecodeInteger(msgIdEl.Value);
            protocolOp  = reader.ReadElement();
            return true;
        }

        // ----------------------------------------------------------------
        // Response decoders
        // ----------------------------------------------------------------

        /// <summary>
        /// Decodes the shared LDAPResult structure used by BindResponse,
        /// SearchResultDone, ModifyResponse, AddResponse, DeleteResponse, etc.
        /// </summary>
        public static LdapResult ParseLdapResult(int messageId, BerElement op)
        {
            var reader       = op.GetReader();
            var codeEl       = reader.ReadElement();
            int code         = BerReader.DecodeInteger(codeEl.Value);
            string matchedDn = reader.ReadOctetString();
            string diagMsg   = reader.ReadOctetString();

            var result = new LdapResult
            {
                MessageId         = messageId,
                ResultCode        = (LdapResultCode)code,
                MatchedDn         = matchedDn,
                DiagnosticMessage = diagMsg,
            };

            // Optional referrals [3] SEQUENCE OF URI
            if (reader.HasMore)
            {
                var el = reader.ReadElement();
                if (el.Tag == BerTag.ContextC3)
                {
                    var refReader = el.GetReader();
                    while (refReader.HasMore)
                        result.Referrals.Add(refReader.ReadOctetString());
                }
            }

            return result;
        }

        /// <summary>Decodes a SearchResultEntry (APPLICATION 4) into an <see cref="LdapEntry"/>.</summary>
        public static LdapEntry ParseSearchEntry(BerElement op)
        {
            var reader = op.GetReader();
            var entry  = new LdapEntry { Dn = reader.ReadOctetString() };

            // PartialAttributeList ::= SEQUENCE OF PartialAttribute
            var attrListEl = reader.ReadElement();
            var attrReader = attrListEl.GetReader();

            while (attrReader.HasMore)
            {
                // PartialAttribute ::= SEQUENCE { type AttributeDescription, vals SET OF AttributeValue }
                var partAttr = attrReader.ReadElement();
                var paReader = partAttr.GetReader();

                string attrType   = paReader.ReadOctetString();
                var    valsEl     = paReader.ReadElement();
                var    valsReader = valsEl.GetReader();

                var values    = new List<string>();
                var rawValues = new List<byte[]>();

                while (valsReader.HasMore)
                {
                    var valEl = valsReader.ReadElement();
                    rawValues.Add(valEl.Value);
                    values.Add(Encoding.UTF8.GetString(valEl.Value));
                }

                entry.Attributes[attrType]    = values;
                entry.RawAttributes[attrType] = rawValues;
            }

            return entry;
        }

        /// <summary>Decodes a BindResponse that carries a SASL server challenge.</summary>
        public static SaslChallenge ParseSaslChallenge(int messageId, BerElement op)
        {
            var reader = op.GetReader();
            var codeEl = reader.ReadElement();
            int code   = BerReader.DecodeInteger(codeEl.Value);

            reader.ReadOctetString(); // matchedDN (discard)
            reader.ReadOctetString(); // diagnosticMessage (discard)

            byte[] serverCreds = null;
            if (reader.HasMore)
            {
                var el = reader.ReadElement();
                if (el.Tag == BerTag.Context7 || el.Tag == BerTag.Context0)
                    serverCreds = el.Value;
            }

            return new SaslChallenge
            {
                ResultCode      = (LdapResultCode)code,
                ServerChallenge = serverCreds ?? new byte[0],
            };
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Peeks at the BER length field at <paramref name="buf"/>[<paramref name="offset"/>]
        /// without advancing any external position.
        /// Returns the encoded value length, or -1 if not enough bytes are available.
        /// Sets <paramref name="headerLen"/> to the number of bytes the length field occupies.
        /// </summary>
        private static int PeekLength(byte[] buf, int offset, int available, out int headerLen)
        {
            headerLen = 0;
            if (offset >= available) return -1;

            byte first = buf[offset];
            if ((first & 0x80) == 0)
            {
                headerLen = 1;
                return first;
            }

            int numBytes = first & 0x7F;
            if (offset + 1 + numBytes > available) return -1;

            headerLen = 1 + numBytes;
            int length = 0;
            for (int i = 0; i < numBytes; i++)
                length = (length << 8) | buf[offset + 1 + i];
            return length;
        }
    }
}
