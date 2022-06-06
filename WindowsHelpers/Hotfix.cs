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
    public class Hotfix
    {
        public static string GetterCommand { get; } = "Get-HotFix | Select-Object HotFixID, Caption, InstalledOn, InstalledBy, Description";
        
        public string HotFixID { get; set; }
        public string Description { get; set; }
        public string InstalledOn { get; set; }
        public string InstalledBy { get; set; }
        public string Caption { get; set; }

        public Hotfix(PSObject poshObj)
        {
            this.Description = PoshHandler.GetPropertyValue<string>(poshObj, "Description");
            this.InstalledOn = PoshHandler.GetPropertyValue<string>(poshObj, "InstalledOn");
            this.HotFixID = PoshHandler.GetPropertyValue<string>(poshObj, "HotFixID");
            this.InstalledBy = PoshHandler.GetPropertyValue<string>(poshObj, "InstalledBy");
            this.Caption = PoshHandler.GetPropertyValue<string>(poshObj, "Caption");
        }

        public async Task UninstallAsync()
        {
            if (string.IsNullOrWhiteSpace(this.HotFixID) == false)
            {
                //string command = "Start-Process wusa.exe -ArgumentList \"/uninstall /kb:"+this.HotFixID+" /quiet /norestart\"";
                string scriptPath = AppDomain.CurrentDomain.BaseDirectory + "Scripts\\Remove-Hotfix.ps1";
                string script = await IOHelpers.ReadFileAsync(scriptPath) + "Remove-Hotfix -KB " + this.HotFixID;

                Log.Info("Uninstalling hotfix " + this.HotFixID);
                using (var posh = new PoshHandler(script, RemoteSystem.Current))
                {
                    await posh.InvokeRunnerAsync(true, true);
                }                    
            }
        }
    }
}
