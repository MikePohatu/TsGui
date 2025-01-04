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
        public GroupableBlindBase() : base() { }
        public GroupableBlindBase(GroupableBlindBase Parent): base(Parent)
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
