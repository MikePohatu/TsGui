#region license
// Copyright (c) 2021 20Road Limited
//
// This file is part of DevChecker.
//
// DevChecker is free software: you can redistribute it and/or modify
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
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Credential: ViewModelBase
    {
        public bool CredentialsSet { get { return !(string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(this.Password)); } }

        private string _username;
        public string Username
        {
            get { return this._username; }
            set { this._username = value; this.OnPropertyChanged(this, "Username"); }
        }

        public string Password { get; private set; }
        public SecureString SecurePassword { get; private set; }

        private string _domain;
        public string Domain
        {
            get { return this._domain; }
            set { this._domain = value; this.OnPropertyChanged(this, "Domain"); }
        }

        private bool _useKerberos = true;
        public bool UseKerberos
        {
            get { return this._useKerberos; }
            set { this._useKerberos = value; this.OnPropertyChanged(this, "UseKerberos"); }
        }

        public void UpdatePassword(SecureString securePassword, string password)
        {
            this.SecurePassword = securePassword;
            this.Password = password;
        }

        public Credential Clone()
        {
            Credential newcred = new Credential();
            newcred.Username = this.Username;
            newcred.Domain = this.Domain;
            newcred.UseKerberos = this.UseKerberos;
            return newcred;
        }

        public void Update(Credential newcreds)
        {            
            this.Username = newcreds.Username;
            this.Password = newcreds.Password;
            this.Domain = newcreds.Domain;
            this.UseKerberos = newcreds.UseKerberos;
            this.SecurePassword = newcreds.SecurePassword; 
            Log.Info("Updated credentials with username: " + this.Username + " and domain: " + this.Domain);
        }
    }
}
