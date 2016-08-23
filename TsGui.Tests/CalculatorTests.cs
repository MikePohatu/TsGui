using NUnit.Framework;
using TsGui.Math;

namespace TsGui.Tests
{
    [TestFixture]
    public class CalculatorTests
    {
        [Test]
        [TestCase("2*4", ExpectedResult = 8)]
        [TestCase("7/2", ExpectedResult = 3.5)]
        [TestCase("2+4", ExpectedResult = 6)]
        [TestCase("2^3", ExpectedResult = 8)]
        [TestCase("2+4*7", ExpectedResult = 30)]
        [TestCase("2+4*7^2", ExpectedResult = 198)]
        [TestCase("2*4*8*2", ExpectedResult = 128)]
        [TestCase("2+4*8*2", ExpectedResult = 66)]
        [TestCase("(2+4)*(8/2)", ExpectedResult = 24)]
        [TestCase(null, ExpectedResult = 0)]
        public double CalculateStringTest(string Variable)
        {
            return Calculator.CalculateString(Variable);
        }
    }
}
