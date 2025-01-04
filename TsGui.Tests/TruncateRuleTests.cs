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
using TsGui.Queries.Rules;
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
