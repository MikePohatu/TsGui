using NUnit.Framework;
using System.Xml.Linq;
using System.Diagnostics;
using System;

using TsGui.Validation;

namespace TsGui.Tests
{
    [TestFixture]
    public class ResultValidatorTests
    {
        [Test]
        [TestCase("MINIT", ExpectedResult = true)]
        [TestCase("MINIT_Test",ExpectedResult = true)]
        [TestCase("MINI", ExpectedResult = false)]       
        [TestCase(null, ExpectedResult = true)]
        public bool ShouldIgnoreTest_Def(string TestString)
        {
            XElement x = new XElement("TestValue");
            x.Add(new XElement("Ignore", "MINIT"));
            x.Add(new XElement("Ignore", "MINWIN"));

            bool result = ResultValidator.ShouldIgnore(x, TestString);
            // TODO: Add your test code here
            return result;
        }

        [Test]
        [TestCase("MINIT_Test", "EndsWith", ExpectedResult = false)]
        [TestCase("MINI", "EndsWith", ExpectedResult = false)]
        [TestCase("Test_MINIT", "EndsWith", ExpectedResult = true)]
        [TestCase("MINIT", "EndsWith", ExpectedResult = true)]
        [TestCase("MINIT_Test", "Contains", ExpectedResult = true)]
        [TestCase("MINI", "Contains", ExpectedResult = false)]
        [TestCase("Test_MINIT", "Contains", ExpectedResult = true)]
        [TestCase("MINIT", "Contains", ExpectedResult = true)]
        [TestCase("MINIT", "Equals", ExpectedResult = true)]
        [TestCase("MINIT ", "Equals", ExpectedResult = false)]
        [TestCase(null, "Equals", ExpectedResult = true)]
        public bool ShouldIgnoreTest_SearchType(string TestString, string SearchType)
        {
            XElement x = new XElement("TestValue");
            XElement xignore = new XElement("Ignore", "MINIT");

            xignore.Add(new XAttribute("SearchType", SearchType));
            x.Add(xignore);

            bool result = ResultValidator.ShouldIgnore(x, TestString);
            // TODO: Add your test code here
            return result;
        }

        [Test]
        [TestCase("MINIT_Test",3, ExpectedResult = "MIN")]
        [TestCase("ThisIsATestString",7, ExpectedResult = "ThisIsA")]
        [TestCase(null, 7, ExpectedResult = null)]
        [TestCase("Testing", 45, ExpectedResult = "Testing")]
        public string TruncateTest(string StringValue, int Length)
        {
            return ResultValidator.Truncate(StringValue, Length);
        }

        [Test]
        [TestCase("MINIT_Test","_", true,ExpectedResult = true)]
        [TestCase("MINIT_Test", "m", true, ExpectedResult = false)]
        [TestCase(null, "m", true, ExpectedResult = false)]
        [TestCase("MINIT_Test", "", true, ExpectedResult = true)]
        [TestCase("MINIT_Test", "M", true, ExpectedResult = true)]
        public bool DoesStringContainCharactersTest(string StringValue, string InvalidChars, bool CaseSensitive)
        {
            return ResultValidator.DoesStringContainCharacters(StringValue, InvalidChars, CaseSensitive);
            
        }

        [Test]
        [TestCase("MINIT_Test", "_", ExpectedResult = "MINITTest")]
        [TestCase("MINIT_Test", "m", ExpectedResult = "MINIT_Test")]
        [TestCase(null, "m", ExpectedResult = null)]
        [TestCase("MINIT_Test", "", ExpectedResult = "MINIT_Test")]
        [TestCase("MINIT_Test", "M", ExpectedResult = "INIT_Test")]
        public string RemoveInvalidTest(string StringValue, string InvalidChars)
        {
            return ResultValidator.RemoveInvalid(StringValue, InvalidChars);

        }

        [Test]
        [TestCase("MINIT_Test", 4, ExpectedResult = false)]
        [TestCase("MINIT_Test", 12, ExpectedResult = true)]
        [TestCase("MINIT_Test", 10, ExpectedResult = true)]
        [TestCase("MINIT_Test", 0, ExpectedResult = true)]
        [TestCase(null, 10, ExpectedResult = false)]
        public bool ValidMaxLengthTest(string StringValue, int MaxLength)
        {
            return ResultValidator.ValidMaxLength(StringValue, MaxLength);
        }

        [Test]
        [TestCase("MINIT_Test", 4, ExpectedResult = true)]
        [TestCase("MINIT_Test", 12, ExpectedResult = false)]
        [TestCase("MINIT_Test", 10, ExpectedResult = true)]
        [TestCase("MINIT_Test", 0, ExpectedResult = true)]
        [TestCase(null, 10, ExpectedResult = false)]
        public bool ValidMinLengthTest(string StringValue, int MinLength)
        {
            return ResultValidator.ValidMinLength(StringValue, MinLength);
        }

        [Test]
        [TestCase("my-us3r_n4m3", "^[a-z0-9_-]{3,16}$", false, ExpectedResult = true)]
        [TestCase("th1s1s-wayt00_l0ngt0beausername", "^[a-z0-9_-]{3,16}$", false, ExpectedResult = false)]
        [TestCase("myp4ssw0rd", "^[a-z0-9_-]{6,18}$", false, ExpectedResult = true)]
        [TestCase("mypa$$w0rd", "^[a-z0-9_-]{6,18}$", false, ExpectedResult = false)]
        public bool DoesRegExMatchTest(string StringValue, string Pattern, bool CaseSensitive)
        {
            return ResultValidator.DoesRegexMatch(StringValue, Pattern, CaseSensitive);
        }

        [Test]
        [TestCase("1", "4", StringMatchingRuleType.LessThan, ExpectedResult = true)]
        [TestCase("4", "4", StringMatchingRuleType.LessThan, ExpectedResult = false)]
        [TestCase("6", "4", StringMatchingRuleType.LessThan, ExpectedResult = false)]
        [TestCase("1", "-4", StringMatchingRuleType.LessThan, ExpectedResult = false)]
        [TestCase("-10", "-4", StringMatchingRuleType.LessThan, ExpectedResult = true)]
        [TestCase("1", "4",StringMatchingRuleType.GreaterThan, ExpectedResult = false)]
        [TestCase("4", "4", StringMatchingRuleType.GreaterThan, ExpectedResult = false)]
        [TestCase("6", "4", StringMatchingRuleType.GreaterThan, ExpectedResult = true)]
        [TestCase("1", "-4", StringMatchingRuleType.GreaterThan, ExpectedResult = true)]
        [TestCase("-10", "-4", StringMatchingRuleType.GreaterThan, ExpectedResult = false)]
        [TestCase("lte", "-4", StringMatchingRuleType.GreaterThan, ExpectedResult = false)]
        [TestCase("-10", "lte", StringMatchingRuleType.GreaterThan, ExpectedResult = false)]
        public bool DoesNumberComparisonMatchTest(string StringInput, string StringRuleContent, StringMatchingRuleType Type)
        {
            return ResultValidator.DoesNumberComparisonMatch(StringInput, StringRuleContent,Type);
        }


        [Test]
        [TestCase("1", ExpectedResult = true)]
        [TestCase("fa", ExpectedResult = false)]
        [TestCase("-", ExpectedResult = false)]
        public bool IsNumericTest(string StringInput)
        {
            return ResultValidator.IsNumeric(StringInput);
        }
    }
}
