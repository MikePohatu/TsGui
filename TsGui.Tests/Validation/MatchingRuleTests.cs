using NUnit.Framework;
using System.Xml.Linq;

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
        public bool RuleTest(string TestString, string RuleString, string SearchType, bool CaseSensitive)
        {
            XElement xignore = new XElement("Rule", RuleString);

            xignore.Add(new XAttribute("Type", SearchType));
            IStringMatchingRule rule = MatchingRuleFactory.GetRuleObject(xignore);

            return rule.DoesMatch(TestString);
        }
    }
}
