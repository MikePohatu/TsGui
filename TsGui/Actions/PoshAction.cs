#region license
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
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
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
