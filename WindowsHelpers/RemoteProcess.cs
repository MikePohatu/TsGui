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
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public class RemoteProcess: IComparable<RemoteProcess>
    {        
        public string Name { get; set; }
        public string Id { get; set; }
        public string Username { get; set; }
        public string SessionID { get; set; }
        public string StartTime { get; set; }
        public string Path { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }

        public static string GetScript { get; } = "Get-Process -IncludeUserName | select Name, Id, Path, Username, SessionID, StartTime, Company, Product";

        public static RemoteProcess Create(PSObject resultobject)
        {
            RemoteProcess proc = new RemoteProcess();
            proc.Name = PoshHandler.GetPropertyValue<string>(resultobject, "Name");
            proc.Id = PoshHandler.GetPropertyValue<string>(resultobject, "Id");
            proc.Path = PoshHandler.GetPropertyValue<string>(resultobject, "Path");
            proc.Company = PoshHandler.GetPropertyValue<string>(resultobject, "Company");
            proc.Product = PoshHandler.GetPropertyValue<string>(resultobject, "Product");
            proc.Username = PoshHandler.GetPropertyValue<string>(resultobject, "Username");
            proc.SessionID = PoshHandler.GetPropertyValue<string>(resultobject, "SessionID");
            proc.StartTime = PoshHandler.GetPropertyValue<string>(resultobject, "StartTime");
            return proc;
        }

        public async Task KillAsync()
        {
            try
            {
                string script = "Stop-Process -Force -ID " + this.Id;
                using (var posh = new PoshHandler(script, RemoteSystem.Current))
                {
                    await posh.InvokeRunnerAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error killing process " + this.Name + ", pid: " + this.Id);
            }
        }

        public int CompareTo(RemoteProcess other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
