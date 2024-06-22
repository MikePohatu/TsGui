#region license
// Copyright (c) 2020 Mike Pohatu
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

// Group.cs - groups of elements to be enabled and disabled by a toggle

using System.Collections.Generic;
using Core.Logging;

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
                    Log.Info("Group " + this.ID + " state changed. New state: " + this._state.ToString());
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
