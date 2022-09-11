using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using System.Management.Automation;
using WindowsHelpers;
using Core.Logging;
using TsGui.Scripts;

namespace TsGui.Scripts
{
    public class PoshScript: BaseScript
    {
        public ScriptResult<PSDataCollection<PSObject>> Result { get; private set; }

        public PoshScript(XElement InputXml) : base(InputXml) { }

        /// <summary>
        /// Run the posh script. Results can be consumed from the Result property when finished
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task RunScriptAsync()
        {
            this.Result = new ScriptResult<PSDataCollection<PSObject>>();

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
                        throw new KnownException($"PowerShell script not found: {this.Path}", "File not found");
                    }
                    else
                    {
                        Log.Error($"PowerShell script not found: {this.Path}");
                        return;
                    }
                }

                using (var posh = new PoshHandler(script))
                {
                    this.Result.ReturnedObject = await posh.InvokeRunnerAsync();
                    this.Result.ReturnCode = 0;
                }
            }
            catch (Exception e)
            {
                if (this._exceptionOnError)
                {
                    throw new KnownException($"PowerShell script {this.Path} caused an error: {Environment.NewLine}", e.Message);
                }
                else
                {
                    Log.Error(e, $"PowerShell script {this.Path} caused an error: {e.Message}");
                }
            }
        }
    }
}
