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
using TsGui.Scripts;

namespace TsGui.Actions
{
    public class PoshAction : IAction
    {
        private PoshScript _script;

        public PoshAction(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        public void LoadXml(XElement InputXml)
        {
            string scriptid = XmlHandler.GetStringFromXml(InputXml, "Global", null);

            if (string.IsNullOrEmpty(scriptid))
            {
                this._script = new PoshScript(InputXml);
            }
            else
            {
                this._script = ScriptLibrary.GetScript(scriptid) as PoshScript;
            }
            if (this._script == null) { throw new KnownException($"No script configuration for action:\n{InputXml}", null); }

        }

        public void RunAction()
        {
            this.RunActionAsync().ConfigureAwait(false);
        }

        public async Task RunActionAsync()
        {
            await this._script.RunScriptAsync();
        }
    }
}
