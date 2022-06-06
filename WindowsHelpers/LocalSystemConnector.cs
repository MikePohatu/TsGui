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

// SystemConnector.cs - class to connect to standard Windows components (WMI and 
// environment variables. 

using System;
using System.Management;
using System.Collections.Generic;
using Core.Logging;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public static class LocalSystemConnector
    {
        public static string GetVariableValue(string Variable)
        {
            if (Variable == null) { return null; }
            string s;

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


        /// <summary>
        /// Get a value from WMI, using the root\CIMV2 namespace
        /// </summary>
        /// <param name="WmiQuery"></param>
        /// <returns></returns>
        public static async Task<string> GetWmiStringAsync(string query)
        {
            var results = await WmiQuery.Create(WmiQuery.DefaultNamespace, query).RunLocalAsync();
            return WmiQuery.GetWmiResultsAsString(results);
        }

        /// <summary>
        /// /get a ManagementBaseObject List from WMI, specifying the desired namespace
        /// </summary>
        /// <param name="NameSpace"></param>
        /// <param name="WmiQuery"></param>
        public static async Task<IEnumerable<ManagementBaseObject>> GetWmiManagementObjectListAsync(string NameSpace, string WmiQuery)
        {
            try
            {
                WmiQuery query = new WmiQuery(NameSpace, WmiQuery);
                var results = await query.RunLocalAsync();
                return results;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error running query against namespace " + NameSpace + ": " + WmiQuery);
                throw e;
            }
        }
    }
}