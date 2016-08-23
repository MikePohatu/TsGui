using System;
using System.Linq;
using System.Collections.Generic;

namespace TsGui.Math
{
    public class Calculator
    {
        

        public static double CalculateString(string Input)
        {
            if (string.IsNullOrEmpty(Input)) { return 0; }

            List<Operator> asOperators = new List<Operator>();
            List<Operator> dmOperators = new List<Operator>();
            List<Operator> expOperators = new List<Operator>();

            Operator currOperator = null;
            string s;
            bool subequation = false;
            string substring = "";
            string valstring = "";
            int openbracecount = 0;
            int closebracecount = 0;

            for (int i = 0; i < Input.Length; i++)
            {
                s = Input.Substring(i, 1);
                if (!(s==" "))
                {
                    if (s == "(")
                    {
                        subequation = true;
                        openbracecount++;
                    }

                    else if (s == ")")
                    {
                        closebracecount++;
                        if (openbracecount == closebracecount)
                        {
                            //we have to convert the result of the substring back to a string.
                            //the remaining function is expecting a string to work with. 
                            valstring = CalculateString(substring).ToString();

                            //reset everything
                            subequation = false;
                            substring = "";
                            openbracecount = 0;
                            closebracecount = 0;
                        }
                    }
                    else if (subequation == true)
                    {
                        substring = substring + s;
                    }

                    
                    else if (new string[] {"+","-","/","*","^"}.Contains(s))
                    {
                        //if this is an operator, new one need to be created and added to the appropriate
                        //list
                        #region
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
                    else { valstring = valstring + s; }
                }
            }

            //now capture the last operand
            Operand lastop = new Operand();
            lastop.Value = Double.Parse(valstring);
            lastop.Prev = currOperator;
            currOperator.B = lastop;

            double result = 0;
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