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

using TsGui.Authentication;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Diagnostics;
using System.Xml.Linq;

namespace TsGui.Actions
{
    public class ADAuthenticationAction: IAction, IAuthenticationComponent
    {
        private string _authid;
        private IDirector _director;
        private ActiveDirectoryAuthenticator _authenticator;
        private string _domain;

        public string AuthID { get { return this._authid; } }

        public ADAuthenticationAction(XElement inputxml, IDirector director)
        {
            this._director = director;
            this.LoadXml(inputxml);
            
        }

        public void RunAction()
        {
            this._authenticator.Authenticate();
        }

        private void LoadXml(XElement inputxml)
        {
            XElement xe = inputxml.Element("Domain");
            if (xe != null)
            {
                this._domain = xe.Value;
            }
            else { throw new TsGuiKnownException("Missing Domain in config", inputxml.ToString()); }

            XAttribute xa = inputxml.Attribute("AuthID");
            if (xa != null)
            {
                this._authid = xa.Value;
                this._authenticator = new ActiveDirectoryAuthenticator(this._authid,this._domain);
                this._director.AuthLibrary.AddAuthenticator(this._authenticator);
            }
            else { throw new TsGuiKnownException("Missing AuthID attribute in config", inputxml.ToString()); }

            
        }
    }
}
