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
    /// Represents one entry returned by a SearchResultEntry response.
    /// String values are UTF-8 decoded; binary values (e.g. objectSid, objectGUID)
    /// are accessible as raw bytes via <see cref="GetAttributeBytes"/>.
    /// </summary>
    public class LdapEntry
    {
        public string Dn { get; internal set; }

        /// <summary>Attribute name → list of UTF-8 decoded string values.</summary>
        public Dictionary<string, List<string>> Attributes { get; }
            = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Attribute name → list of raw byte values (for binary attributes like objectSid).</summary>
        internal Dictionary<string, List<byte[]>> RawAttributes { get; }
            = new Dictionary<string, List<byte[]>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Returns all string values for the named attribute, or an empty list if absent.</summary>
        public IList<string> GetAttribute(string name)
        {
            List<string> vals;
            return Attributes.TryGetValue(name, out vals) ? vals : new List<string>();
        }

        /// <summary>Returns the first string value of the named attribute, or null if absent.</summary>
        public string GetFirstValue(string name)
        {
            var vals = GetAttribute(name);
            return vals.Count > 0 ? vals[0] : null;
        }

        /// <summary>
        /// Returns the raw bytes of the first value of a binary attribute
        /// (e.g. objectSid, objectGUID). Returns null if the attribute is absent.
        /// Always prefer this over <see cref="GetFirstValue"/> for binary attributes.
        /// </summary>
        public byte[] GetAttributeBytes(string name)
        {
            List<byte[]> vals;
            if (RawAttributes.TryGetValue(name, out vals) && vals.Count > 0)
                return vals[0];
            return null;
        }

        /// <summary>Returns all raw byte values for a multi-valued binary attribute.</summary>
        public IList<byte[]> GetAttributeAllBytes(string name)
        {
            List<byte[]> vals;
            return RawAttributes.TryGetValue(name, out vals) ? vals : new List<byte[]>();
        }

        public override string ToString() => Dn;
    }
}
