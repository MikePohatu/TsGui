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
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Net;

namespace TsGui.Authentication.ActiveDirectory
{
    public class ActiveDirectoryAuthentication : IAuth
    {
        private NetworkCredential _netcredential;
        private AuthState _state;

        public PrincipalContext Context { get; set; }
        public AuthState State { get { return this._state; } }
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
        public List<string> RequiredGroups { get; set; } 

        public ActiveDirectoryAuthentication(string authuser, SecureString authpw, string domain, List<string> groups)
        {
            this._state = AuthState.AccessDenied;
            this._netcredential = new NetworkCredential(authuser, authpw, domain);
            this.RequiredGroups = groups;
        }

        public AuthState Authenticate()
        {
            try
            {
                this.Context = new PrincipalContext(ContextType.Domain, this._netcredential.Domain, this._netcredential.UserName, this._netcredential.Password);
                if (ActiveDirectoryMethods.IsUserMemberOfGroups(this.Context,this._netcredential.UserName,this.RequiredGroups) == true)
                { return AuthState.Authorised; }
                else { return AuthState.NotAuthorised; }
                
            }
            catch
            {
                return AuthState.AccessDenied;
            }

        }

        //private bool IsValidLogin()
        //{
        //    bool authed = false;
        //    try
        //    {
        //        DirectoryEntry entry = new DirectoryEntry("LDAP://" + this.Domain, this._netcredential.UserName, this._netcredential.Password);
        //        object nativeObject = entry.NativeObject;
        //        authed = true;
        //    }
        //    catch (DirectoryServicesCOMException) { }

        //    return authed;
        //}
    }
}
