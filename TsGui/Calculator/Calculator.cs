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

using System;
using System.Linq;
using System.Collections.Generic;
//using System.Diagnostics;

namespace TsGui.Math
{
    public class Calculator
    {
        

        public static double CalculateString(string Input)
        {
            //Debug.WriteLine("Calculate string: " + Input);
            if (string.IsNullOrEmpty(Input)) { return 0; }

            List<Operator> asOperators = new List<Operator>();
            List<Operator> dmOperators = new List<Operator>();
            List<Operator> expOperators = new List<Operator>();

            Operator currOperator = null;
            string s;
            bool subequation = false;
            string substring = "";
            string valstring = "";
            int openParenCount = 0;

            for (int i = 0; i < Input.Length; i++)
            {
                s = Input.Substring(i, 1);
                if (!(s==" "))
                {
                    if (s == "(")
                    {
                        openParenCount++;
                        if (subequation == true) { substring = substring + s; }
                        else { subequation = true; }  
                    }

                    else if (s == ")")
                    {
                        openParenCount--;
                        if (openParenCount == 0)
                        {
                            //we have to convert the result of the substring back to a string.
                            //the remaining function is expecting a string to work with. keep the
                            //existing valstring in case there is a sign there
                            valstring = valstring + (CalculateString(substring).ToString());

                            //reset everything
                            subequation = false;
                            substring = "";
                        }
                        else { substring = substring + s; }
                    }
                    else if (subequation == true)
                    {
                        substring = substring + s;
                    }
                  
                    else if (new string[] {"+","-","/","*","^"}.Contains(s))
                    {
                        //if this is an operator, new one need to be created and added to the appropriate
                        //list
                        
                        if (string.IsNullOrEmpty(valstring))
                        { valstring = valstring + s; }
                        else
                        #region
                        {
                            Operator newoperator = new Operator();
                            if (s == "+")
                            {
                                newoperator.Type = OperatorType.Add;
                                asOperators.Add(newoperator);
                            }
                            else if (s == "-")
                            {
                                newoperator.Type = OperatorType.Subtract;
                                asOperators.Add(newoperator);
                            }
                            else if (s == "/")
                            {
                                newoperator.Type = OperatorType.Divide;
                                dmOperators.Add(newoperator);
                            }
                            else if (s == "*")
                            {
                                newoperator.Type = OperatorType.Multiply;
                                dmOperators.Add(newoperator);
                            }
                            else if (s == "^")
                            {
                                newoperator.Type = OperatorType.Exponent;
                                expOperators.Add(newoperator);
                            }
                            #endregion

                            //now create the new operand and setup the mappings
                            Operand o = new Operand();
                            o.Value = Double.Parse(valstring);
                            o.Prev = currOperator;
                            if (currOperator != null) { currOperator.B = o; }
                            o.Next = newoperator;
                            o.Next.A = o;
                            currOperator = o.Next;
                            valstring = "";
                        }
                    }
                    else { valstring = valstring + s; }
                }
            }

            //now capture the last operand
            Operand lastop = new Operand();
            lastop.Value = Double.Parse(valstring);
            lastop.Prev = currOperator;
            if (currOperator != null) { currOperator.B = lastop; }

            double result = lastop.Value;
            foreach (Operator o in expOperators) { result = ProcessOperator(o); }
            foreach (Operator o in dmOperators) { result = ProcessOperator(o); }
            foreach (Operator o in asOperators) { result = ProcessOperator(o); }

            return result;
        }

        private static double ProcessOperator(Operator O)
        {
            Operand newval = new Operand();
            newval.Value = O.Result;

            if (O.A.Prev != null) { O.A.Prev.B = newval; }
            if (O.B.Next != null) { O.B.Next.A = newval; }
            newval.Prev = O.A.Prev;
            newval.Next = O.B.Next;
                  
            return O.Result;
        }
    }
}