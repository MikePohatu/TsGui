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
            this._script = new PoshScript(InputXml);
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
