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
using Core.Logging;
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;

namespace TsGui.Sets
{
    public class Set: GroupableBlindBase, ILinkTarget
    {
        private List<Variable> _variables = new List<Variable>();
        private QueryPriorityList _enabledQueries;
        public List<SetList> SetLists { get; } = new List<SetList>();

        public string Path { get; private set; } = Director.Instance.DefaultPath;
        public string ID { get; private set; }

        public string LiveValue
        {
            get { return this.Enabled && this.IsActive ? "TRUE" : "FALSE"; }
        }

        private bool _enabled = true;
        public bool Enabled
        {
            get { return this._enabled; }
            set 
            { 
                this._enabled = value;
                this.OnPropertyChanged(this, "LiveValue");
            }
        }

        public Set(XElement inputXml): base()
        {
            this.LoadXml(inputXml);
        }

        private new void LoadXml(XElement inputXml)
        {
            base.LoadXml(inputXml);
            this.Path = XmlHandler.GetStringFromXml(inputXml, "Path", this.Path);
            this.ID = XmlHandler.GetStringFromXml(inputXml, "ID", this.ID);

            XElement x = inputXml.Element("Enabled");
            if (x != null)
            {
                this._enabledQueries = new QueryPriorityList(x,this);
            }

            foreach (XElement element in inputXml.Elements("Variable"))
            {
                string name = XmlHandler.GetStringFromXml(element, "Name", null);
                string value = XmlHandler.GetStringFromXml(element, "Value", null);
                if (name != null)
                {
                    this._variables.Add(new Variable(name, value, this.Path));
                }
                else
                {
                    Log.Error("Set contains variable without name");
                }

            }

            foreach (XElement element in inputXml.Elements("List"))
            {
                var list = new SetList(element, this);
                this.SetLists.Add(list);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void EvaluateGroups()
        {
            base.EvaluateGroups();
            this.OnPropertyChanged(this, "LiveValue");
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        {
            foreach (var query in this._enabledQueries.Queries)
            {
                var wrang = await query.ProcessQueryAsync(message);
                if (wrang != null)
                {
                    string s = wrang.GetString();
                    if (s != null && s.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                    {
                        Log.Info("Set enabled");
                        this.Enabled = true;
                        return;
                    }
                }
            }

            Log.Info("Set disabled");
            this.Enabled = false;
        }

        /// <summary>
        /// Process any lists and return the full list of variables
        /// </summary>
        /// <returns></returns>
        public async Task<List<Variable>> ProcessAsync()
        {
            List<Variable> list = new List<Variable>();
            list.AddRange(this._variables); 

            foreach (var setlist in this.SetLists)
            {
                var processed = await setlist.ProcessAsync();
                list.AddRange(processed);
            }

            return list;
        }
    }
}
