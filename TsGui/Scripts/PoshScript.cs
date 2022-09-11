using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using System.Management.Automation;
using WindowsHelpers;
using Core.Logging;
using TsGui.Scripts;
using TsGui.Linking;
using TsGui.Queries;
using MessageCrap;
using System.Collections.Generic;

namespace TsGui.Scripts
{
    public class PoshScript: BaseScript
    {
        private ILinkTarget _owner;
        private List<Parameter> _parameters = new List<Parameter>();

        public ScriptResult<PSDataCollection<PSObject>> Result { get; private set; }


        public PoshScript(XElement InputXml) : base(InputXml) 
        {
            this.LoadXml(InputXml);
        }

        public PoshScript(XElement InputXml, ILinkTarget owner) : base(InputXml)
        {
            this.LoadXml(InputXml);
            this._owner = owner;
        }

        private void LoadXml(XElement InputXml)
        {
            foreach (XElement x in InputXml.Elements("Switch"))
            {
                Parameter p = new Parameter(x, this._owner);
                this._parameters.Add(p);
            }
            foreach (XElement x in InputXml.Elements("Paramter"))
            {
                Parameter p = new Parameter(x, this._owner);
                this._parameters.Add(p);
            }
        }

        /// <summary>
        /// Run the posh script. Results can be consumed from the Result property when finished
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public override async Task RunScriptAsync()
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
                    foreach (Parameter p in this._parameters)
                    {
                        var wrangler = await p.GetResultWrangler(null);
                        posh.Runner.AddParameter(p.Name, wrangler.GetString());
                    }
                    if (!string.IsNullOrWhiteSpace(this._params)) { posh.Runner.AddArgument(this._params); }

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
