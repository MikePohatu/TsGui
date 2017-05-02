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

using TsGui.View;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Grouping
{
    public abstract class GroupableBase: ViewModelBase
    {
        protected IDirector _director;
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
        public GroupableBase(IDirector MainController)
        {
            this._director = MainController;
        }

        public GroupableBase(GroupableBase Parent, IDirector MainController)
        {
            this._director = MainController;
            this._purgeinactive = Parent.PurgeInactive;
        }

        //Events
        #region
        public abstract event GroupableStateChange GroupingStateChange;

        public void OnParentGroupingStateChange(object o, GroupingEventArgs e)
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
                { this.AddGroup(xGroup.Value); }
            }
        }

        protected virtual Group AddGroup(string GroupID)
        {
            Group g = this._director.GroupLibrary.AddToGroup(GroupID, this);
            this._groups.Add(g);
            return g;
        }
    }
}
