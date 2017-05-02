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

using System.Collections.Generic;

namespace TsGui.Authentication
{
    public class AuthLibrary
    {
        private Dictionary<string, IUsername> _usernames = new Dictionary<string, IUsername>();
        private Dictionary<string, IPassword> _passwords = new Dictionary<string, IPassword>();
        private Dictionary<string, IAuthenticator> _authenticators = new Dictionary<string, IAuthenticator>();
        //private Dictionary<string, IAuthBrokerConsumer> _pendingbrokerconsumers = new Dictionary<string, IAuthBrokerConsumer>();

        public IUsername GetUsername(string ID)
        {
            IUsername option;
            this._usernames.TryGetValue(ID, out option);
            return option;
        }

        public IPassword GetPassword(string ID)
        {
            IPassword option;
            this._passwords.TryGetValue(ID, out option);
            return option;
        }

        public void AddUsernameSource(IUsername newusersource)
        {
            IAuthenticator auth;
            if (this._authenticators.TryGetValue(newusersource.AuthID, out auth) == true)
            {
                auth.UsernameSource = newusersource;
            }
            else
            { this._usernames.Add(newusersource.AuthID, newusersource); }
        }

        public void AddPasswordSource(IPassword newpwsource)
        {
            IAuthenticator auth;
            if (this._authenticators.TryGetValue(newpwsource.AuthID, out auth) == true)
            {
                auth.PasswordSource = newpwsource;
            }
            else
            { this._passwords.Add(newpwsource.AuthID, newpwsource); }
        }

        public void AddAuthenticator(IAuthenticator newauth)
        {
            IUsername user;
            if (this._usernames.TryGetValue(newauth.AuthID, out user) == true)
            {
                newauth.UsernameSource = user;
                this._usernames.Remove(newauth.AuthID);
            }

            IPassword pass;
            if (this._passwords.TryGetValue(newauth.AuthID, out pass) == true)
            {
                newauth.PasswordSource = pass;
                this._passwords.Remove(newauth.AuthID);
            }

            this._authenticators.Add(newauth.AuthID, newauth);
        }
    }
}
