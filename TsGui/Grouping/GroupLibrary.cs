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

// GroupLibrary.cs - class to store groups

using System.Collections.Generic;


namespace TsGui.Grouping
{
    public class GroupLibrary
    {

        private Dictionary<string, Group> _groups = new Dictionary<string, Group>();

        private Group CreateGroup(string ID)
        {
            Group group;
            group = new Group(ID);
            this._groups.Add(ID, group);
            return group;
        }

        /// <summary>
        /// Add a groupable element to a group
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Element"></param>
        /// <returns></returns>
        public Group AddToGroup(string ID, GroupableBase Element)
        {
            Group group = this.GetGroupFromID(ID);
            group.Add(Element);

            return group;
        }

        /// <summary>
        /// Get a group object, create if doesn't exist
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Group GetGroupFromID(string ID)
        {
            Group group;
            this._groups.TryGetValue(ID, out group);
            if (group == null) { group = this.CreateGroup(ID); }

            return group;
        }
    }
}
