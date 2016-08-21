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

// SystemConnector.cs - class to connect to standard Windows components (WMI and 
// environment variables. 

using System;
using System.Collections.Generic;
using System.Management;
using System.Diagnostics;

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

        //get a dictionary of names,values from WMI. 
        public static Dictionary<string,string> GetWmiDictionary(string WmiQuery, string KeyProperty, string Separator, List<string> Properties)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
 
            try
            {
                WqlObjectQuery wqlQuery = new WqlObjectQuery(WmiQuery);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wqlQuery);
                
                foreach (ManagementObject m in searcher.Get())
                {
                    KeyValuePair<string, string> kv = ConcatenatePropertyData(m, KeyProperty, Separator, Properties);
                    values.Add(kv.Key,kv.Value);                 
                }

                return values;
            }
            catch
            {
                return null;
                //throw new InvalidOperationException("Error running WMI query: " + WmiQuery + Environment.NewLine);
            }

        }

        //return a key,value pair from a dictionary. keyproperty does to key, specified properties are concatenated together with the separator
        private static KeyValuePair<string, string> ConcatenatePropertyData(ManagementObject Input, string KeyProperty, string Separator, List<string> Properties)
        {
            string s1 = "";
            string s2 = "";
            string tempval = null;
            int i = 0;

            s1 = Input.GetPropertyValue(KeyProperty).ToString();

            foreach (string prop in Properties)
            {
                if (!string.IsNullOrEmpty(prop))
                {
                    tempval = Input.GetPropertyValue(prop).ToString();
                    if (i == 0) { s2 = tempval; }
                    else { s2 = s2 + Separator + tempval; }
                    i++;
                }
            }
            Debug.WriteLine("New KV pair: " + s1 + "," + s2);

            return new KeyValuePair<string, string>(s1, s2);
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
