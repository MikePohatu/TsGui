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

// GroupingEventArgs.cs - EventArgs to be used with grouping events

using System;

namespace TsGui.Grouping
{
    public class GroupingEventArgs: EventArgs
    {
        public GroupStateChanged GroupStateChanged { get; set; }

        public GroupingEventArgs() { }
        public GroupingEventArgs(GroupStateChanged StateChanged) { this.GroupStateChanged = StateChanged; }
    }
}
