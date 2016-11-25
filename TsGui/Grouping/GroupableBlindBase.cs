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

namespace TsGui.Grouping
{
    public class GroupableBlindBase: GroupableBase
    {
        //Constructor
        public GroupableBlindBase(MainController MainController) : base(MainController) { }
        public GroupableBlindBase(GroupableBlindBase Parent, MainController MainController): base(Parent, MainController)
        {
            Parent.GroupingStateChange += this.OnParentGoupingStateChange;
            this._parent = Parent;
        }

        //Events
        #region
        //Setup the INotifyPropertyChanged interface 
        public event GrouableStateChange GroupingStateChange;
        #endregion

        protected override void EvaluateGroups()
        {
            if (this._parent != null)
            {
                if (this._parent.IsActive == false)
                { this._isactive = false; return; }
            }

            foreach (Group g in this._groups)
            {
                if (g.State == GroupState.Disabled) { this._isactive = false; return; }
            }

            this._isactive = true;
        }
    }
}
