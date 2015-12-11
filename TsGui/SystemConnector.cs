using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace TsGui
{
    internal class SystemConnector
    {
        //check an xml element. check for environmental variable elements
        //return the first one that brings back a value. otherwise, return 
        //the value of the root xml
        public static string GetVariableValue(string Variable)
        {
            string s;

            //try ts env 
            //try process variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.Process);
            if (s != null) { return s; }

            //try computer variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.Machine);
            if (s != null) { return s; }

            //try user variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.User);
            if (s != null) { return s; }

            //not found. return null
            return null;
             
        }
    }
}
