using NUnit.Framework;
using TsGui.Connectors;

namespace TsGui.Tests
{
    [TestFixture]
    public class SystemConnectorTests
    {
        [Test]
        [TestCase("ComputerName", ExpectedResult = "WIN10")]
        [TestCase(null, ExpectedResult = null)]
        public string GetVariableValueTest(string Variable)
        {
            return SystemConnector.GetVariableValue(Variable);
        }
    }
}
