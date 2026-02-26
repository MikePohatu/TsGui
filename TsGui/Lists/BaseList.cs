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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TsGui.Lists
{
    public abstract class BaseList
    {
        protected string _prefix;
        protected int _countLength = 2;
        protected string _path;
        protected IConfigParent _parent;

        public string ID { get; protected set; }

        public BaseList(IConfigParent parent)
        {
            this.UpdateParent(parent);
        }

        public void LoadXml(XElement inputxml)
        {
            this._prefix = XmlHandler.GetStringFromXml(inputxml, "Prefix", this._prefix);
            this._countLength = XmlHandler.GetIntFromXml(inputxml, "CountLength", _countLength);
            this._path = XmlHandler.GetStringFromXml(inputxml, "Path", this._path);
            this.ID = XmlHandler.GetStringFromXml(inputxml, "ID", this.ID);
        }

        /// <summary>
        /// Only to be called from ListLibrary. Update the Parent for the list. 
        /// </summary>
        /// <param name="parent"></param>
        public void UpdateParent(IConfigParent parent)
        {
            this._parent = parent;
            this._path = parent?.Path != null ? parent.Path : Director.Instance.DefaultPath;
        }

        public abstract Task<List<Variable>> ProcessAsync();
    }
}
