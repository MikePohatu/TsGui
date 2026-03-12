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
    /// LDAP search scope (RFC 4511 §4.5.1).
    /// </summary>
    public enum SearchScope
    {
        /// <summary>Only the base object itself.</summary>
        Base     = 0,
        /// <summary>One level below the base object.</summary>
        OneLevel = 1,
        /// <summary>The base object and all its descendants.</summary>
        Subtree  = 2,
    }
}
