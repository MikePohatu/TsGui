#region license
// Copyright (c) 2021 20Road Limited
//
// This file is part of DevChecker.
//
// DevChecker is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Logging;
using Core.Diagnostics;
using System.Management;

namespace WindowsHelpers
{
    public class WmiQuery
    {
        public static string DefaultNamespace { get; } = @"root\CIMV2";
        public string NameSpace { get; private set; } = DefaultNamespace;
        public string QueryString { get; private set; }

        public string ComputerName { get; private set; } = ".";
        public bool Completed { get; private set; } = false;

        /// <summary>
        /// Create a query specifying the WMI namespace, and the WMI query
        /// </summary>
        /// <param name="NameSpace"></param>
        /// <param name="WmiQuery"></param>
        public WmiQuery(string NameSpace, string WmiQuery)
        {
            this.NameSpace = string.IsNullOrWhiteSpace(NameSpace) ? DefaultNamespace : NameSpace;
            this.QueryString = WmiQuery;
        }

        /// <summary>
        /// Create a query the WMI namespace, and the WMI query, and the remote computer name
        /// </summary>
        /// <param name="NameSpace"></param>
        /// <param name="WmiQuery"></param>
        /// <param name="ComputerName"></param>
        public WmiQuery(string NameSpace, string WmiQuery, string ComputerName)
        { 
            this.ComputerName = string.IsNullOrWhiteSpace(ComputerName) ? "." : ComputerName;
            this.NameSpace = string.IsNullOrWhiteSpace(NameSpace) ? DefaultNamespace : NameSpace;
            this.QueryString = WmiQuery;
        }


        /// <summary>
        /// Create query with the default root\CIMV2 namespace
        /// </summary>
        /// <param name="WmiQuery"></param>
        public WmiQuery(string WmiQuery)
        {
            this.QueryString = WmiQuery;
        }

        /// <summary>
        /// Static method to allow for chaining of methods e.g.. Create(...).RunAsync()
        /// </summary>
        /// <param name="NameSpace"></param>
        /// <param name="WmiQuery"></param>
        /// <param name="ComputerName"></param>
        /// <returns></returns>
        public static WmiQuery Create(string NameSpace, string WmiQuery, string ComputerName)
        {
            return new WmiQuery(NameSpace, WmiQuery, ComputerName);
        }

        /// <summary>
        /// Static method to allow for chaining of methods e.g.. Create(...).RunAsync()
        /// </summary>
        /// <param name="NameSpace"></param>
        /// <param name="WmiQuery"></param>
        /// <returns></returns>
        public static WmiQuery Create(string NameSpace, string WmiQuery)
        {
            return new WmiQuery(NameSpace, WmiQuery);
        }

        /// <summary>
        /// Static method to allow for chaining of methods e.g.. Create(...).RunAsync()
        /// </summary>
        /// <param name="NameSpace"></param>
        /// <returns></returns>
        public static WmiQuery Create(string NameSpace)
        {
            return new WmiQuery(NameSpace);
        }

        /// <summary>
        /// Run the query using the options configured on the object. Not supported on remote connections. ComputerName will be ignored
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ManagementBaseObject>> RunLocalAsync()
        {
            if (this.Completed)
            {
                string message = "Query has already been run: \\\\.\\" + this.NameSpace + " : " + this.QueryString;
                Log.Error(message);
                throw new KnownException(message, "");
            }

            try
            {
                this._asyncresults = new List<ManagementBaseObject>();
                ManagementScope scope = new ManagementScope("\\\\.\\" + this.NameSpace);
                ObjectQuery query = new ObjectQuery(this.QueryString);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementOperationObserver observer = new ManagementOperationObserver();

                // Attach handler to events for results and completion.  
                observer.ObjectReady += new ObjectReadyEventHandler(this.NewObject);
                observer.Completed += new CompletedEventHandler(this.Done);

                // Call the asynchronous overload of Get()  
                // to start the enumeration.  
                searcher.Get(observer);

                // Do something else while results  
                // arrive asynchronously.  
                while (!this.Completed)
                {
                    await Task.Delay(500);
                }

                return this._asyncresults;
            }
            catch (UnauthorizedAccessException e)
            {
                this.Completed = true;
                Log.Error("Access denied to computer. " + e.Message);
                throw e;
            }
            catch (Exception e)
            {
                this.Completed = true;
                Log.Error("Failed to run query: " + e.Message);
                throw e;
            }
        }

        private List<ManagementBaseObject> _asyncresults;
        private void Done(object sender, CompletedEventArgs obj)
        {
            this.Completed = true;
        }

        private void NewObject(object sender, ObjectReadyEventArgs obj)
        {
            this._asyncresults.Add(obj.NewObject);
        }

        /// <summary>
        /// Run the query using the options configured on the object
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ManagementBaseObject> Run()
        {
            if (this.Completed)
            {
                string message = "Query has already been run: \\\\" + this.ComputerName + "\\" + this.NameSpace + " : " + this.QueryString;
                Log.Error(message);
                throw new KnownException(message, "");
            }

            try
            {
                ManagementScope scope = new ManagementScope("\\\\" + this.ComputerName + "\\" + this.NameSpace);
                ObjectQuery query = new ObjectQuery(this.QueryString);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                var results = searcher.Get();
                ManagementBaseObject[] outlist = new ManagementBaseObject[results.Count];
                results.CopyTo(outlist,0);
                return outlist;
            }
            catch (UnauthorizedAccessException e)
            {
                this.Completed = true;
                Log.Error("Access denied to computer. " + e.Message);
                throw e;
            }
            catch (Exception e)
            {
                this.Completed = true;
                Log.Error("Failed to run query: " + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Get a string concatenating all property values together
        /// </summary>
        /// <param name="NameSpace"></param>
        /// <param name="WmiQuery"></param>
        /// <returns></returns>
        public static string GetWmiResultsAsString(IEnumerable<ManagementBaseObject> collection)
        {
            string s = null;
            try
            {
                foreach (ManagementBaseObject m in collection)
                {
                    foreach (PropertyData propdata in m.Properties)
                    {
                        s = s + propdata.Value;
                    }
                }

                if (String.IsNullOrEmpty(s)) { return null; }
                else { return s; }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error converting WMI results to string");
                return null;
            }

        }

        /// <summary>
        /// Return a list of values of the property with specified name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static List<T> GetWmiProperties<T>(IEnumerable<ManagementBaseObject> collection, string propertyName)
        {
            List<T> props = new List<T>();

            try
            {
                foreach (ManagementBaseObject m in collection)
                {
                    object val = m.Properties[propertyName].Value;
                    props.Add((T)val);
                }

                return props;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error getting WMI results for property: " + propertyName);
                return null;
            }
        }

        /// <summary>
        /// Return the value of the property with specified name, return the first value found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetWmiProperty<T>(IEnumerable<ManagementBaseObject> collection, string propertyName)
        {
            try
            {
                foreach (ManagementBaseObject m in collection)
                {
                    object val = m.Properties[propertyName].Value;
                    return (T)(val);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error getting WMI result for property: " + propertyName);  
            }
            return default(T);
        }
    }
}
