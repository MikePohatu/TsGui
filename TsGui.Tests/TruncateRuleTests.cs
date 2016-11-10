using NUnit.Framework;
using TsGui.Queries;
using System.Xml.Linq;

namespace TsGui.Tests
{
    [TestFixture]
    public class TruncateRuleTests
    {
        [Test]
        [TestCase("2016","2", ExpectedResult = "16")]
        [TestCase("asdfliiiKKasdf", "5", ExpectedResult = "Kasdf")]
        [TestCase("asdf", "5", ExpectedResult = "asdf")]
        public string KeepFromEndTest(string Input, string Number)
        {
            XElement x = new XElement("Truncate");
            x.Add(new XAttribute("Type", "KeepFromEnd"));
            x.Value = Number;

            TruncateRule rule = new TruncateRule(x);
            return rule.Process(Input);
        }

        [Test]
        [TestCase("2016", "2", ExpectedResult = "20")]
        [TestCase("asdfliiiKKasdf", "5", ExpectedResult = "asdfl")]
        [TestCase("asdf", "5", ExpectedResult = "asdf")]
        public string KeepFromStartTest(string Input, string Number)
        {
            XElement x = new XElement("Truncate");
            x.Add(new XAttribute("Type", "KeepFromStart"));
            x.Value = Number;

            TruncateRule rule = new TruncateRule(x);
            return rule.Process(Input);
        }

        [Test]
        [TestCase("2016", "2", ExpectedResult = "16")]
        [TestCase("asdfliiiKKasdf", "5", ExpectedResult = "iiiKKasdf")]
        [TestCase("asdf", "5", ExpectedResult = "")]
        public string RemoveFromStartTest(string Input, string Number)
        {
            XElement x = new XElement("Truncate");
            x.Add(new XAttribute("Type", "RemoveFromStart"));
            x.Value = Number;

            TruncateRule rule = new TruncateRule(x);
            return rule.Process(Input);
        }

        [Test]
        [TestCase("2016", "2", ExpectedResult = "20")]
        [TestCase("asdfliiiKKasdf", "5", ExpectedResult = "asdfliiiK")]
        [TestCase("asdf", "5", ExpectedResult = "")]
        public string RemoveFromEndTest(string Input, string Number)
        {
            XElement x = new XElement("Truncate");
            x.Add(new XAttribute("Type", "RemoveFromEnd"));
            x.Value = Number;

            TruncateRule rule = new TruncateRule(x);
            return rule.Process(Input);
        }
    }
}
