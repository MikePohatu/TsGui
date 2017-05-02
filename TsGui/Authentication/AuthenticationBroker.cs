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

using System.Xml.Linq;
using TsGui.Diagnostics.Logging;

namespace TsGui.Authentication
{
    public class AuthenticationBroker
    {
        //protected IAuthenticator _authenticator;
        private string _authid;

        public IAuthenticator Authenticator { get; set; }
        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string AuthID { get { return this._authid; } }

        public AuthenticationBroker(string authid)
        { this._authid = authid; }

        public AuthState Authenticate()
        {
            AuthState retstate;
            if (this.Authenticator != null ) { retstate = this.Authenticator.Authenticate(); }
            else { retstate = AuthState.AuthError; }

            LoggerFacade.Info("Authentication broker returned " + retstate.ToString());
            return retstate;
        }
    }
}
