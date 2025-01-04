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
using Core.Diagnostics;
using MessageCrap;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Queries;

namespace TsGui.Scripts
{
    public class Parameter: ILinkTarget, IParameter
    {
        private ILinkTarget _linktarget;
        private QueryPriorityList _querylist;
        public string Name { get; set; }
        public bool IsSwitch { get; set; } = false;

        public Parameter(XElement InputXml)
        {
            this._linktarget = this;
            this.LoadXml(InputXml);
        }

        public Parameter(XElement InputXml, ILinkTarget target)
        {
            this._linktarget = target == null ? this : target;
            this.LoadXml(InputXml);
        }

        public async Task<object> GetValue(Message message)
        {
            var wrangler = await _querylist?.GetResultWrangler(message);
            return wrangler?.GetString();
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml.Name == "Switch") { this.IsSwitch = true; }

            this.Name = XmlHandler.GetStringFromXml(InputXml, "Name", this.Name);
            this.Name = XmlHandler.GetStringFromXml(InputXml, "Name", this.Name);
            string value = XmlHandler.GetStringFromXml(InputXml, "Value", null);

            //validation values
            if (string.IsNullOrEmpty(this.Name)) { throw new KnownException($"Parameter name not defined:\n{InputXml}", null); }

            //Values set as elements win over attributes
            XElement set = InputXml.Element("SetValue");
            if (set != null)
            {
                this._querylist = new QueryPriorityList(set, this._linktarget);
            }

            if (this._querylist == null || this._querylist.Queries.Count == 0)
            {
                if (string.IsNullOrEmpty(value) && this.IsSwitch == false) { throw new KnownException($"Parameter {this.Name} does not define a value:\n{InputXml}", null); }
                this._querylist = new QueryPriorityList(this._linktarget);
                this._querylist.AddQuery(new ValueOnlyQuery(value));
            }
        }

        //do nothing on source value updated. These are triggered by the script running and querying
        //the parameter
        public async Task OnSourceValueUpdatedAsync(Message message) 
        {
            await Task.CompletedTask;
        }
    }
}
