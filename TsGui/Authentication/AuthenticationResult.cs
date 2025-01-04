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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsGui.Authentication
{
    public class AuthenticationResult
    {
        /// <summary>
        /// Dictionary with group or membership name as, is member etc as the value
        /// </summary>
        public Dictionary<string, bool> Memberships { get; set; } = null;

        /// <summary>
        /// The authentication process state i.e. NotAuthed, AccessDenied, Authorised, NotAuthorised, AuthError, NoPassword
        /// </summary>
        public AuthState State { get; set; } = AuthState.NotAuthed;

        public AuthenticationResult() { }
        
        public AuthenticationResult(AuthState state, Dictionary<string, bool> memberships)
        {
            State = state;
            Memberships = memberships;
        }

        public AuthenticationResult(AuthState state)
        {
            State = state;
        }
    }
}
