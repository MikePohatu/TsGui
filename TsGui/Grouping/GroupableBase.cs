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

using System.ComponentModel;
using System.Collections.Generic;

namespace TsGui.Grouping
{
    public abstract class GroupableBase: IGroupable, INotifyPropertyChanged
    {
        protected bool _enabled = true;
        protected bool _hidden = false;
        protected List<Group> _groups = new List<Group>();


        public int GroupCount { get { return this._groups.Count; } }
        public int DisabledParentCount { get; set; }
        public int HiddenParentCount { get; set; }

        public List<Group> Groups { get { return this._groups; } }
        public bool IsEnabled
        {
            get { return this._enabled; }
            set {this._enabled = value; this.OnPropertyChanged(this, "IsEnabled"); }
        }
        public bool IsHidden
        {
            get { return this._hidden; }
            set { this._hidden = value; this.OnPropertyChanged(this, "IsHidden"); }
        }
        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false))
                { return true; }
                else { return false; }
            }
        }

        //Events
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }

        //public event ParentHide ParentHide;
        //public event ParentEnable ParentEnable;
        #endregion


        public void OnGroupStateChange()
        {
            GroupingLogic.EvaluateGroups(this);
        }
    }
}
