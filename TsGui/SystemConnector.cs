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
        public static string GetEnvVar(XElement InputXml)
        {
            #region
            string s;

            foreach (XElement x in InputXml.Elements("EnvironmentVariable"))
            {
                if (x != null)
                {
                    //try ts env 
                    //try process variables
                    s = Environment.GetEnvironmentVariable(x.Value, EnvironmentVariableTarget.Process);
                    if (s != null) { return s; }

                    //try computer variables
                    s = Environment.GetEnvironmentVariable(x.Value, EnvironmentVariableTarget.Machine);
                    if (s != null) { return s; }

                    //try user variables
                    s = Environment.GetEnvironmentVariable(x.Value, EnvironmentVariableTarget.User);
                    if (s != null) { return s; }
                }
            }

            XText xt = InputXml.Nodes().OfType<XText>().FirstOrDefault();
            s = xt.Value.Trim();

            if (s != null) { return s; }
            else { return null; }

            #endregion
        }
    }
}
