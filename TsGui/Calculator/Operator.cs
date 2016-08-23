//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System.Diagnostics;

namespace TsGui.Math
{
    public class Operator
    {
        public OperatorType Type { get; set; }
        public Operand A { get; set; }
        public Operand B { get; set; }
        public double Result { get { return this.Calc(); } }

        private double Calc()
        {
            if ((A == null) || (B == null)) { Debug.WriteLine("null A or B"); }
            if (this.Type == OperatorType.Add) { return (A.Value + B.Value); }
            else if (this.Type == OperatorType.Divide) { return (A.Value / B.Value); }
            else if (this.Type == OperatorType.Multiply) { return (A.Value * B.Value); }
            else if (this.Type == OperatorType.Subtract) { return (A.Value - B.Value); }
            else if (this.Type == OperatorType.Exponent) { return System.Math.Pow(A.Value, B.Value); }
            else { return 0; }
        }
    }
}
