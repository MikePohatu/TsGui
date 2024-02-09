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
using NUnit.Framework;
using System.Collections.Generic;
using System.Security;
using System.Xml.Linq;
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
            XElement x = new XElement("Authentication");
            x.Add(new XAttribute("AuthID", "testid"));
            x.Add(new XAttribute("Domain", authargs.Domain));
            ActiveDirectoryAuthenticator adauth = new ActiveDirectoryAuthenticator(x);
            ActiveDirectoryAuthenticatorTestSource source = new ActiveDirectoryAuthenticatorTestSource();
            adauth.PasswordSource = source;
            adauth.UsernameSource = source;
            source.Username = authargs.AuthUser;
            source.SecureString = GetSecureStringFromString(authargs.AuthPassword);
            adauth.Groups = args.AuthArgs.Groups;
            adauth.Authenticate();
            bool result = ActiveDirectoryMethods.IsUserMemberOfAllGroups(adauth.Context, args.UserName, args.Groups);
            Assert.AreEqual(args.ExpectedResult,result);
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
