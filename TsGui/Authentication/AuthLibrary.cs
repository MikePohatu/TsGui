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
        private Dictionary<string, AuthenticationBroker> _brokers = new Dictionary<string, AuthenticationBroker>();

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


        public void AddBroker(string AuthID, AuthenticationBroker newbroker)
        {
            bool pendingtasks = false;

            IUsername user;
            if (this._usernames.TryGetValue(AuthID, out user) == true)
            {
                newbroker.UsernameSource = user;
                this._usernames.Remove(AuthID);
            }
            else
            { pendingtasks = true; }

            IPassword pass;
            if (this._passwords.TryGetValue(AuthID, out pass) == true)
            {
                newbroker.PasswordSource = pass;
                this._passwords.Remove(AuthID);
            }
            else
            { pendingtasks = true; }

            if (pendingtasks == true) { this._brokers.Add(AuthID, newbroker); }
        }

        public void AddUsernameSource(string AuthID, IUsername newusersource)
        {
            AuthenticationBroker broker;
            if (this._brokers.TryGetValue(AuthID, out broker) == true)
            {
                broker.UsernameSource = newusersource;
            }
            else
            { this._usernames.Add(AuthID, newusersource); }
        }

        public void AddPasswordSource(string AuthID, IPassword newpwsource)
        {
            AuthenticationBroker broker;
            if (this._brokers.TryGetValue(AuthID, out broker) == true)
            {
                broker.PasswordSource = newpwsource;
            }
            else
            { this._passwords.Add(AuthID, newpwsource); }
        }
    }
}
