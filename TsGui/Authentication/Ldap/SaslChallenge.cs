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
    /// Carries the server's challenge data during a multi-step SASL bind exchange.
    /// </summary>
    public class SaslChallenge
    {
        public LdapResultCode ResultCode      { get; internal set; }
        public byte[]         ServerChallenge { get; internal set; }

        /// <summary>True when the server has reported a successful bind (no further steps needed).</summary>
        public bool IsComplete    => ResultCode == LdapResultCode.Success;

        /// <summary>True when the server expects another client response.</summary>
        public bool NeedsMoreData => ResultCode == LdapResultCode.SaslBindInProgress;
    }
}
