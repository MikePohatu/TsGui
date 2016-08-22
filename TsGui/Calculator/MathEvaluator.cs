using System;
using System.Collections.Generic;

namespace TsGui.Cal
{
    public class Calculator
    {
        public static double Arithmetic(String input)
        {
            Stack<String> operations = new Stack<String>();
            Stack<Double> values = new Stack<Double>();
            string s;
            bool subequation = false;
            string substring = "";
            int openbracecount = 0;
            int closebracecount = 0;

            for (int i = 0; i < input.Length; i++)
            {
                s = input.Substring(i, 1);
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
                            values.Push(Arithmetic(substring));
                            subequation = false;
                            substring = "";
                        }
                    }
                }
            }

            int operationcount = operations.Count;
            string currop;
            double currval;
            double result = values.Pop(); //pop first value

            while (operationcount > 0)
            {
                currop = operations.Pop();
                currval = values.Pop();

                if (currop == "+") { result = result + currval; }
                else if (currop == "-") { result = result + currval; }
                else if (currop == "/") { result = result + currval; }
                else if (currop == "*") { result = result + currval; }

                operationcount--;
            }

            return result;
        }
    }
}