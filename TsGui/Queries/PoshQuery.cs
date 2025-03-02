﻿#region license
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
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using TsGui.Linking;
using System.Management.Automation;
using WindowsHelpers;
using Core.Logging;
using TsGui.Scripts;
using Core;

namespace TsGui.Queries
{
    public class PoshQuery : BaseQuery
    {
        private int _runningCount = 0;
        private Task _processingTask;
        private PoshScript _script;
        private bool _exceptionOnError = true;

        private List<KeyValuePair<string, XElement>> _propertyTemplates;

        public PoshQuery(ILinkTarget owner) : base(owner) { }

        public PoshQuery(XElement InputXml, ILinkTarget owner) : base(owner)
        {
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            string scriptid = XmlHandler.GetStringFromXml(InputXml, "Global", null);

            if (string.IsNullOrEmpty(scriptid))
            {
                if (InputXml.HasElements)
                {
                    XElement scriptx = InputXml.Element("Script");
                    this._script = new PoshScript(scriptx, this._linktarget);
                }
                else
                {
                    this._script = new PoshScript(InputXml, this._linktarget);
                }
                
            }
            else
            {
                this._script = ScriptLibrary.GetScript(scriptid) as PoshScript;
            }
            if (this._script == null) { throw new KnownException($"No script configuration for query:\n{InputXml}", null); }

            this._processingwrangler.Separator = XmlHandler.GetStringFromXml(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXml(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);
            this._propertyTemplates = QueryHelpers.GetTemplatesFromXmlElements(InputXml.Elements("Property"));
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        {
            if (this._reprocess || !this._processed) { await this.ProcessQueryAsync(message); }
        }

        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            if (this._script == null) { throw new KnownException("Script object not defined", "PoshQuery.ProcessQueryAsync"); }

            this._runningCount++;
            PSDataCollection<PSObject> results = null;
            //check the _processingTask to make sure the query isn't already running. 
            if (this._processingTask == null)
            {
                //Now go through the objects returned by the script, and add the relevant values to the wrangler. 
                try
                {
                    //Don't run each and every time unless specifically specified
                    if (this._processed == true && this._reprocess == false) { return this._returnwrangler; }
                    else if (this._processed == true) { this._processingwrangler = this._processingwrangler.Clone(); }


                    this._processingTask = this._script.RunScriptAsync();
                    await this._processingTask;
                    results = this._script.Result.ReturnedObject;
                }
                catch (Exception e)
                {
                    if (this._exceptionOnError)
                    {
                        throw new KnownException($"PowerShell query {this._script.Path} caused an error: {Environment.NewLine}", e.Message);
                    }
                    else
                    {
                        Log.Error(e, $"PowerShell query {this._script.Path} caused an error: {e.Message}");
                    }
                }

                if (results != null)
                {
                    try
                    {
                        this.AddPoshPropertiesToWrangler(this._processingwrangler, results, this._propertyTemplates);
                    }
                    catch (Exception e)
                    {
                        var submessage = $"{Environment.NewLine}Check that script returns a string or simple object e.g. hashtable{Environment.NewLine}";
                        if (this._script.IsInlineScript)
                        {
                            throw new KnownException($"Couldn't process results of PowerShell script: {Environment.NewLine}{this._script.ScriptContent}{submessage}{Environment.NewLine}Error: ", e.Message);
                        }
                        else
                        {
                            throw new KnownException($"Couldn't process results of PowerShell script {this._script.Path}{submessage}{Environment.NewLine}Error: ", e.Message);
                        }
                    }
                }

                if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
                { this._returnwrangler = this._processingwrangler; }
                else { this._returnwrangler = null; }

                this._processingTask = null;
            }
            //if the script is currently processing, wait for it to finish, then return the results from the other run
            else
            {
                Log.Trace($"Script {this._script.Name} run multiple times, run count: {this._runningCount}");
                await this._processingTask;
            }
            
            this._processed = true;
            this._runningCount--;

            return this._returnwrangler;
        }

        private void AddPoshPropertiesToWrangler(ResultWrangler Wrangler, PSDataCollection<PSObject> results, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            if (results == null) { return; }

            foreach (PSObject result in results)
            {
                if (result != null)
                {
                    Wrangler.NewResult();
                    FormattedProperty prop = null;

                    //first check it's not just a string
                    if (TypeHelpers.IsString(result.BaseObject) || TypeHelpers.IsPrimitiveType(result.BaseObject))
                    {
                        prop = new FormattedProperty();
                        prop.Input = result.ToString();
                        Wrangler.AddFormattedProperty(prop);
                    }
                    else
                    {
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
    }
}
