using NUnit.Framework;
using System.Xml.Linq;
using System.Diagnostics;

namespace TsGui.Tests
{
    [TestFixture]
    public class CheckerTests
    {
        [Test]
        [TestCase("MINIT_Test",ExpectedResult = true)]
        [TestCase("MINI", ExpectedResult = false)]
        [TestCase("MINIT",ExpectedResult = true)]
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
        public string Truncate(string StringValue, int Length)
        {
            string result = Checker.Truncate(StringValue, Length);
            return result;
        }
    }
}
