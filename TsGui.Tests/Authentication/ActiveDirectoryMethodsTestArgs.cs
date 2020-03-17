#region license
// Copyright (c) 2020 Mike Pohatu
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
using TsGui.Authentication;

namespace TsGui.Tests.Authentication
{
    public class ActiveDirectoryMethodsTestArgs
    {
        public ActiveDirectoryAuthenticatorTestArgs AuthArgs { get; set; }
        public bool ExpectedResult { get; set; }
        public string UserName { get; set; }
        public List<string> Groups { get; set; }

        public ActiveDirectoryMethodsTestArgs(ActiveDirectoryAuthenticatorTestArgs authargs, string user, List<string> groups, bool expectedresult)
        {
            this.AuthArgs = authargs;
            this.UserName = user;
            this.Groups = groups;
            this.ExpectedResult = expectedresult;
        }
    }
}
