#region license
// Copyright (c) 2020 Mike Pohatu
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;
using TsGui.Validation;

namespace TsGui.Sets
{
    public class Set: GroupableBlindBase, ILinkTarget
    {
        public List<Variable> Variables { get; } = new List<Variable>();
        private QueryPriorityList _enabledQueries;

        public bool Enabled { get; private set; } = true;

        public Set(XElement inputXml): base()
        {
            
            this.LoadXml(inputXml);
        }

        private new void LoadXml(XElement inputXml)
        {
            base.LoadXml(inputXml);
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
                    this.Variables.Add(new Variable(name, value, Director.Instance.DefaultPath));
                }
                else
                {
                    Log.Error("Set contains variable without name");
                }

            }
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
    }
}
