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
using TsGui.Validation;

namespace TsGui.Tests
{
    [TestFixture]
    public class ResultValidatorTests
    {
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
    }
}
