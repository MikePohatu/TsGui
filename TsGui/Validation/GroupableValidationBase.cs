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

// GroupableValidationBase - base class for Compliance and StringValidation classes

using System.Collections.Generic;

using TsGui.Grouping;

namespace TsGui.Validation
{
    public abstract class GroupableValidationBase
    {
        protected List<Group> _groups = new List<Group>();

        public bool IsActive { get; set; }

        //Grouping 
        public GroupStateChange RevalidationRequired;
        public void OnGroupStateChange()
        {
            bool result = false;
            foreach (Group g in this._groups)
            { if (g.State == GroupState.Enabled) { result = true; break; } }

            if (this.IsActive != result)
            {
                this.IsActive = result;
                this.RevalidationRequired?.Invoke();
            }
        }
    }
}
