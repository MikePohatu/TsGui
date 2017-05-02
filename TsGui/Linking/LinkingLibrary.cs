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
using TsGui.Diagnostics;

namespace TsGui.Linking
{
    public class LinkingLibrary
    {
        private Dictionary<string, ILinkSource> _sources = new Dictionary<string, ILinkSource>();
        private Dictionary<string, List<ILinkingEventHandler>> _pendingqueries = new Dictionary<string, List<ILinkingEventHandler>>();

        public ILinkSource GetSourceOption(string ID)
        {
            ILinkSource option;
            this._sources.TryGetValue(ID, out option);
            return option;
        }

        public void AddHandler(string ID, ILinkingEventHandler newhandler)
        {
            ILinkSource source;
            if (this._sources.TryGetValue(ID, out source) == true)
            {
                this.RegisterHandlerToSource(source, newhandler);
            }
            else
            {
                List<ILinkingEventHandler> pendinglist;
                if (this._pendingqueries.TryGetValue(ID, out pendinglist) == true)
                { pendinglist.Add(newhandler); }
                else
                {
                    pendinglist = new List<ILinkingEventHandler>();
                    pendinglist.Add(newhandler);
                    this._pendingqueries.Add(ID, pendinglist);
                }
            }
        }

        public void AddSource(ILinkSource NewSource)
        {
            List<ILinkingEventHandler> pendinglist;
            ILinkSource testoption;

            if (this._sources.TryGetValue(NewSource.ID, out testoption) == true ) { throw new TsGuiKnownException("Duplicate ID found in LinkableLibrary: " + NewSource.ID,""); }
            else { this._sources.Add(NewSource.ID,NewSource); }

            //now register any pending targets and cleanup
            if (this._pendingqueries.TryGetValue(NewSource.ID, out pendinglist) == true)
            {
                foreach (ILinkingEventHandler handler in pendinglist)
                {
                    this.RegisterHandlerToSource(NewSource, handler);
                }
                this._pendingqueries.Remove(NewSource.ID);
            }
        }

        private void RegisterHandlerToSource(ILinkSource Source, ILinkingEventHandler Handler)
        {
            Source.ValueChanged += Handler.OnLinkedSourceValueChanged;
        }
    }
}
