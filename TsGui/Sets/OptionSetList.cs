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
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Options;

namespace TsGui.Sets
{
    public class OptionSetList: ISetList, ILinkTarget
    {
        private List<IOption> _linkSources = new List<IOption>();
        private List<string> _linkIDs = new List<string>();
        private string _prefix;
        private int _countLength;
        private Set _parent;

        public OptionSetList(XElement inputXml, Set Parent)
        {
            this._parent = Parent;
            this._prefix = XmlHandler.GetStringFromXml(inputXml, "Prefix", null);
            this._countLength = XmlHandler.GetIntFromXml(inputXml, "CountLength", 2);
        }

        public Task OnSourceValueUpdatedAsync(Message updateMessage)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Variable>> ProcessAsync()
        {
            await Task.CompletedTask;

            return new List<Variable>();
        }
    }
}
