using Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Scripts
{
    public abstract class BaseScript
    {
        protected ScriptSettings _settings = new ScriptSettings();
        protected bool _exceptionOnError = false;
        protected bool _exceptionOnMissingFile = true;
        protected string _params;

        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public string ID { get; protected set; }
        public bool IsInlineScript { get; protected set; } = false;
        public string ScriptContent { get; protected set; } = string.Empty;

        protected virtual void LoadXml(XElement InputXml)
        {
            this._params = XmlHandler.GetStringFromXml(InputXml, "Params", this._params);
            this._exceptionOnError = XmlHandler.GetBoolFromXml(InputXml, "HaltOnError", this._exceptionOnError);
            this._exceptionOnMissingFile = XmlHandler.GetBoolFromXml(InputXml, "HaltOnMissing", this._exceptionOnError);
            this.Name = XmlHandler.GetStringFromXml(InputXml, "Name", this.Name);
            this.Path = XmlHandler.GetStringFromXml(InputXml, "Path", this.Path);
            this.ID = XmlHandler.GetStringFromXml(InputXml, "ID", this.ID);

            if (InputXml.Elements().Count() == 0)
            {
                this.IsInlineScript = true;
                this.ScriptContent = InputXml.Value;
            }

            //make sure there is a script set
            if (string.IsNullOrWhiteSpace(this.Name) 
                && string.IsNullOrEmpty(this.Path) && string.IsNullOrWhiteSpace(this.ScriptContent))
            {
                throw new KnownException("Script is defined but no name, path, or inline code is set", null);
            }

            //build a list of path options to check
            if (this.IsInlineScript==false)
            {
                string exefolder = AppDomain.CurrentDomain.BaseDirectory;
                List<string> testpaths = new List<string>();
                if (string.IsNullOrWhiteSpace(this.Path) == false)
                {
                    testpaths.Add(this.Path);
                }
                testpaths.Add($"{exefolder}\\Scripts\\{this.Name}");
                testpaths.Add($"{exefolder}\\{this.Name}");

                //try to find the script
                foreach (string path in testpaths)
                {
                    if (File.Exists(path))
                    {
                        this.Path = path;
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(this.Path)) { throw new KnownException($"Failed to find script file: {this.Name}", null); }

                //set name in case not set
                if (string.IsNullOrWhiteSpace(this.Name)) { this.Name = this.Path; }
            }            
        }

        public abstract Task RunScriptAsync();
    }
}
