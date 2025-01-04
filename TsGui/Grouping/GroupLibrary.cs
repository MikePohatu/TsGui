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

// GroupLibrary.cs - class to store groups

using System.Collections.Generic;


namespace TsGui.Grouping
{
    public static class GroupLibrary
    {
        private static Dictionary<string, Group> _groups = new Dictionary<string, Group>();

        /// <summary>
        /// Default value of PurgeInactive for new GroupableBase objects
        /// </summary>
        public static bool PurgeInactive { get; set; } = false;

        private static Group CreateGroup(string ID)
        {
            Group group;
            group = new Group(ID);
            _groups.Add(ID, group);
            return group;
        }

        /// <summary>
        /// Add a groupable element to a group
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Element"></param>
        /// <returns></returns>
        public static Group AddToGroup(string ID, GroupableBase Element)
        {
            Group group = GetGroupFromID(ID);
            group.Add(Element);

            return group;
        }

        /// <summary>
        /// Get a group object, create if doesn't exist
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Group GetGroupFromID(string ID)
        {
            Group group;
            if (_groups.TryGetValue(ID, out group) == false)
            {
                group = CreateGroup(ID);
            }

            return group;
        }

        /// <summary>
        /// Clear the loaded groups
        /// </summary>
        public static void Reset()
        {
            _groups.Clear();
        }

        public static void Init()
        {
            foreach (var group in _groups.Values)
            {
                group.Init();
            }
        }
    }
}
