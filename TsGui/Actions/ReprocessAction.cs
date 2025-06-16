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
using TsGui.Linking;
using MessageCrap;
using System.Collections.Generic;
using System.Linq;
using TsGui.Scripts;

namespace TsGui.Actions
{
    public class ReprocessAction : IAction
    {
        List<string> _sourceIds;
        List<IReprocessable> _sources;

        public ReprocessAction(XElement InputXml)
        {
            this.LoadXml(InputXml);
            Director.Instance.ConfigLoadFinished += OnConfigLoadFinished;
        }

        private void OnConfigLoadFinished(object sender, System.EventArgs e)
        {
            foreach (var sourceId in this._sourceIds)
            {
                var linkingsource = LinkingHub.Instance.GetSourceOption(sourceId) as IReprocessable;
                if (linkingsource != null) { this._sources.Add(linkingsource); }
                else
                {
                    var script = ScriptLibrary.GetScript(sourceId);
                    this._sources.Add(script);
                }
            }
        }

        public void LoadXml(XElement InputXml)
        {
            //init lists before load/reload
            this._sources = new List<IReprocessable>();
            this._sourceIds = new List<string>();

            foreach (var element in InputXml.Elements("ID"))
            {
                string sourceId = element.Value;
                this._sourceIds.Add(sourceId);
            }

            //nothing found, support SourceID as an attribute to add one Source
            if (this._sourceIds.Count == 0)
            {
                var sourceId = XmlHandler.GetStringFromXml(InputXml, "ID", string.Empty);
                if (string.IsNullOrEmpty(sourceId) == false)
                {
                    this._sourceIds.Add(sourceId);
                }

            }
        }

        public void RunAction()
        {
            this.RunActionAsync().ConfigureAwait(false);
        }

        public async Task RunActionAsync()
        {
            if (this._sources.Count == 0) { return; }
            foreach (var source in this._sources)
            {
                await source.OnReprocessAsync();
            }
        }
    }
}
