using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using TsGui.Authentication.LocalConfig;

namespace TsGui.Tests.Authentication.LocalConfig
{
    public class PasswordTests
    {
        [Test]
        [TestCase("Password1", ExpectedResult = true)]
        public bool AuthenticateBasicHashingTest(string TestPW)
        {
            string key = Password.CreateKey();
            string hash = Password.HashPassword(TestPW, key);

            Password p = new Password(hash, key);
            return p.PasswordMatches(TestPW);
        }
    }
}
