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
using TsGui.Diagnostics.Logging;

namespace TsGui.Grouping
{
    public class Group
    {
        public event GroupStateChange StateEvent;
        
        private List<GroupableBase> _groupables;
        private GroupState _state;

        //properties
        #region
        public GroupState State
        {
            get { return this._state; }
            set
            {
                if (this._state != value)
                {
                    this._state = value;
                    StateEvent?.Invoke();
                    LoggerFacade.Info("Group " + this.ID + "state changed. New state: " + this._state.ToString());
                }              
            }
        }
        public bool PurgeInactive { get; set; }
        public string ID { get; set; }
        public int Count { get { return this._groupables.Count; } }
        #endregion

        //constructor
        public Group (string ID)
        {
            this._groupables = new List<GroupableBase>();
            this.ID = ID;
            this.State = GroupState.Enabled;
            this.PurgeInactive = false;
        }

        //method
        public void Add(GroupableBase GroupableElement)
        {
            this._groupables.Add(GroupableElement);
            this.StateEvent += GroupableElement.OnGroupStateChange;
        }
    }
}
