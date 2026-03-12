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
    /// Controls how alias entries are dereferenced during an LDAP search (RFC 4511 §4.5.1).
    /// </summary>
    public enum DereferenceAliases
    {
        NeverDerefAliases   = 0,
        DerefInSearching    = 1,
        DerefFindingBaseObj = 2,
        DerefAlways         = 3,
    }
}
