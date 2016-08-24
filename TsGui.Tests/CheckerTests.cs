using NUnit.Framework;
using System.Xml.Linq;
using System.Diagnostics;

namespace TsGui.Tests
{
    [TestFixture]
    public class CheckerTests
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

            bool result = Checker.ShouldIgnore(x, TestString);
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

            bool result = Checker.ShouldIgnore(x, TestString);
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
            return Checker.Truncate(StringValue, Length);
        }

        [Test]
        [TestCase("MINIT_Test","_", true,ExpectedResult = false)]
        [TestCase("MINIT_Test", "m", true, ExpectedResult = false)]
        [TestCase(null, "m", true, ExpectedResult = false)]
        [TestCase("MINIT_Test", "", true, ExpectedResult = true)]
        [TestCase("MINIT_Test", "M", true, ExpectedResult = false)]
        public bool ValidCharactersTest(string StringValue, string InvalidChars, bool CaseSensitive)
        {
            return Checker.ValidCharacters(StringValue, InvalidChars, CaseSensitive);
            
        }

        [Test]
        [TestCase("MINIT_Test", 4, ExpectedResult = false)]
        [TestCase("MINIT_Test", 12, ExpectedResult = true)]
        [TestCase("MINIT_Test", 10, ExpectedResult = true)]
        [TestCase("MINIT_Test", 0, ExpectedResult = true)]
        [TestCase(null, 10, ExpectedResult = false)]
        public bool ValidMaxLengthTest(string StringValue, int MaxLength)
        {
            return Checker.ValidMaxLength(StringValue, MaxLength);
        }

        [Test]
        [TestCase("MINIT_Test", 4, ExpectedResult = true)]
        [TestCase("MINIT_Test", 12, ExpectedResult = false)]
        [TestCase("MINIT_Test", 10, ExpectedResult = true)]
        [TestCase("MINIT_Test", 0, ExpectedResult = true)]
        [TestCase(null, 10, ExpectedResult = false)]
        public bool ValidMinLengthTest(string StringValue, int MinLength)
        {
            return Checker.ValidMinLength(StringValue, MinLength);
        }
    }
}
