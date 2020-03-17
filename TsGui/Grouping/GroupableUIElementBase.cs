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

// GroupableUIElementBase.cs - base class for grouable UI objects

using System.Windows;

namespace TsGui.Grouping
{
    public abstract class GroupableUIElementBase: GroupableBase, IGroupableUIElement, IGroupChild
    {
        protected bool _isenabled = true;
        protected bool _ishidden = false;
        private Visibility _visibility = Visibility.Visible;
        private GroupableUIElementBase _parent;

        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false)) { return true; }
                else { return false; }
            }
        }
        public bool IsEnabled
        {
            get { return this._isenabled; }
            set
            {
                if (value != this._isenabled)
                {
                    this._isenabled = value;
                    this.GroupingStateChange?.Invoke(this,new GroupingEventArgs(GroupStateChanged.IsEnabled));
                    this.OnPropertyChanged(this, "IsEnabled");
                }              
            }
        }
        public bool IsHidden
        {
            get { return this._ishidden; }
            set
            {
                if (value != this._ishidden)
                {
                    this.HideUnhide(value);
                    this.GroupingStateChange?.Invoke(this, new GroupingEventArgs(GroupStateChanged.IsHidden));
                    this.OnPropertyChanged(this, "IsHidden");
                }            
            }
        }
        public Visibility Visibility
        {
            get { return this._visibility; }
            set { this._visibility = value; this.OnPropertyChanged(this, "Visibility"); }
        }

        //Constructor
        public GroupableUIElementBase() : base() { }
        public GroupableUIElementBase(GroupableUIElementBase Parent): base(Parent)
        {
            this._parent = Parent;
            this._parent.GroupingStateChange += this.OnParentGroupingStateChange;
        }

        //Events
        public override event GroupableStateChange GroupingStateChange;

        //methods
        protected override void EvaluateGroups()
        {
            GroupState groupsstate = GroupState.Hidden;
            GroupState parentstate = GroupState.Enabled;
            if (this._parent != null)
            {
                 if (this._parent.IsHidden == true) { this.ChangeState(GroupState.Hidden); return; }
                 if (this._parent.IsEnabled == false) { parentstate = GroupState.Disabled; }
            }

            if (this._groups.Count == 0) { groupsstate = GroupState.Enabled; }

            foreach (Group g in this._groups)
            {
                if (g.State == GroupState.Enabled) { groupsstate = GroupState.Enabled; break; }
                else if (g.State == GroupState.Disabled) { groupsstate = GroupState.Disabled; }
            }

            if (groupsstate == GroupState.Hidden) { this.ChangeState(GroupState.Hidden); }
            else if (parentstate == GroupState.Disabled) { this.ChangeState(GroupState.Disabled); }
            else { this.ChangeState(groupsstate); }
        }

        private void ChangeState(GroupState State)
        {
            switch (State)
            {
                case GroupState.Disabled:
                    this.IsHidden = false;
                    this.IsEnabled = false;
                    break;
                case GroupState.Enabled:
                    this.IsHidden = false;
                    this.IsEnabled = true;
                    break;
                case GroupState.Hidden:
                    this.IsHidden = true;
                    this.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        protected void HideUnhide(bool Hidden)
        {
            this._ishidden = Hidden;
            if (Hidden == true)
            { this.Visibility = Visibility.Collapsed; }
            else
            { this.Visibility = Visibility.Visible; }
        }
    }
}
