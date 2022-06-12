using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TsGui.Scripts
{
    public class BaseScript
    {
        protected bool _exceptionOnError = true;
        protected bool _exceptionOnMissingFile = true;
        protected string _params;

        public string Path { get; private set; }
        public string Name { get; private set; }

        protected BaseScript(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        private void LoadXml(XElement InputXml)
        {
            this._params = XmlHandler.GetStringFromXElement(InputXml, "Params", this._params);
            this._exceptionOnError = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnError", this._exceptionOnError);
            this._exceptionOnMissingFile = XmlHandler.GetBoolFromXElement(InputXml, "HaltOnMissing", this._exceptionOnError);
            this.Name = XmlHandler.GetStringFromXElement(InputXml, "Name", this.Name);

            this.Path = InputXml.Element("Script")?.Value;
            //make sure there is a script to run
            if (string.IsNullOrEmpty(this.Path)) { throw new InvalidOperationException("Script file is required: " + Environment.NewLine + InputXml); }
            
            //fallback in case not set
            if (string.IsNullOrWhiteSpace(this.Name)) { this.Name = this.Path; }
        }
    }
}
