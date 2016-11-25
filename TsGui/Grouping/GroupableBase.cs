//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// GroupableBase.cs - base class for grouable objects

using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Grouping
{
    public abstract class GroupableBase
    {
        protected MainController _controller;
        protected bool _isactive = true;
        protected List<Group> _groups = new List<Group>();
        protected bool _purgeinactive = false;

        public List<Group> Groups { get { return this._groups; } }
        public bool PurgeInactive
        {
            get { return this._purgeinactive; }
            set { this._purgeinactive = value; }
        }

        //Constructor
        public GroupableBase(MainController MainController)
        {
            this._controller = MainController;
        }

        public GroupableBase(GroupableBase Parent, MainController MainController)
        {
            this._controller = MainController;
            this._purgeinactive = Parent.PurgeInactive;
        }

        //Events
        #region
        public abstract event GrouableStateChange GroupingStateChange;

        public void OnParentGoupingStateChange(object o, GroupingEventArgs e)
        {
            this.EvaluateGroups();
        }

        public void OnGroupStateChange()
        {
            this.EvaluateGroups();
        }
        #endregion

        protected abstract void EvaluateGroups();

        protected void LoadXml(XElement InputXml)
        {
            this.PurgeInactive = XmlHandler.GetBoolFromXAttribute(InputXml, "PurgeInactive", this.PurgeInactive);

            IEnumerable<XElement> xGroups = InputXml.Elements("Group");
            if (xGroups != null)
            {
                foreach (XElement xGroup in xGroups)
                { this._groups.Add(this._controller.AddToGroup(xGroup.Value, this)); }
            }
        }
    }
}
