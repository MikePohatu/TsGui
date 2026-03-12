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

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Represents a decoded LDAP result returned by bind, search-done,
    /// modify, delete, and other operations.
    /// </summary>
    public class LdapResult
    {
        public int           MessageId         { get; internal set; }
        public LdapResultCode ResultCode       { get; internal set; }
        public string        MatchedDn         { get; internal set; }
        public string        DiagnosticMessage { get; internal set; }
        public IList<string> Referrals         { get; internal set; } = new List<string>();

        public bool IsSuccess => ResultCode == LdapResultCode.Success;

        public override string ToString() =>
            $"[{ResultCode}] {DiagnosticMessage} (matchedDN={MatchedDn})";
    }
}
