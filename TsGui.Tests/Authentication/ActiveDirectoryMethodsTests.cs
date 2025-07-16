#region license
// Copyright (c) 2025 Mike Pohatu
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
using System.Linq;

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
            adauth.AddGroups(args.AuthArgs.Groups);
            var dummy = adauth.AuthenticateAsync().Result;
            var results = ActiveDirectoryMethods.IsUserMemberOfGroups(adauth.Context, args.UserName, args.ExpectedResults.Keys.ToList());
            Assert.AreEqual(args.ExpectedResults, results);
        }

        public static IEnumerable<TestCaseData> ActiveDirectoryAuthentication_IsMemberOfGroupsTest_TestCases
        {
            get
            {
                ActiveDirectoryAuthenticatorTestArgs authargs = new ActiveDirectoryAuthenticatorTestArgs(null, AuthState.Authorised, new Dictionary<string, bool>());

                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs,"mikep", new Dictionary<string, bool>
                {
                    { "Domain Admins", false }
                })) ;
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "mikep", new Dictionary<string, bool>
                {
                    { "Enterprise Admins", false }
                }));
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "mikep", new Dictionary<string, bool>
                {
                    { "Domain Users", true }
                }));
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "mikep", new Dictionary<string, bool>
                {
                    { "SCCM Admins", true }
                }));
                yield return new TestCaseData(new ActiveDirectoryMethodsTestArgs(authargs, "administrator", new Dictionary<string, bool>
                {
                    { "SCCM Admins", false }
                }));
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
