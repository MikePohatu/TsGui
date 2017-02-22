using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
