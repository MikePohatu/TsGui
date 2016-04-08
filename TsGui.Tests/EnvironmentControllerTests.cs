using NUnit.Framework;
using System.Xml.Linq;
using System;

namespace TsGui.Tests
{
    [TestFixture]
    public class EnvironmentControllerTests
    {

        [Test]
        [TestCase(null, ExpectedResult = null)]
        [TestCase("ComputerName", ExpectedResult = "WIN10")]
        public string GetEnvVarTest(string VariableName)
        {
            EnvironmentController controller = new EnvironmentController();
            return controller.GetEnvVar(VariableName);
        }
    }
}
