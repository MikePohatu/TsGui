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

namespace TsGui.Authentication.Ldap
{
    /// <summary>
    /// Thrown when an LDAP operation fails.
    /// </summary>
    public class LdapException : Exception
    {
        public LdapResultCode ResultCode { get; }

        public LdapException(string message)
            : base(message) { }

        public LdapException(LdapResultCode code, string message)
            : base($"LDAP error {(int)code} ({code}): {message}")
        {
            ResultCode = code;
        }

        public LdapException(string message, Exception inner)
            : base(message, inner) { }
    }
}
