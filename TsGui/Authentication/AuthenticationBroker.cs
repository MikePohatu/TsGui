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

namespace TsGui.Authentication
{
    public class AuthenticationBroker
    {
        private IAuth _auth;
        private string _authid;

        public IPassword PasswordSource { get; set; }
        public IUsername UsernameSource { get; set; }
        public string AuthID { get { return this._authid; } }

        public void OnPasswordSourceChange()
        {
            this._auth.SecurePassword = this.PasswordSource.SecurePassword;
        }

        public void OnUsernameSourceChange()
        {
            this._auth.Username = this.UsernameSource.Username;
        }

        public AuthState Authenticate()
        { return this._auth.Authenticate(); }

        private void LoadXml(XElement inputxml)
        {
            this._authid = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", this._authid);
        }
    }
}
