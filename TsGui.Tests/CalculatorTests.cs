#region license
// Copyright (c) 2020 Mike Pohatu
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
using TsGui.Math;

namespace TsGui.Tests
{
    [TestFixture]
    public class CalculatorTests
    {
        [Test]        
        [TestCase("7/2", ExpectedResult = 3.5)]
        [TestCase("8/2", ExpectedResult = 4)]
        [TestCase("2+4", ExpectedResult = 6)]
        [TestCase("2+4", ExpectedResult = 6)]
        [TestCase("2^3", ExpectedResult = 8)]
        [TestCase("2+4*7", ExpectedResult = 30)]
        [TestCase("2+4*7^2", ExpectedResult = 198)]
        [TestCase("2*4*8*2", ExpectedResult = 128)]
        [TestCase("2+4*8*2", ExpectedResult = 66)]
        [TestCase("2+-4*8*2", ExpectedResult = -62)]
        [TestCase("(2+4)*(8/2)", ExpectedResult = 24)]
        [TestCase("( 2.5 + 14 ) * ( 118 / 2 )", ExpectedResult = 973.5)]
        [TestCase("( 2.5 + 14.7 ) * ( -115 ^ 2 )", ExpectedResult = 227470)]
        [TestCase("( 2.5 + 14.7 ) * -( 115.3 ^ 2 )", ExpectedResult = -228658.348)]
        [TestCase("((2+4)*(8/2))", ExpectedResult = 24)]
        [TestCase("((2+4)*7)/2", ExpectedResult = 21)]
        [TestCase("((2+4)*7)/-2", ExpectedResult = -21)]
        [TestCase("((2+4)*7)^2/14", ExpectedResult = 126)]
        [TestCase(null, ExpectedResult = 0)]
        public double CalculateStringTest(string Variable)
        {
            return Calculator.CalculateString(Variable);
        }
    }
}
