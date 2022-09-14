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
using Core;
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public class InstalledApplication
    {
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Version { get; set; }
        public string BitDepth { get; set; }
        public string UninstallString { get; set; }

        public InstalledApplication(PSObject poshObj)
        {
            this.Name = PoshHandler.GetPropertyValue<string>(poshObj, "Name");
            this.Publisher = PoshHandler.GetPropertyValue<string>(poshObj, "Publisher");
            this.Version = PoshHandler.GetPropertyValue<string>(poshObj, "Version");
            this.BitDepth = PoshHandler.GetPropertyValue<string>(poshObj, "BitDepth");
            this.UninstallString = PoshHandler.GetPropertyValue<string>(poshObj, "UninstallString");
        }

        public async Task UninstallAsync()
        {
            if (string.IsNullOrWhiteSpace(this.UninstallString))
            {
                Log.Error("No uninstall string specified for " + this.Name);
            }
            else 
            {
                string installargs = this.UninstallString;
                string loweruninstall = this.UninstallString.ToLower();

                if (loweruninstall.Trim().StartsWith("msiexec")==false)
                {
                    Log.Error("Sorry only MSI/msiexec uninstalls are supported");
                }
                else
                {
                    if (loweruninstall.Contains("/qb"))
                    {
                        installargs = installargs.Replace("/qb", "/qn", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (loweruninstall.Contains("/q ") || loweruninstall.EndsWith("/q"))
                    {
                        installargs = installargs.Replace("/q", "/qn", StringComparison.OrdinalIgnoreCase);
                    }
                    else if (!loweruninstall.Contains("/q"))
                    {
                        installargs = installargs + " /qn";
                    }

                    var split = installargs.Split(' ');
                    var cmd = split[0];
                    installargs = installargs.Replace(cmd, "");

                    string command = "Start-Process " + cmd + " -Wait -ArgumentList '" + installargs +"'";

                    Log.Info("Uninstalling application " + this.Name);
                    using (var posh = new PoshHandler(command, RemoteSystem.Current))
                    {
                        await posh.InvokeRunnerAsync();
                    }
                }                
            }
        }
    }
}
