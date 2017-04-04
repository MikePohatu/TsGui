//    Copyright (C) 2017 Mike Pohatu

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

// LinkableLibrary.cs - stores GuiOptions against their ID

using System.Collections.Generic;
using System;

using TsGui.Options;

namespace TsGui.Linking
{
    public class LinkingLibrary
    {
        private Dictionary<string, IOption> _sources = new Dictionary<string, IOption>();
        private Dictionary<string, List<IOption>> _pendingtargets = new Dictionary<string, List<IOption>>();

        public IOption GetOption(string ID)
        {
            IOption option;
            this._sources.TryGetValue(ID, out option);
            return option;
        }

        public void AddTarget(string ID, IOption NewTarget)
        {
            IOption source;
            if (this._sources.TryGetValue(ID, out source) == true)
            {
                this.RegisterTargetToSource(source, NewTarget);
            }
            else
            {
                List<IOption> pendinglist;
                if (this._pendingtargets.TryGetValue(ID, out pendinglist) == true)
                { pendinglist.Add(NewTarget); }
                else
                {
                    pendinglist = new List<IOption>();
                    pendinglist.Add(NewTarget);
                    this._pendingtargets.Add(ID, pendinglist);
                }
            }
        }

        public void AddSource(IOption NewSource)
        {
            List<IOption> pendingtargetslist;
            IOption testoption;

            if (this._sources.TryGetValue(NewSource.ID, out testoption) == true ) { throw new InvalidOperationException("Duplicate ID found in LinkableLibrary: " + NewSource.ID); }
            else { this._sources.Add(NewSource.ID,NewSource); }

            //now register any pending targets and cleanup
            if (this._pendingtargets.TryGetValue(NewSource.ID, out pendingtargetslist) == true)
            {
                foreach (IOption target in pendingtargetslist)
                {
                    this.RegisterTargetToSource(NewSource, target);
                }
                this._pendingtargets.Remove(NewSource.ID);
            }
        }

        private void RegisterTargetToSource(IOption Source, IOption Target)
        {

        }
    }
}
