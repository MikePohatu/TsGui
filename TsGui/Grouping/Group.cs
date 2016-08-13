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

// Group.cs - groups of elements to be enabled and disabled by a toggle

using System.Collections.Generic;
using System;
//using System.Diagnostics;

namespace TsGui
{
    public class Group
    {
        //public event GroupToggleEvent GroupToggleEvent;
        public event GroupDisplay DisplayEvent;
        public event GroupEnable EnableEvent;

        private List<IGroupable> _elements;
        private bool _isEnabled;
        private bool _isHidden;

        //properties
        #region
        public bool PurgeInactive { get; set; }
        public string ID { get; set; }
        public int Count { get { return this._elements.Count; } }
        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set
            {
                if (this._isEnabled != value)
                {
                    this._isEnabled = value;
                    EnableEvent?.Invoke(value);
                }
            }
        }
        public bool IsHidden
        {
            get { return this._isHidden; }
            set
            {
                if (this._isHidden != value )
                {
                    this._isHidden = value;
                    DisplayEvent?.Invoke(!value); 
                }
            }
        }
        #endregion

        //constructor
        public Group (string ID)
        {
            this._elements = new List<IGroupable>();
            this.ID = ID;
            this.IsEnabled = true;
            this.IsHidden = false;
            this.PurgeInactive = false;
        }


        //method
        public void Add(IGroupable GroupableElement)
        {
            this._elements.Add(GroupableElement);
            this.DisplayEvent += GroupableElement.OnGroupDisplay;
            this.EnableEvent += GroupableElement.OnGroupEnable;

            //if this is the first group the element has been added to, re-initialise it
            if (GroupableElement.EnabledGroupsCount == 999) { GroupableElement.EnabledGroupsCount = 0; }
            if (GroupableElement.DisplayedGroupsCount == 999) { GroupableElement.DisplayedGroupsCount = 0; }

            if (this.IsEnabled == true) { GroupableElement.EnabledGroupsCount++; }
            if (this.IsHidden == false) { GroupableElement.DisplayedGroupsCount++; }

        }

        public void Remove(IGroupable GroupableElement)
        {
            this._elements.Remove(GroupableElement);
            this.DisplayEvent -= GroupableElement.OnGroupDisplay;
            this.EnableEvent -= GroupableElement.OnGroupEnable;

            if (this.IsEnabled == true)
            {
                if (GroupableElement.EnabledGroupsCount > 0) { GroupableElement.EnabledGroupsCount--; }
            }
            if (this.IsHidden == false)
            {
                if (GroupableElement.DisplayedGroupsCount > 0) { GroupableElement.DisplayedGroupsCount--; }
            }
        }
    }
}
