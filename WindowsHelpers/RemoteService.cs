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
    public class RemoteService: ViewModelBase, IComparable<RemoteService>
    {
        private string _displayName;
        public string DisplayName
        {
            get { return this._displayName; }
            set { this._displayName = value; this.OnPropertyChanged(this, "DisplayName"); }
        }

        private string _name;
        public string Name
        {
            get { return this._name; }
            set { this._name = value; this.OnPropertyChanged(this, "Name"); }
        }

        private string _status;
        public string Status
        {
            get { return this._status; }
            set { this._status = value; this.OnPropertyChanged(this, "Status"); }
        }

        public static string GetScript { get; } = "Get-Service";

        public static RemoteService Create(PSObject resultobject)
        {
            RemoteService service = new RemoteService();
            service.Name = PoshHandler.GetPropertyValue<string>(resultobject, "Name");
            service.Status = PoshHandler.GetPropertyValue<string>(resultobject, "Status");
            service.DisplayName = PoshHandler.GetPropertyValue<string>(resultobject, "DisplayName");
            return service;
        }

        public void Refresh(PSObject resultobject)
        {
            if (resultobject != null)
            {
                this.Name = PoshHandler.GetPropertyValue<string>(resultobject, "Name");
                this.Status = PoshHandler.GetPropertyValue<string>(resultobject, "Status");
                this.DisplayName = PoshHandler.GetPropertyValue<string>(resultobject, "DisplayName");
            }
            else
            {
                Log.Error("Can't refresh service " + this.Name + ". Empty results");
            }
        }

        public async Task RestartServiceAsync()
        {
            string scriptPath = AppDomain.CurrentDomain.BaseDirectory + "Scripts\\ServiceControl.ps1";

            try
            {
                string script = await IOHelpers.ReadFileAsync(scriptPath);
                using (PoshHandler posh = new PoshHandler(script, RemoteSystem.Current))
                {
                    posh.Runner.AddStatement().AddCommand("RestartService").AddParameter("ServiceName", this.Name);
                    var result = await posh.InvokeRunnerAsync(true);
                    this.Refresh(result.First());
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error restarting service: " + this.Name);
            }
        }

        public async Task StopServiceAsync()
        {
            string scriptPath = AppDomain.CurrentDomain.BaseDirectory + "Scripts\\ServiceControl.ps1";

            try
            {
                string script = await IOHelpers.ReadFileAsync(scriptPath);
                using (PoshHandler posh = new PoshHandler(script, RemoteSystem.Current))
                {
                    posh.Runner.AddStatement().AddCommand("StopService").AddParameter("ServiceName", this.Name);
                    var result = await posh.InvokeRunnerAsync(true);
                    this.Refresh(result.First());
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error stopping service: " + this.Name);
            }
        }

        public async Task StartServiceAsync()
        {
            string scriptPath = AppDomain.CurrentDomain.BaseDirectory + "Scripts\\ServiceControl.ps1";

            try
            {
                string script = await IOHelpers.ReadFileAsync(scriptPath);
                using (PoshHandler posh = new PoshHandler(script, RemoteSystem.Current))
                {
                    posh.Runner.AddStatement().AddCommand("StartService").AddParameter("ServiceName", this.Name);
                    var result = await posh.InvokeRunnerAsync(true);
                    this.Refresh(result.First());
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error starting service: " + this.Name);
            }
        }

        public int CompareTo(RemoteService other)
        {
            if (string.IsNullOrWhiteSpace(this.DisplayName))
            {
                return this.Name.CompareTo(other.Name);
            }
            else
            {
                return this.DisplayName.CompareTo(other.DisplayName);
            }
        }
    }
}
