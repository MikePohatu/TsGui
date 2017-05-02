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


using NUnit.Framework;
using System.Collections.Generic;
using System.Security;
using TsGui.Authentication.ActiveDirectory;
using TsGui.Authentication;

namespace TsGui.Tests.Authentication
{
    [TestFixture]
    public class ActiveDirectoryMethodsTests
    {
        //IsMemberOfGroups tests
        #region
        [Test]
        [TestCaseSource("ActiveDirectoryAuthentication_IsMemberOfGroupsTest_TestCases")]
        public void IsMemberOfGroupsTest(ActiveDirectoryMethodsTestArgs args)
        {
            ActiveDirectoryAuthenticatorTestArgs authargs = args.AuthArgs;
            SecureString secpw = GetSecureStringFromString(authargs.AuthPassword);
            ActiveDirectoryAuthenticator adauth = new ActiveDirectoryAuthenticator(authargs.AuthUser, secpw, authargs.Domain, args.Groups);
            adauth.Authenticate();
            bool result = ActiveDirectoryMethods.IsUserMemberOfGroups(adauth.Context, args.UserName, args.Groups);
            NUnit.Framework.Assert.AreEqual(args.ExpectedResult,result);
        }

        public static IEnumerable<TestCaseData> ActiveDirectoryAuthentication_IsMemberOfGroupsTest_TestCases
        {
            get
            {
                ActiveDirectoryAuthenticatorTestArgs authargs = new ActiveDirectoryAuthenticatorTestArgs(null, null, null, null, AuthState.Authorised);
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs,"mikep", new List<string> { "Domain Admins" }, true));
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "mikep", new List<string> { "Enterprise Admins" }, true));
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "mikep", new List<string> { "Domain Users" }, true));
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "mikep", new List<string> { "SCCM Admins" }, true));
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "administrator", new List<string> { "SCCM Admins" }, false));
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
