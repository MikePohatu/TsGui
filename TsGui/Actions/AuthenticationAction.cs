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
using TsGui.Authentication;
using Core.Diagnostics;
using System.Xml.Linq;

namespace TsGui.Actions
{
    public class AuthenticationAction: IAction, IAuthenticatorConsumer
    {
        private string _authid;

        public string AuthID { get { return this._authid; } }
        public IAuthenticator Authenticator { get; set; }

        public AuthenticationAction(XElement inputxml)
        {
            this.LoadXml(inputxml);
            //this._director.AuthLibrary.AddAuthenticator(new ActiveDirectoryAuthenticator(this._authid, this._domain));
            Director.Instance.AuthLibrary.AddAuthenticatorConsumer(this);
        }

        public void RunAction()
        {
            this.Authenticator?.Authenticate();
        }

        private void LoadXml(XElement inputxml)
        {
            this._authid = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", null);
            if (this._authid == null) { throw new KnownException("Missing AuthID attribute in config", inputxml.ToString()); }
        }
    }
}
