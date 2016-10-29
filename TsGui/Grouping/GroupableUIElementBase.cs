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

        public int GroupCount { get { return this._groups.Count; } }
        public int DisabledParentCount { get; set; }
        public int HiddenParentCount { get; set; }

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
                this._isenabled = value;
                this.OnPropertyChanged(this, "IsEnabled");
                this.GroupableEnable?.Invoke(value);
            }
        }
        public bool IsHidden
        {
            get { return this._ishidden; }
            set
            {
                this.HideUnhide(value);
                this.OnPropertyChanged(this, "IsHidden");
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

        //Events
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;
        public event GroupableHide GroupableHide;
        public event GroupableEnable GroupableEnable;

        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }

        public void OnParentHide(bool Hide)
        {
            GroupingLogic.OnParentHide(this, Hide);
        }

        public void OnParentEnable(bool Enable)
        {
            GroupingLogic.OnParentEnable(this, Enable);
        }
        #endregion


        public void OnGroupStateChange()
        {
            GroupingLogic.EvaluateGroups(this);
        }

        protected void HideUnhide(bool Hidden)
        {
            this._ishidden = Hidden;
            if (Hidden == true)
            { this.Visibility = Visibility.Collapsed; }
            else
            { this.Visibility = Visibility.Visible; }

            this.GroupableHide?.Invoke(Hidden);
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
