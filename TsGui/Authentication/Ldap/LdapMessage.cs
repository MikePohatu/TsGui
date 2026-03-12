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
using System.Collections.Generic;
using System.Text;

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Builds LDAP protocol messages as BER-encoded byte arrays (RFC 4511).
    /// </summary>
    internal static class LdapMessage
    {
        /// <summary>
        /// Wraps a protocol operation in a standard LDAPMessage envelope:
        ///   LDAPMessage ::= SEQUENCE { messageID MessageID, protocolOp CHOICE { ... }, controls [0] OPTIONAL }
        /// </summary>
        public static byte[] Wrap(int messageId, byte[] protocolOp, byte[] controls = null)
        {
            var inner = new BerWriter();
            inner.WriteInteger(messageId);
            inner.WriteRaw(protocolOp);
            if (controls != null)
                inner.WriteConstructed(BerTag.ContextC0, controls);

            return new BerWriter()
                .WriteSequence(inner.ToArray())
                .ToArray();
        }

        // ----------------------------------------------------------------
        // BindRequest  (RFC 4511 §4.2)
        // ----------------------------------------------------------------

        /// <summary>Builds a simple (plain-text password) BindRequest.</summary>
        public static byte[] BuildSimpleBindRequest(int msgId, string dn, string password)
        {
            var auth = new BerWriter()
                .WriteContextOctetString(BerTag.Context0, password ?? string.Empty)
                .ToArray();

            var body = new BerWriter()
                .WriteInteger(3)
                .WriteOctetString(dn ?? string.Empty)
                .WriteRaw(auth)
                .ToArray();

            var op = new BerWriter()
                .WriteConstructed(BerTag.BindRequest, body)
                .ToArray();

            return Wrap(msgId, op);
        }

        /// <summary>Builds one step of a SASL BindRequest. Pass null credentials for the initial empty step.</summary>
        public static byte[] BuildSaslBindRequest(int msgId, string dn,
            string mechanism, byte[] credentials = null)
        {
            var saslInner = new BerWriter();
            saslInner.WriteOctetString(mechanism);
            if (credentials != null && credentials.Length > 0)
                saslInner.WriteOctetString(credentials);

            var auth = new BerWriter()
                .WriteConstructed(BerTag.ContextC3, saslInner.ToArray())
                .ToArray();

            var body = new BerWriter()
                .WriteInteger(3)
                .WriteOctetString(dn ?? string.Empty)
                .WriteRaw(auth)
                .ToArray();

            var op = new BerWriter()
                .WriteConstructed(BerTag.BindRequest, body)
                .ToArray();

            return Wrap(msgId, op);
        }

        // ----------------------------------------------------------------
        // UnbindRequest  (RFC 4511 §4.3)
        // ----------------------------------------------------------------

        public static byte[] BuildUnbindRequest(int msgId)
        {
            var op = new byte[] { BerTag.UnbindRequest, 0x00 };
            return Wrap(msgId, op);
        }

        // ----------------------------------------------------------------
        // SearchRequest  (RFC 4511 §4.5.1)
        // ----------------------------------------------------------------

        public static byte[] BuildSearchRequest(
            int                msgId,
            string             baseDn,
            SearchScope        scope,
            DereferenceAliases derefAliases,
            int                sizeLimit,
            int                timeLimit,
            bool               typesOnly,
            byte[]             filterBytes,
            IList<string>      attributes)
        {
            var attrWriter = new BerWriter();
            if (attributes != null)
                foreach (var attr in attributes)
                    attrWriter.WriteOctetString(attr);

            var attrSeq = new BerWriter()
                .WriteSequence(attrWriter.ToArray())
                .ToArray();

            var body = new BerWriter()
                .WriteOctetString(baseDn ?? string.Empty)
                .WriteEnumerated((int)scope)
                .WriteEnumerated((int)derefAliases)
                .WriteInteger(sizeLimit)
                .WriteInteger(timeLimit)
                .WriteBoolean(typesOnly)
                .WriteRaw(filterBytes)
                .WriteRaw(attrSeq)
                .ToArray();

            var op = new BerWriter()
                .WriteConstructed(BerTag.SearchRequest, body)
                .ToArray();

            return Wrap(msgId, op);
        }

        // ----------------------------------------------------------------
        // AbandonRequest  (RFC 4511 §4.11)
        // ----------------------------------------------------------------

        public static byte[] BuildAbandonRequest(int msgId, int abandonMsgId)
        {
            var idBytes = new BerWriter().WriteInteger(abandonMsgId).ToArray();
            var op = new byte[idBytes.Length];
            Array.Copy(idBytes, op, idBytes.Length);
            op[0] = 0x50; // [APPLICATION 16] primitive
            return Wrap(msgId, op);
        }

        // ----------------------------------------------------------------
        // StartTLS ExtendedRequest  (RFC 4511 §4.14 / RFC 4513)
        // ----------------------------------------------------------------

        public static byte[] BuildStartTlsRequest(int msgId)
        {
            var body = new BerWriter()
                .WriteContextOctetString(BerTag.Context0, "1.3.6.1.4.1.1466.20037")
                .ToArray();

            var op = new BerWriter()
                .WriteConstructed(BerTag.ExtendedRequest, body)
                .ToArray();

            return Wrap(msgId, op);
        }
    }
}
