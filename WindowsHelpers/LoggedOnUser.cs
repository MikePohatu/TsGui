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
using System.Management.Automation;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public class LoggedOnUser
    {
        public string UserName { get; set; }
        public string SessionName { get; set; }
        public string ID { get; set; }
        public string State { get; set; }
        public string IdleTime { get; set; }
        public string LogonTime { get; set; }

        public LoggedOnUser(PSObject posh)
        {
            this.UserName = PoshHandler.GetPropertyValue<string>(posh, "UserName");
            this.SessionName = PoshHandler.GetPropertyValue<string>(posh, "SessionName");
            this.ID = PoshHandler.GetPropertyValue<string>(posh, "ID");
            this.State = PoshHandler.GetPropertyValue<string>(posh, "State");
            this.IdleTime = PoshHandler.GetPropertyValue<string>(posh, "IdleTime");
            this.LogonTime = PoshHandler.GetPropertyValue<string>(posh, "LogonTime");
        }

        public async Task LogOff()
        {
            if (string.IsNullOrWhiteSpace(this.ID) == false)
            {
                string script = "logoff " + this.ID;
                using (PoshHandler posh = new PoshHandler(script, RemoteSystem.Current))
                {
                    var results = await posh.InvokeRunnerAsync();
                }
            }
        }
    }
}
