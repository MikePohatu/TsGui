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
using System.Xml.Linq;
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

        //get a key value pair from WMI. first value returned is the key, the remaining values are concatenated
        public static Dictionary<string,string> GetWmiPair(XElement InputXml)
        {
            string s1;
            string s2;
            Dictionary<string, string> options = new Dictionary<string, string>();
            int i = 0;
            XElement x;
            string keyproperty = null;
            string wmiclass = null;
            string WmiQuery = null;
            //string properties = null;
            IEnumerable<XElement> properties;
            string propertiesString = null;

            //first read and process the XML
            x = (InputXml.Element("KeyProperty"));
            if (x!=null)
            {
                keyproperty = x.Value;
            }
            x = (InputXml.Element("Class"));
            if (x != null)
            {
                wmiclass = x.Value;
            }
            properties = InputXml.Elements("Property");
            if (properties != null)
            {
                foreach (string s in properties)
                {
                    propertiesString = propertiesString + "," + s;
                }
            }

            if ((string.IsNullOrEmpty(wmiclass)) || (string.IsNullOrEmpty(propertiesString)) || (string.IsNullOrEmpty(keyproperty)))
            { throw new InvalidOperationException("Missing WMI class or properties"); }

            WmiQuery = "select " + keyproperty + "," + propertiesString + " FROM " + wmiclass;

            //now process
            try
            {
                WqlObjectQuery wqlQuery = new WqlObjectQuery(WmiQuery);
                //wqlQuery.
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wqlQuery);
                //ManagementObjectCollection searchercollection = searcher.Get();
                
                foreach (ManagementObject m in searcher.Get())
                {
                    Debug.WriteLine(m.ToString());
                    s1 = null;
                    s2 = null;
                    foreach (PropertyData propdata in m.Properties)
                    {
                        if (i == 0) { s1 = propdata.Value.ToString(); }
                        else if (i == 1) { s2 = propdata.Value.ToString(); }
                        else { s2 = s2 + ", " + propdata.Value.ToString(); }
                        i++;
                    }
                    options.Add(s1, s2);
                }

                return options;
            }
            catch
            {
                throw new InvalidOperationException("Error running WMI query: " + WmiQuery + Environment.NewLine);
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
