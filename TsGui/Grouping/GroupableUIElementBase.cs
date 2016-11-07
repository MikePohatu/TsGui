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
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Grouping
{
    public abstract class GroupableUIElementBase: IGroupableUIElement, IGroupChild, INotifyPropertyChanged
    {
        protected MainController _controller;
        protected bool _isenabled = true;
        protected bool _ishidden = false;
        private List<Group> _groups = new List<Group>();
        private Visibility _visibility = Visibility.Visible;
        private bool _purgeinactive = false;
        private GroupableUIElementBase _parent;

        public List<Group> Groups { get { return this._groups; } }

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
        public bool PurgeInactive
        {
            get { return this._purgeinactive; }
            set { this._purgeinactive = value; }
        }

        //Constructor
        public GroupableUIElementBase(MainController MainController)
        {
            this._controller = MainController;
        }

        public GroupableUIElementBase(GroupableUIElementBase Parent, MainController MainController)
        {
            this._controller = MainController;

            //register grouping events from the parent element
            this._parent = Parent;
            this._parent.GroupingStateChange += this.OnParentGoupingStateChange;
        }
        //Events
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;
        public event GrouableStateChange GroupingStateChange;

        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }

        public void OnParentGoupingStateChange(object o, GroupingEventArgs e)
        {
            this.EvaluateGroups();
        }

        public void OnGroupStateChange()
        {
            this.EvaluateGroups();
        }
        #endregion

        protected void EvaluateGroups()
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
