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
