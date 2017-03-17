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

// GroupableBlindBase.cs - base class for grouable blind/noui objects
using System.ComponentModel;

namespace TsGui.Grouping
{
    public abstract class GroupableBlindBase: GroupableBase, INotifyPropertyChanged
    {
        protected GroupableBlindBase _parent;

        public bool IsActive
        {
            get { return this._isactive; }
            set
            {
                this._isactive = value;
                this.GroupingStateChange?.Invoke(this, new GroupingEventArgs(GroupStateChanged.IsEnabled));
            }
        }

        //Constructor
        public GroupableBlindBase(MainController MainController) : base(MainController) { }
        public GroupableBlindBase(GroupableBlindBase Parent, MainController MainController): base(Parent, MainController)
        {
            this._parent = Parent;
            this._parent.GroupingStateChange += this.OnParentGroupingStateChange;
        }

        //Events
        public override event GroupableStateChange GroupingStateChange;

        protected override void EvaluateGroups()
        {
            if (this._parent != null)
            {
                if (this._parent.IsActive == false) { this.IsActive = false; return; }
            }

            if (this._groups.Count == 0) { this.IsActive = true; return; }

            foreach (Group g in this._groups)
            {
                if (g.State == GroupState.Enabled) { this.IsActive = true; return; }
            }

            this.IsActive = false;
        }
    }
}
