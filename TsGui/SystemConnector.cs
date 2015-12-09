using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace TsGui
{
    internal class SystemConnector
    {
        //check an xml element. check for environmental variable elements
        //return the first one that brings back a value. otherwise, return 
        //the value of the root xml
        public static string GetEnvVar(XElement InputXml)
        {
            #region
            string s;

            foreach (XElement x in InputXml.Elements("EnvironmentalVariable"))
            {
                if (x != null)
                {
                    s = Environment.GetEnvironmentVariable(x.Value);
                    if (s != null) { return s; }
                }
            }

            return InputXml.Value;
            #endregion
        }
    }
}
