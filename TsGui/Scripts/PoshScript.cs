using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using System.Management.Automation;
using WindowsHelpers;
using Core.Logging;
using TsGui.Scripts;

namespace TsGui.Scipts
{
    public class PoshScript
    {
        private bool _exceptionOnError = true;
        private bool _exceptionOnMissingFile = true;
        private ScriptResult<PSDataCollection<PSObject>> _result;

        public string Path { get; private set; }

        public PoshScript(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        public void LoadXml(XElement InputXml)
        {
            this._exceptionOnError = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnError", this._exceptionOnError);
            this._exceptionOnMissingFile = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnMissing", this._exceptionOnError);

            this.Path = InputXml.Element("Script")?.Value;
            //make sure there is a script to run
            if (string.IsNullOrEmpty(this.Path)) { throw new InvalidOperationException("Script file is required: " + Environment.NewLine + InputXml); }
        }

        public async Task<PSDataCollection<PSObject>> RunScriptAsync()
        {
            this._result = new ScriptResult<PSDataCollection<PSObject>>();

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
                        return null;
                    }
                }

                using (var posh = new PoshHandler(script))
                {
                    this._result.ReturnedObject = await posh.InvokeRunnerAsync();
                    this._result.ReturnCode = 0;
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

            return this._result.ReturnedObject;
        }
    }
}
