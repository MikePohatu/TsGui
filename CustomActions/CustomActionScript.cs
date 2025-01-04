#region license
// Copyright (c) 2025 Mike Pohatu
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
using Core.Logging;
using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using WindowsHelpers;
using System.Collections.ObjectModel;
using Core;
//using ConfigMgrHelpers;
using System.Collections.Generic;

namespace CustomActions
{
    public class CustomActionScript: ViewModelBase, IComparable<CustomActionScript>, IDisposable
    {
        /// <summary>
        /// Action script has been run successfully
        /// </summary>
        public event EventHandler RunCompleted;

        private bool _loaded = false;
        private string _scriptpath = string.Empty;
        private string _script = string.Empty;
        private string _filename = string.Empty;

        /// <summary>
        /// The actual script name
        /// </summary>
        public string Name { get { return this._filename; } }


        /// <summary>
        /// Is the action currently running
        /// </summary>
        private bool _isrunning = false;
        public bool IsRunning
        {
            get { return this._isrunning; }
            set { this._isrunning = value; this.OnPropertyChanged(this, "IsRunning"); }
        }

        /// <summary>
        /// The display name of the script. Will return the Name attribute if not set in the Settings
        /// </summary>
        public string DisplayName
        {
            get { return string.IsNullOrWhiteSpace(this.Settings?.DisplayName) == true ? this._filename : this.Settings.DisplayName; }
        }
        public CustomActionSettings Settings { get; private set; }

        /// <summary>
        /// The data that can be used by a TableViewer control
        /// </summary>
        public ObservableCollection<object> TableData { get; } = new ObservableCollection<object>();

        /// <summary>
        /// The results from the action script
        /// </summary>
        public ObservableCollection<PSObject> ResultList { get; set; }

        public CustomActionScript()
        {
            if (this.Settings == null || this.Settings.RunOnClient)
            {
                RemoteSystem.Connected += this.OnConnected;
                RemoteSystem.Connecting += this.OnConnecting;
            }
            //else
            //{
            //    CmServer.Connected += this.OnConnected; 
            //    CmServer.Connecting += this.OnConnecting;
            //}
        }

        /// <summary>
        /// Cleanup event registrations
        /// </summary>
        public void Dispose()
        {

            if (this.Settings == null || this.Settings.RunOnClient)
            {
                RemoteSystem.Connected -= this.OnConnected;
                RemoteSystem.Connecting -= this.OnConnecting;
            }
            //else
            //{
            //    CmServer.Connected -= this.OnConnected;
            //    CmServer.Connecting -= this.OnConnecting;
            //}
        }

        public int CompareTo(CustomActionScript other)
        {
            if (other == null) return 1;
            return this.DisplayName.CompareTo(other.DisplayName);
        }

        public async void OnConnected (object sender, EventArgs args)
        {
            if (this.Settings != null && this.Settings.RunOnConnect == true)
            {
                await this.RunActionAsync();
            }
        }

        public void OnConnecting(object sender, EventArgs args)
        {
            this.TableData.Clear();
        }

        /// <summary>
        /// Load the script file 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public async Task Load(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath) == false)
            {
                this._scriptpath = filepath;
                this._script = await IOHelpers.ReadFileAsync(filepath);

                if (string.IsNullOrWhiteSpace(this._script) == false)
                {
                    this._loaded = true;

                    using (StringReader reader = new StringReader(this._script))
                    {
                        bool readingsettings = false;
                        StringBuilder builder = new StringBuilder();
                        string line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null)
                            {
                                if (readingsettings)
                                {
                                    if (line.TrimStart().ToLower().StartsWith("actionsettings#>")) { readingsettings = false; }
                                    else { builder.AppendLine(line); }
                                }
                                else
                                {
                                    if (line.TrimStart().ToLower().StartsWith("<#actionsettings")) { readingsettings = true; }
                                }
                            }

                        } while (line != null);

                        string settingsjson = builder.ToString();
                        if (string.IsNullOrWhiteSpace(settingsjson) == false)
                        {
                            this.Settings = CustomActionSettings.Create(settingsjson);
                        }
                    }
                    this._filename = Path.GetFileNameWithoutExtension(filepath);
                }
            }
        }

        /// <summary>
        /// Run the action. Return false with error, otherwise true
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RunActionAsync()
        {
            if (this._loaded == true && string.IsNullOrWhiteSpace(this._script) == false)
            {
                // if runonclient and remote system isn't connected, bail out
                if ((this.Settings == null || this.Settings.RunOnClient) && (RemoteSystem.Current == null || RemoteSystem.Current.IsConnected == false))
                {
                    Log.Warn("Remote system not connected. Please connect before running " + this.DisplayName);
                    return false;
                }

                //if (this.Settings.RequiresServerConnect && ( CmServer.Current == null || CmServer.Current.IsConnected == false))
                //{
                //    Log.Warn(this.DisplayName +" requires connect before it can be run");
                //    return false;
                //}
                
                Log.Info("Running custom action: " + this.DisplayName);
                this.IsRunning = true;
                this.TableData.Clear();
                bool hidescript = this.Settings == null ? false : !this.Settings.LogScriptContent;

                string sanitisedscript = this.ReplaceScriptTokens(this._script);

                PoshHandler posh;
                if (this.Settings.RunOnClient)
                {
                    posh = new PoshHandler(sanitisedscript, RemoteSystem.Current);
                }
                else
                {
                    posh = new PoshHandler(sanitisedscript);
                }

                PSDataCollection<PSObject> results;
                if (this.Settings.LogOutput|| this.Settings.DisplayElement == DisplayElements.Log)
                {
                    results = await posh.InvokeRunnerAsync(hidescript, true);
                }
                else
                {
                    results = await posh.InvokeRunnerAsync(hidescript, false);
                }

                this.ResultList = new ObservableCollection<PSObject>();
                foreach (var obj in results)
                {
                    this.ResultList.Add(obj);
                    this.TableData.Add(obj);
                }

                posh.Dispose();
                this.IsRunning = false;
                RunCompleted?.Invoke(this, new EventArgs());
                return true;
            }
            else
            {
                Log.Warn("Script hasn't finished loading yet. Please try again soon. Script: " + this._scriptpath);
                this.IsRunning = false;
                return false;
            }
        }

        private string ReplaceScriptTokens(string script)
        {
            if (string.IsNullOrWhiteSpace(script)) { return null; }
            string sanitised = script;
            if (RemoteSystem.Current != null && string.IsNullOrWhiteSpace(RemoteSystem.Current.ComputerName) == false) { sanitised = sanitised.Replace("{{CLIENT}}", RemoteSystem.Current.ComputerName); }
            //if (CmServer.Current != null && string.IsNullOrWhiteSpace(CmServer.Current.ServerName) == false)
            //{
            //    sanitised = sanitised.Replace("{{CM_SERVER}}", CmServer.Current.ServerName);
            //    sanitised = sanitised.Replace("{{CM_SITE}}", CmServer.Current.SiteCode);
            //    sanitised = sanitised.Replace("{{CM_SITE_NAMESPACE}}", CmServer.Current.SiteWmiNamespace);
            //}                

            return sanitised;
        }
    }
}
