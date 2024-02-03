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
using System.Xml.Linq;
using Core.Logging;

namespace TsGui.Authentication
{
    public static class AuthLibrary
    {
        private static Dictionary<string, IUsername> _usernames = new Dictionary<string, IUsername>();
        private static Dictionary<string, IPassword> _passwords = new Dictionary<string, IPassword>();
        private static Dictionary<string, IPassword> _passwordconfirmers = new Dictionary<string, IPassword>();
        private static Dictionary<string, IAuthenticator> _authenticators = new Dictionary<string, IAuthenticator>();
        private static Dictionary<string, List<IAuthenticatorConsumer>> _pendingconsumers = new Dictionary<string, List<IAuthenticatorConsumer>>();

        public static void LoadXml(XElement InputXml)
        {
            foreach (XElement xauth in InputXml.Elements("Authentication"))
            {
                AddAuthenticator(AuthenticationFactory.GetAuthenticator(xauth));
            }
        }

        public static IUsername GetUsername(string ID)
        {
            IUsername option;
            _usernames.TryGetValue(ID, out option);
            return option;
        }

        public static IPassword GetPassword(string ID)
        {
            IPassword option;
            _passwords.TryGetValue(ID, out option);
            return option;
        }

        public static IAuthenticator GetAuthenticator(string ID)
        {
            IAuthenticator option;
            _authenticators.TryGetValue(ID, out option);
            return option;
        }

        public static void AddAuthenticatorConsumer(IAuthenticatorConsumer consumer)
        {
            IAuthenticator auth;
            if (_authenticators.TryGetValue(consumer.AuthID,out auth) == true)
            { consumer.Authenticator = auth; }
            else
            {
                List<IAuthenticatorConsumer> consumerlist;
                if (_pendingconsumers.TryGetValue(consumer.AuthID,out consumerlist) != true)
                {
                    consumerlist = new List<IAuthenticatorConsumer>();
                    _pendingconsumers.Add(consumer.AuthID, consumerlist);
                }

                consumerlist.Add(consumer);
            }
        }

        public static void AddUsernameSource(IUsername newusersource)
        {
            IAuthenticator auth;
            if (_authenticators.TryGetValue(newusersource.AuthID, out auth) == true)
            {
                auth.UsernameSource = newusersource;
            }
            else
            { _usernames.Add(newusersource.AuthID, newusersource); }
        }

        public static void AddPasswordSource(IPassword newpwsource)
        {
            IAuthenticator auth;
            if (_authenticators.TryGetValue(newpwsource.AuthID, out auth) == true)
            {
                if (auth.PasswordSource == null) { auth.PasswordSource = newpwsource; }
                else { AddPasswordConfirmationSource(newpwsource); }
            }
            else
            {
                IPassword outpw;
                if (_passwords.TryGetValue(newpwsource.AuthID, out outpw))
                {
                    AddPasswordConfirmationSource(newpwsource);
                } 
                else
                {
                    _passwords.Add(newpwsource.AuthID, newpwsource);
                }
            }
                
        }

        private static void AddPasswordConfirmationSource(IPassword newpwsource)
        {
            IAuthenticator auth;
            if (_authenticators.TryGetValue(newpwsource.AuthID, out auth) == true)
            {
                IPasswordConfirmingAuthenticator confirmerauth = auth as IPasswordConfirmingAuthenticator;
                if (confirmerauth != null) { confirmerauth.PasswordConfirmationSource = newpwsource; }
                else { Log.Error($"AuthID {newpwsource.AuthID} is not a password confirmer"); }
            }
            else
            {
                IPassword outpw;
                if (_passwordconfirmers.TryGetValue(newpwsource.AuthID, out outpw))
                {
                    Log.Error($"AuthID {newpwsource.AuthID} already has a confirmation password defined");
                }
                else
                {
                    _passwordconfirmers.Add(newpwsource.AuthID, newpwsource);
                }
            }
        }

        public static void AddAuthenticator(IAuthenticator newauth)
        {
            IUsername user;
            if (_usernames.TryGetValue(newauth.AuthID, out user) == true)
            {
                newauth.UsernameSource = user;
                _usernames.Remove(newauth.AuthID);
            }

            IPassword pass;
            if (_passwords.TryGetValue(newauth.AuthID, out pass) == true)
            {
                newauth.PasswordSource = pass;
                _passwords.Remove(newauth.AuthID);
            }
            if (_passwordconfirmers.TryGetValue(newauth.AuthID, out pass) == true)
            {
                IPasswordConfirmingAuthenticator confirmerauth = newauth as IPasswordConfirmingAuthenticator;
                if (confirmerauth != null)
                {
                    confirmerauth.PasswordConfirmationSource = pass;
                    _passwordconfirmers.Remove(newauth.AuthID);
                }
            }

            List<IAuthenticatorConsumer> consumerlist;
            if (_pendingconsumers.TryGetValue(newauth.AuthID, out consumerlist) == true)
            {
                foreach (IAuthenticatorConsumer consumer in consumerlist)
                {
                    if (consumer.AuthID.Equals(newauth.AuthID))
                    {
                        consumer.Authenticator = newauth;
                    }
                }

                _pendingconsumers.Remove(newauth.AuthID);
            }

            _authenticators.Add(newauth.AuthID, newauth);
        }

        public static void Reset()
        {
            _authenticators.Clear();
            _pendingconsumers.Clear();
            _passwordconfirmers.Clear();
            _passwords.Clear();
            _usernames.Clear();
        }
    }
}
