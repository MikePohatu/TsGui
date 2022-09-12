using Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Scripts
{
    public abstract class BaseScript
    {
        protected ScriptSettings _settings;
        protected string _scriptcontent = string.Empty;
        protected bool _exceptionOnError = false;
        protected bool _exceptionOnMissingFile = true;
        protected string _params;

        public string Path { get; protected set; }
        public string Name { get; protected set; }


        protected virtual void LoadXml(XElement InputXml)
        {
            this._params = XmlHandler.GetStringFromXElement(InputXml, "Params", this._params);
            this._exceptionOnError = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnError", this._exceptionOnError);
            this._exceptionOnMissingFile = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnMissing", this._exceptionOnError);
            this.Name = XmlHandler.GetStringFromXAttribute(InputXml, "Name", this.Name);
            this.Name = XmlHandler.GetStringFromXElement(InputXml, "Name", this.Name);
            this.Path = XmlHandler.GetStringFromXAttribute(InputXml, "Path", this.Path);
            this.Path = XmlHandler.GetStringFromXElement(InputXml, "Path", this.Path);

            //make sure there is a script set
            if (string.IsNullOrWhiteSpace(this.Name) && string.IsNullOrEmpty(this.Path))
            {
                throw new KnownException("Script is defined but no name or path is set", null);
            }

            //build a list of path options to check
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            List<string> testpaths = new List<string>();
            if (string.IsNullOrWhiteSpace(this.Path)==false)
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

        public abstract Task RunScriptAsync();
    }
}
