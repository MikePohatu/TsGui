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
using System.Xml.Linq;
using TsGui.Tests.Linking;
using TsGui.Validation.StringMatching;


namespace TsGui.Tests.Validation
{
    [TestFixture]
    class MatchingRuleTests
    {
        [Test]
        [TestCase("MINIT_Test", "MINIT", "EndsWith", false, ExpectedResult = false)]
        [TestCase("MINI", "MINIT", "EndsWith", false, ExpectedResult = false)]
        [TestCase("Test_MINIT", "MINIT", "EndsWith", false, ExpectedResult = true)]
        [TestCase("MINIT", "MINIT", "EndsWith", false, ExpectedResult = true)]
        [TestCase("MINIT_Test", "MINIT", "Contains", false, ExpectedResult = true)]
        [TestCase("MINI", "MINIT", "Contains", false, ExpectedResult = false)]
        [TestCase("Test_MINIT", "MINIT", "Contains", false, ExpectedResult = true)]
        [TestCase("MINIT", "MINIT", "Contains", false, ExpectedResult = true)]
        [TestCase("MINIT", "MINIT", "Equals", false, ExpectedResult = true)]
        [TestCase("MINIT ", "MINIT", "Equals", false, ExpectedResult = false)]
        [TestCase("my-us3r_n4m3", "^[a-z0-9_-]{3,16}$", "RegEx", false, ExpectedResult = true)]
        [TestCase("th1s1s-wayt00_l0ngt0beausername", "^[a-z0-9_-]{3,16}$", "RegEx", false, ExpectedResult = false)]
        [TestCase("myp4ssw0rd", "^[a-z0-9_-]{6,18}$", "RegEx", false, ExpectedResult = true)]
        [TestCase("mypa$$w0rd", "^[a-z0-9_-]{6,18}$", "RegEx", false, ExpectedResult = false)]
        [TestCase("1", "4", "LessThan",true, ExpectedResult = true)]
        [TestCase("4", "4", "LessThan", true, ExpectedResult = false)]
        [TestCase("6", "4", "LessThan",true, ExpectedResult = false)]
        [TestCase("1", "-4", "LessThan",true, ExpectedResult = false)]
        [TestCase("-10", "-4", "LessThan",true, ExpectedResult = true)]
        [TestCase("1", "4", "GreaterThan",true, ExpectedResult = false)]
        [TestCase("4", "4", "GreaterThan", true, ExpectedResult = false)]
        [TestCase("6", "4", "GreaterThan", true, ExpectedResult = true)]
        [TestCase("1", "-4", "GreaterThan", true, ExpectedResult = true)]
        [TestCase("-10", "-4", "GreaterThan", true, ExpectedResult = false)]
        [TestCase("lte", "-4", "GreaterThan", true, ExpectedResult = false)]
        [TestCase("-10", "lte", "GreaterThan", true, ExpectedResult = false)]
        [TestCase("1", "","IsNumeric",true,ExpectedResult = true)]
        [TestCase("fa", "", "IsNumeric", true, ExpectedResult = false)]
        [TestCase("-", "", "IsNumeric", true, ExpectedResult = false)]
        [TestCase(null, "blah", "Equals",false, ExpectedResult = false)]
        [TestCase(null, "*NULL", "Equals", false, ExpectedResult = true)]
        [TestCase(null, "", "Equals", false, ExpectedResult = false)]
        [TestCase(null, "Test", "StartsWith", false, ExpectedResult = false)]
        [TestCase(null, "Test", "Characters", false, ExpectedResult = false)]
        [TestCase(null, "Test", "EndsWith", false, ExpectedResult = false)]
        [TestCase(null, "Test", "Contains", false, ExpectedResult = false)]
        [TestCase(null, "0", "GreaterThan", false, ExpectedResult = false)]
        [TestCase(null, "0", "LessThan", false, ExpectedResult = false)]
        [TestCase(null, "0", "GreaterThanOrEqualTo", false, ExpectedResult = false)]
        [TestCase(null, "0", "LessThanOrEqualTo", false, ExpectedResult = false)]
        [TestCase(null, "^[a-z0-9_-]{6,18}$", "RegEx", false, ExpectedResult = false)]
        public bool RuleTest(string TestString, string RuleString, string SearchType, bool CaseSensitive)
        {
            XElement xrule = new XElement("Rule", RuleString);

            xrule.Add(new XAttribute("Type", SearchType));
            IStringMatchingRule rule = MatchingRuleFactory.GetRuleObject(xrule, new DummyLinkTarget());

            return rule.DoesMatch(TestString);
        }
    }
}
