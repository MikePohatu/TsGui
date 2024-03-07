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
using TsGui.Authentication;
using TsGui.Authentication.ActiveDirectory;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Security;
namespace TsGui.Tests.Authentication
{
    [TestFixture]
    public class ActiveDirectoryAuthenticatorTests
    {
        

        //Authenticate tests
        #region
        [Test]
        [TestCaseSource("ActiveDirectoryAuthentication_Authenticate_TestCases")]
        public void AuthenticateMembershipsTest(ActiveDirectoryAuthenticatorTestArgs args)
        {
            XElement x = new XElement("Authentication");
            x.Add(new XAttribute("AuthID", "testid"));
            x.Add(new XAttribute("Domain", args.Domain));
            ActiveDirectoryAuthenticator adauth = new ActiveDirectoryAuthenticator(x);
            ActiveDirectoryAuthenticatorTestSource source = new ActiveDirectoryAuthenticatorTestSource();
            adauth.PasswordSource = source;
            adauth.UsernameSource = source;
            source.Username = args.AuthUser;
            source.SecureString = GetSecureStringFromString(args.AuthPassword);
            adauth.AddGroups(args.Groups);
            var result = adauth.AuthenticateAsync().Result;


            if (args.ExpectedMemberships != null && result.Memberships != null)
            {
                foreach (var membership in args.ExpectedMemberships.Keys)
                {
                    bool returnedval = false;
                    if (result.Memberships.TryGetValue(membership, out returnedval))
                    {
                        if (returnedval != args.ExpectedMemberships[membership])
                        {
                            NUnit.Framework.Assert.That(false, "Group membership doesn't match expected: " + membership);
                        }
                    }
                }

                NUnit.Framework.Assert.That(true, "Group memberships match");
            }
            else if (args.ExpectedMemberships == null && result.Memberships == null || args.ExpectedMemberships == null && result.Memberships != null && result.Memberships.Count == 0)
            {
                NUnit.Framework.Assert.That(true, "No group memberships expected");
            }
            else if (args.ExpectedMemberships == null && result.Memberships != null && result.Memberships.Count > 0)
            {
                NUnit.Framework.Assert.That(false, "No memberships expected");
            }
            else
            {
                NUnit.Framework.Assert.That(false, "Null memberships returned");
            }
        }

        [Test]
        [TestCaseSource("ActiveDirectoryAuthentication_Authenticate_TestCases")]
        public void AuthenticateStateTest(ActiveDirectoryAuthenticatorTestArgs args)
        {
            ActiveDirectoryAuthenticatorTestSource source = new ActiveDirectoryAuthenticatorTestSource();

            XElement x = new XElement("Authentication");
            x.Add(new XAttribute("AuthID", "testid"));
            x.Add(new XAttribute("Domain", args.Domain));
            ActiveDirectoryAuthenticator adauth = new ActiveDirectoryAuthenticator(x);
            adauth.PasswordSource = source;
            adauth.UsernameSource = source;
            source.Username = args.AuthUser;
            source.SecureString = GetSecureStringFromString(args.AuthPassword);
            adauth.AddGroups(args.Groups);
            var result = adauth.AuthenticateAsync().Result;
            NUnit.Framework.Assert.AreEqual(args.ExpectedState, result.State);
        }

        public static IEnumerable<TestCaseData> ActiveDirectoryAuthentication_Authenticate_TestCases
        {
            get
            {
                yield return new TestCaseData(new ActiveDirectoryAuthenticatorTestArgs(null, AuthState.Authorised, null));
                yield return new TestCaseData(new ActiveDirectoryAuthenticatorTestArgs("badpassword", null, AuthState.AccessDenied, null));
                yield return new TestCaseData(new ActiveDirectoryAuthenticatorTestArgs(new List<string> { "SCCM Admins" }, AuthState.NotAuthorised, new Dictionary<string, bool>
                {
                    {"SCCM Admins", false }
                }));
                yield return new TestCaseData(new ActiveDirectoryAuthenticatorTestArgs(new List<string> { "Domain Users" }, AuthState.Authorised, new Dictionary<string, bool>
                {
                    {"Domain Users", true }
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
