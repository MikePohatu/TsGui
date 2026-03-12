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
    /// Encodes RFC 4515 LDAP search filter strings into BER-encoded bytes.
    ///
    /// Supported filter types:
    ///   (attr=*)          present
    ///   (attr=value)      equality
    ///   (attr=*sub*)      substring (initial / any / final)
    ///   (attr>=value)     greaterOrEqual
    ///   (attr&lt;=value)     lessOrEqual
    ///   (attr~=value)     approxMatch
    ///   (&amp;(f1)(f2))       AND
    ///   (|(f1)(f2))       OR
    ///   (!(f1))           NOT
    /// </summary>
    public static class LdapFilter
    {
        /// <summary>
        /// Parses <paramref name="filter"/> and returns its BER encoding.
        /// </summary>
        /// <exception cref="ArgumentException">Filter string is null or empty.</exception>
        /// <exception cref="LdapException">Filter string cannot be parsed.</exception>
        public static byte[] Parse(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("Filter must not be empty", "filter");

            filter = filter.Trim();

            // Strip balanced outer parentheses
            if (filter.StartsWith("(") && filter.EndsWith(")"))
            {
                string inner = filter.Substring(1, filter.Length - 2);
                if (IsBalanced(inner))
                    filter = inner;
            }

            // Composite filters
            if (filter.StartsWith("&")) return EncodeComposite(BerTag.ContextC0, filter.Substring(1));
            if (filter.StartsWith("|")) return EncodeComposite(BerTag.ContextC1, filter.Substring(1));
            if (filter.StartsWith("!")) return EncodeNot(filter.Substring(1));

            // Simple attribute filters — check multi-char operators first
            int geIdx = filter.IndexOf(">=", StringComparison.Ordinal);
            int leIdx = filter.IndexOf("<=", StringComparison.Ordinal);
            int approxIdx = filter.IndexOf("~=", StringComparison.Ordinal);
            int eqIdx = filter.IndexOf('=');

            if (geIdx > 0) return EncodeSimple(BerTag.ContextC5, filter, geIdx, 2); // greaterOrEqual
            if (leIdx > 0) return EncodeSimple(BerTag.ContextC6, filter, leIdx, 2); // lessOrEqual
            if (approxIdx > 0) return EncodeSimple(BerTag.ContextC8, filter, approxIdx, 2); // approxMatch

            if (eqIdx > 0)
            {
                string attr = filter.Substring(0, eqIdx);
                string val = filter.Substring(eqIdx + 1);

                if (val == "*") return EncodePresentFilter(attr);
                if (val.Contains("*")) return EncodeSubstringFilter(attr, val);

                return EncodeSimple(BerTag.ContextC3, filter, eqIdx, 1); // equality
            }

            throw new LdapException($"Cannot parse filter: {filter}");
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private static bool IsBalanced(string s)
        {
            int depth = 0;
            foreach (char c in s)
            {
                if (c == '(') depth++;
                else if (c == ')') depth--;
                if (depth < 0) return false;
            }
            return depth == 0;
        }

        private static byte[] EncodeComposite(byte tag, string rest)
        {
            var inner = new BerWriter();
            foreach (var part in SplitFilters(rest))
                inner.WriteRaw(Parse(part));
            return new BerWriter().WriteConstructed(tag, inner.ToArray()).ToArray();
        }

        private static byte[] EncodeNot(string rest)
        {
            var parts = SplitFilters(rest);
            if (parts.Count != 1)
                throw new LdapException("NOT filter must have exactly one operand");
            return new BerWriter()
                .WriteConstructed(BerTag.ContextC2, Parse(parts[0]))
                .ToArray();
        }

        /// <summary>Splits a string like "(f1)(f2)(f3)" into ["(f1)", "(f2)", "(f3)"].</summary>
        private static List<string> SplitFilters(string s)
        {
            var result = new List<string>();
            int depth = 0, start = -1;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(' && depth++ == 0) start = i;
                else if (s[i] == ')' && --depth == 0)
                    result.Add(s.Substring(start, i - start + 1));
            }
            return result;
        }

        private static byte[] EncodeSimple(byte tag, string filter, int opIdx, int opLen)
        {
            string attr = filter.Substring(0, opIdx);
            string val = filter.Substring(opIdx + opLen);
            var inner = new BerWriter()
                .WriteOctetString(attr)
                .WriteOctetString(UnescapeFilterValue(val))
                .ToArray();
            return new BerWriter().WriteConstructed(tag, inner).ToArray();
        }

        /// <summary>
        /// Decodes RFC 4515 filter value escaping: converts \xx hex sequences to their
        /// raw byte values. Plain text characters are UTF-8 encoded as-is.
        /// This is required for binary attribute filters such as objectSid and objectGUID,
        /// where the value is expressed as \xx\xx\xx... escaped hex bytes.
        /// </summary>
        private static byte[] UnescapeFilterValue(string val)
        {
            // Fast path: no escaping present
            if (!val.Contains("\\"))
                return Encoding.UTF8.GetBytes(val);

            var result = new List<byte>();
            int i = 0;
            while (i < val.Length)
            {
                if (val[i] == '\\' && i + 2 < val.Length)
                {
                    string hex = val.Substring(i + 1, 2);
                    byte b;
                    if (byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out b))
                    {
                        result.Add(b);
                        i += 3;
                        continue;
                    }
                }
                // Plain character — encode as UTF-8
                foreach (byte b in Encoding.UTF8.GetBytes(new char[] { val[i] }))
                    result.Add(b);
                i++;
            }
            return result.ToArray();
        }

        private static byte[] EncodePresentFilter(string attr)
        {
            // present [7] AttributeDescription
            return new BerWriter()
                .WriteContextOctetString(BerTag.Context7, attr)
                .ToArray();
        }

        private static byte[] EncodeSubstringFilter(string attr, string val)
        {
            // SubstringFilter ::= SEQUENCE { type AttributeDescription, substrings SEQUENCE OF CHOICE {
            //     initial [0] AssertionValue, any [1] AssertionValue, final [2] AssertionValue } }
            string[] parts = val.Split('*');
            var subsWriter = new BerWriter();

            if (!string.IsNullOrEmpty(parts[0]))
                subsWriter.WriteTlv(BerTag.Context0, UnescapeFilterValue(parts[0])); // initial

            for (int i = 1; i < parts.Length - 1; i++)
                if (!string.IsNullOrEmpty(parts[i]))
                    subsWriter.WriteTlv(BerTag.Context1, UnescapeFilterValue(parts[i])); // any

            if (parts.Length > 1 && !string.IsNullOrEmpty(parts[parts.Length - 1]))
                subsWriter.WriteTlv(BerTag.Context2, UnescapeFilterValue(parts[parts.Length - 1])); // final

            var substringsSeq = new BerWriter().WriteSequence(subsWriter.ToArray()).ToArray();
            var inner = new BerWriter()
                .WriteOctetString(attr)
                .WriteRaw(substringsSeq)
                .ToArray();

            return new BerWriter().WriteConstructed(BerTag.ContextC4, inner).ToArray();
        }
    }
}