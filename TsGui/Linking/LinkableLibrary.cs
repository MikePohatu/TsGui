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
using System;

namespace TsGui.Linking
{
    public class LinkableLibrary
    {
        private Dictionary<string, IOption> _options = new Dictionary<string, IOption>();

        public IOption GetOption(string ID)
        {
            IOption option;
            this._options.TryGetValue(ID, out option);
            return option;
        }

        public void AddOption(IOption Option)
        {
            if (Option == null) { throw new ArgumentNullException("Null Option cannot be added to LinkableLibrary"); }
            IOption option;
            this._options.TryGetValue(Option.ID, out option);

            if (option == null) { this._options.Add(Option.ID, Option); }
            else { throw new TsGuiKnownException("Option ID specified more than once. ID: " + Option.ID,"LinkableLibrary IDs set more than once"); }
        }
    }
}
