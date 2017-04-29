﻿//    Copyright (C) 2017 Mike Pohatu

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


using NUnit.Framework;
using TsGui.Authentication;
using System.Collections.Generic;
using System.Security;
namespace TsGui.Tests.Authentication
{
    [TestFixture]
    public class ActiveDirectoryAuthenticationTests
    {
        

        //Authenticate tests
        #region
        [Test]
        [TestCaseSource("ActiveDirectoryAuthentication_Authenticate_TestCases")]
        public void AuthenticateTest(ActiveDirectoryAuthenticationTestArgs args)
        {
            SecureString secpw = GetSecureStringFromString(args.AuthPassword);

            ActiveDirectoryAuthentication adauth = new ActiveDirectoryAuthentication(args.AuthUser, secpw, args.Domain,args.Groups);
            AuthState state = adauth.Authenticate();
            NUnit.Framework.Assert.AreEqual(args.ExpectedResult, state);
        }

        public static IEnumerable<TestCaseData> ActiveDirectoryAuthentication_Authenticate_TestCases
        {
            get
            {
                yield return new TestCaseData(new ActiveDirectoryAuthenticationTestArgs(null, null, null, null, AuthState.Authorised));
                yield return new TestCaseData(new ActiveDirectoryAuthenticationTestArgs(null, "asdf", null, null, AuthState.AccessDenied));
                yield return new TestCaseData(new ActiveDirectoryAuthenticationTestArgs(null, null, null, new List<string> { "SCCM Admins" }, AuthState.NotAuthorised));
                yield return new TestCaseData(new ActiveDirectoryAuthenticationTestArgs(null, null, null, new List<string> { "Domain Users" }, AuthState.Authorised));
            }
        }
        #endregion


        private SecureString GetSecureStringFromString(string s)
        {
            SecureString secure = new SecureString();
            foreach (char c in s)
            {
                secure.AppendChar(c);
            }
            return secure;
        }
    }
}
