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
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using WindowsHelpers;
using Core.Logging;
using System.Diagnostics;

namespace TsGui.Scripts
{
    public class BatchScript: BaseScript
    {
        public ScriptResult<string> Result { get; private set; }

        public BatchScript(XElement InputXml): base() 
        { this.LoadXml(InputXml); }

        /// <summary>
        /// Run the script. Results can be consumed from the Result property when finished
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public override async Task RunScriptAsync()
        {
            this.Result = new ScriptResult<string>();

            //Now go through the objects returned by the script, and add the relevant values to the wrangler. 
            try
            {
                string script = string.Empty;
                string scriptroot = AppDomain.CurrentDomain.BaseDirectory + @"\scripts\";
                if (System.IO.File.Exists(this.Path))
                {
                    script = await IOHelpers.ReadFileAsync(this.Path);
                }
                else if (System.IO.File.Exists(scriptroot + this.Path))
                {
                    script = await IOHelpers.ReadFileAsync(scriptroot + this.Path);
                }
                else
                {
                    if (this._exceptionOnMissingFile)
                    {
                        throw new KnownException($"Batch script not found: {this.Path}", "File not found");
                    }
                    else
                    {
                        Log.Error($"Batch script not found: {this.Path}");
                        return;
                    }
                }

                using (var posh = new PoshHandler(script))
                {
                    Process proc = AsyncHelpers.GetProcess(this.Path, this._params);
                    this.Result.ReturnCode = await AsyncHelpers.StartProcessAsync(proc);
                    this.Result.ReturnedObject = proc.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                if (this._exceptionOnError)
                {
                    throw new KnownException($"Batch script {this.Path} caused an error: {Environment.NewLine}", e.Message);
                }
                else
                {
                    Log.Error(e, $"Batch script {this.Path} caused an error: {e.Message}");
                }
            }
        }
    }
}
