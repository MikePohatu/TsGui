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
using System.IO;
using System.Text;

namespace TsGui.Scripts
{
    public class PoshScript: BaseScript
    {
        private ILinkTarget _linktarget;
        public List<Parameter> Parameters { get; private set; } = new List<Parameter>();

        public ScriptResult<PSDataCollection<PSObject>> Result { get; private set; }

        public PoshScript(XElement InputXml) : base() 
        { 
            this.LoadXml(InputXml);
        }

        public PoshScript(XElement InputXml, ILinkTarget target) : base() 
        {
            this._linktarget = target;
            this.LoadXml(InputXml);
        }

        protected override void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            foreach (XElement x in InputXml.Elements("Switch"))
            {
                Parameter p = new Parameter(x, this._linktarget);
                this.Parameters.Add(p);
            }
            foreach (XElement x in InputXml.Elements("Parameter"))
            {
                Parameter p = new Parameter(x, this._linktarget);
                this.Parameters.Add(p);
            }
        }


        /// <summary>
        /// Load the script file 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public async Task LoadScriptAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Path) == false)
            {
                this._scriptcontent = await IOHelpers.ReadFileAsync(this.Path);

                if (string.IsNullOrWhiteSpace(this._scriptcontent) == false)
                {
                    using (StringReader reader = new StringReader(this._scriptcontent))
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
                                    if (line.TrimStart().ToLower().StartsWith("scriptsettings#>")) { readingsettings = false; }
                                    else { builder.AppendLine(line); }
                                }
                                else
                                {
                                    if (line.TrimStart().ToLower().StartsWith("<#scriptsettings")) { readingsettings = true; }
                                }
                            }

                        } while (line != null);

                        string settingsjson = builder.ToString();
                        this._settings = ScriptSettings.Create(settingsjson);
                    }
                }
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
                if (System.IO.File.Exists(this.Path))
                {
                    await this.LoadScriptAsync();
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

                using (var posh = new PoshHandler(this._scriptcontent))
                {
                    foreach (Parameter p in this.Parameters)
                    {
                        var wrangler = await p.GetResultWrangler(null);
                        string value = wrangler.GetString();
                        posh.Runner.AddParameter(p.Name, value);
                    }
                    if (!string.IsNullOrWhiteSpace(this._params)) { posh.Runner.AddArgument(this._params); }

                    this.Result.ReturnedObject = await posh.InvokeRunnerAsync(!this._settings.LogScriptContent, this._settings.LogOutput);
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
