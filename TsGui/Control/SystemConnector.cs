using System;
using System.Management;

namespace TsGui
{
    public static class SystemConnector
    {
        //check an xml element. check for environmental variable elements
        //return the first one that brings back a value. otherwise, return 
        //the value of the root xml
        public static string GetVariableValue(string Variable)
        {
            if (Variable == null) { return null; }
            string s;

            //try ts env 
            //try process variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(s)) { return s; }

            //try computer variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.Machine);
            if (!string.IsNullOrEmpty(s)) { return s; }

            //try user variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(s)) { return s; }

            //not found. return null
            return null;
             
        }

        //get a value from WMI
        public static string GetWmiString(string WmiQuery)
        {
            string s = null;
            try
            {
                WqlObjectQuery wqlQuery = new WqlObjectQuery(WmiQuery);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wqlQuery);

                foreach (ManagementObject m in searcher.Get())
                {
                    foreach (PropertyData propdata in m.Properties)
                    {
                        s = s + propdata.Value;
                    }                    
                }

                if (String.IsNullOrEmpty(s)) { return null; }
                else { return s; }
            }
            catch
            {
                //Debug.WriteLine("Exception thrown in SystemConnector: GetWmiQuery");
                return null;
            }
            
        }

        public static ManagementObjectCollection GetWmiManagementObjects(string WmiQuery)
        {
            try
            {
                WqlObjectQuery wqlQuery = new WqlObjectQuery(WmiQuery);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wqlQuery);

                return searcher.Get();
            }
            catch
            {
                //Debug.WriteLine("Exception thrown in SystemConnector: GetWmiQuery");
                return null;
            }
        }
    }
}
