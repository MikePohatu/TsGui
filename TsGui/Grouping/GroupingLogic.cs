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

// GroupingLogic.cs - handles the logic for dealing with grouping operations for 
// IGroupAbleElement objects

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsGui
{
    internal static class GroupingLogic
    {
        //public event ParentToggleEvent ParentChanged;
        ////Only subscribed if member of a group. Registers changes to parent elements. 
        //public  static void OnParentChanged(IGroupable GroupElement, IGroupParent p, bool IsEnabled, bool IsHidden)
        //{
        //    //Debug.WriteLine("    TsColumn: OnParentChanged called: IsEnabled, IsHidden:" + IsEnabled + IsHidden);

        //    if ((IsHidden == true) || (IsEnabled == false))
        //    {
        //        GroupElement.IsEnabled = IsEnabled;
        //        GroupElement.IsHidden = IsHidden;
        //    }
        //    else if (GroupElement._group != null)
        //    {
        //        GroupElement.IsHidden = GroupElement._group.IsHidden;
        //        GroupElement.IsEnabled = GroupElement._group.IsEnabled;
        //    }
        //    else
        //    {
        //        GroupElement.IsHidden = false;
        //        GroupElement.IsEnabled = true;
        //    }
        //    //raise new event for child controls
        //    GroupElement.ParentChanged?.Invoke(GroupElement, IsEnabled, IsHidden);
        //}
        public static void OnGroupHide(IGroupable GroupElement, bool Hide)
        {
            if (Hide == true)
            {
                if (GroupElement.ActiveGroupsCount > 0)
                { GroupElement.ActiveGroupsCount--; }

                if (GroupElement.ActiveGroupsCount == 0) { GroupElement.IsHidden = true; }
            }
            else
            {
                GroupElement.ActiveGroupsCount++;
                GroupElement.IsHidden = false;
            }
        }

        public static void OnGroupEnable(IGroupable GroupElement, bool Enable)
        {
            if (Enable == true)
            {
                GroupElement.ActiveGroupsCount++;
                GroupElement.IsEnabled = true;
            }
            else
            {
                if (GroupElement.ActiveGroupsCount > 0)
                { GroupElement.ActiveGroupsCount--; }

                if (GroupElement.ActiveGroupsCount == 0) { GroupElement.IsEnabled = false; }
            }
        }
    }
}
