#region license
// Copyright (c) 2026 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Management.Automation;
using WindowsHelpers;

namespace TsGui.Authentication.Script
{
    /// <summary>
    /// The object to be returned by the script. 
    /// </summary>
    public class ScriptAuthObject
    {
        public bool IsAuthenticated { get; set; } = false;
        public bool IsAuthorized { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        public Dictionary<string, bool> GroupMemberships { get; set; }

        /// <summary>
        /// 0 = OK, 1 = Warning, 2 = Error, 3 = Fatal
        /// </summary>
        public int ErrorLevel { get; set; } = 0;

        public static ScriptAuthObject GetAuthFromPosh(PSDataCollection<PSObject> results)
        {
            var authObj = new ScriptAuthObject();
            foreach (PSObject result in results)
            {
                authObj.IsAuthenticated = PoshHandler.GetPropertyValue<bool>(result, "IsAuthenticated");
                authObj.IsAuthorized = PoshHandler.GetPropertyValue<bool>(result, "IsAuthorized");
                authObj.Message = PoshHandler.GetPropertyValue<string>(result, "Message");
                authObj.GroupMemberships = PoshHandler.GetPropertyHashTable<bool>(result, "GroupMemberships");
            }

            return authObj;
        }

    }
}
