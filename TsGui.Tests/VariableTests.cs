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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsGui.Tests
{
    [TestFixture]
    public class VariableTests
    {
        [Test]
        [TestCase("2016", ExpectedResult = false)]
        [TestCase("asdfliiiKKasdf", ExpectedResult = true)]
        [TestCase("GLKJliiiKKasdf", ExpectedResult = true)]
        [TestCase(" a sdf", ExpectedResult = false)]
        [TestCase(" a sdf", ExpectedResult = false)]
        [TestCase("a*sdf", ExpectedResult = false)]
        [TestCase("_asdf", ExpectedResult = false)]
        [TestCase("0asdf", ExpectedResult = false)]
        public bool ConfirmValidNameTest(string variableName)
        {
            string validated = null;
            return Variable.ConfirmValidName(variableName, out validated);
        }

        [Test]
        [TestCase("2016", ExpectedResult = "")]
        [TestCase("asdfliiiKKasdf", ExpectedResult = "asdfliiiKKasdf")]
        [TestCase("GLKJliiiKKasdf", ExpectedResult = "GLKJliiiKKasdf")]
        [TestCase(" a sdf", ExpectedResult = "asdf")]
        [TestCase("-a sdf", ExpectedResult = "asdf")]
        [TestCase("a*sdf", ExpectedResult = "asdf")]
        [TestCase("_asdf", ExpectedResult = "asdf")]
        [TestCase("0asdf", ExpectedResult = "asdf")]
        public string ConfirmValidNameStripped(string variableName)
        {
            string validated = null;
            Variable.ConfirmValidName(variableName, out validated);
            return validated;
        }
    }
}
