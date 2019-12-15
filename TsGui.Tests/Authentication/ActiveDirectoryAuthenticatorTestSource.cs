//    Copyright (C) 2017 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.


using System.Security;
using System.Net;
using TsGui.Authentication;

namespace TsGui.Tests.Authentication
{
    public class ActiveDirectoryAuthenticatorTestSource: IUsername, IPassword
    {
        private NetworkCredential _netcredential = new NetworkCredential();

        public string AuthID { get; set; }
        public string Username
        {
            get { return this._netcredential.UserName; }
            set { this._netcredential.UserName = value; }
        }
        public SecureString SecurePassword
        {
            get { return this._netcredential.SecurePassword; }
            set { this._netcredential.SecurePassword = value; }
        }
        public string Password { get { return this._netcredential.Password; } }
    }
}
