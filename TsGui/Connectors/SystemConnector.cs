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
using System.Management;
using System.Collections.Generic;
using TsGui.Diagnostics.Logging;

namespace TsGui.Connectors
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
        public static string GetWmiString(string NameSpace, string WmiQuery)
        {
            string s = null;
            try
            { 
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(NameSpace,WmiQuery))
                { 
                    foreach (ManagementObject m in searcher.Get())
                    {
                        foreach (PropertyData propdata in m.Properties)
                        {
                            s = s + propdata.Value;
                        }
                    }
                }

                if (String.IsNullOrEmpty(s)) { return null; }
                else { return s; }
            }
            catch
            {
                LoggerFacade.Error("Error running query against namespace " + NameSpace + ": " + WmiQuery);
                return null;
            }
            
        }

        public static ManagementObjectCollection GetWmiManagementObjectCollection(string NameSpace, string WmiQuery)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(NameSpace, WmiQuery))
                { return searcher.Get(); }
            }
            catch
            {
                LoggerFacade.Error("Error running query against namespace " + NameSpace + ": " + WmiQuery);
                return null;
            }
        }

        public static List<ManagementObject> GetWmiManagementObjectList(string NameSpace, string WmiQuery)
        {
            List<ManagementObject> list = new List<ManagementObject>();
            using (ManagementObjectCollection collection = GetWmiManagementObjectCollection(NameSpace, WmiQuery))
            {
                if (collection != null)
                {
                    foreach (ManagementObject m in collection)
                    {
                        list.Add(m);
                    }
                }
            }
            return list;
        }
    }
}
