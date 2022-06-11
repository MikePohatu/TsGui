using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using TsGui.Linking;
using System.Management.Automation;
using WindowsHelpers;
using Core.Logging;

namespace TsGui.Queries
{
    public class PoshQuery : BaseQuery
    {
        private string _scriptPath;
        private bool _exceptionOnError = true;
        private bool _exceptionOnMissingFile = true;

        private List<KeyValuePair<string, XElement>> _propertyTemplates;

        public PoshQuery(ILinkTarget owner) : base(owner) { }

        public PoshQuery(XElement InputXml, ILinkTarget owner) : base(owner)
        {
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this._exceptionOnError = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnError", this._exceptionOnError);
            this._exceptionOnMissingFile = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnMissing", this._exceptionOnError);

            this._scriptPath = InputXml.Element("Script")?.Value;
            //make sure there is a script to run
            if (string.IsNullOrEmpty(this._scriptPath)) { throw new InvalidOperationException("Script file is required: " + Environment.NewLine + InputXml); }
            this._processingwrangler.Separator = XmlHandler.GetStringFromXElement(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXElement(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);
            this._propertyTemplates = QueryHelpers.GetTemplatesFromXmlElements(InputXml.Elements("Property"));
        }

        public override async Task<ResultWrangler> ProcessQuery(Message message)
        {
            //Now go through the objects returned by the script, and add the relevant values to the wrangler. 
            try
            {
                //Don't run each and every time unless specifically specified
                if (this._processed == true && this._reprocess == false) { return this._returnwrangler; }
                else if (this._processed == true) { this._processingwrangler = this._processingwrangler.Clone(); }

                string script = string.Empty;
                string scriptroot = AppDomain.CurrentDomain.BaseDirectory + @"\scripts\";
                if (System.IO.File.Exists(this._scriptPath))
                {
                    script = await IOHelpers.ReadFileAsync(this._scriptPath);
                }
                else if (System.IO.File.Exists(scriptroot + this._scriptPath))
                {
                    script = await IOHelpers.ReadFileAsync(scriptroot + this._scriptPath);
                }
                else
                {
                    if (this._exceptionOnMissingFile)
                    {
                        throw new KnownException($"PowerShell script not found: {this._scriptPath}", "File not found");
                    }
                    else
                    {
                        Log.Error($"PowerShell script not found: {this._scriptPath}");
                        return null;
                    }
                }

                using (var posh = new PoshHandler(script))
                {
                    var results = await posh.InvokeRunnerAsync();
                    this.AddPoshPropertiesToWrangler(this._processingwrangler, results, this._propertyTemplates);
                }
            }
            catch (Exception e)
            {
                if (this._exceptionOnError)
                {
                    throw new KnownException($"PowerShell script {this._scriptPath} caused an error: {Environment.NewLine}", e.Message);
                }
                else
                {
                    Log.Error(e, $"PowerShell script {this._scriptPath} caused an error: {e.Message}");
                }
            }

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            return this._returnwrangler;
        }

        private void AddPoshPropertiesToWrangler(ResultWrangler Wrangler, PSDataCollection<PSObject> results, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            foreach (PSObject result in results)
            {
                Wrangler.NewResult();
                FormattedProperty prop = null;

                //if properties have been specified in the xml, query them directly in order
                if (PropertyTemplates.Count != 0)
                {
                    foreach (KeyValuePair<string, XElement> template in PropertyTemplates)
                    {
                        prop = new FormattedProperty(template.Value);
                        prop.Input = PoshHandler.GetPropertyValue<string>(result, template.Key);
                        Wrangler.AddFormattedProperty(prop);
                    }
                }
                //if properties not set, add them all 
                else
                {
                    foreach (PSPropertyInfo property in result.Properties)
                    {
                        prop = new FormattedProperty();
                        prop.Input = property.Value?.ToString();
                        Wrangler.AddFormattedProperty(prop);
                    }
                }
            }
        }
    }
}
