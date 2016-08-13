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
using System.Diagnostics;
using System;

namespace TsGui
{
    internal static class GroupingLogic
    {
        public static void OnGroupDisplay(IGroupable Element, bool Display)
        {
            if (Display == true)
            {
                Element.DisplayedGroupsCount++;
                //Enable and unhide the element unless the parent elements contradict 
                if (Element.HiddenParentCount == 0)
                { Element.IsHidden = false; }

                if (Element.DisabledParentCount == 0)
                { Element.IsEnabled = true; }
            }
            else
            {
                if (Element.DisplayedGroupsCount > 0) { Element.DisplayedGroupsCount--; }
                if (Element.DisplayedGroupsCount == 0) { Element.IsHidden = true; }
            }
        }

        public static void OnGroupEnable(IGroupable Element, bool Enable)
        {
            if (Enable == true)
            {
                Element.EnabledGroupsCount++;
                Element.DisplayedGroupsCount++;
                //Enable and unhide the element unless the parent elements contradict 
                if (Element.HiddenParentCount == 0)
                { Element.IsHidden = false; }

                if (Element.DisabledParentCount == 0)
                { Element.IsEnabled = true; }                
            }
            else
            {
                if (Element.EnabledGroupsCount > 0) { Element.EnabledGroupsCount--; }
                if (Element.EnabledGroupsCount == 0)
                {
                    Element.IsEnabled = false;
                    //If the remaining groups are set to hide, hide the element
                    if (Element.DisplayedGroupsCount == 0) { Element.IsHidden = true; }
                }
            }
        }

        public static void OnParentHide(IGroupChild Element, bool Hide)
        {
            if (Hide == true)
            {                
                Element.HiddenParentCount++;
                Element.IsHidden = true;
            }
            else
            {
                if (Element.HiddenParentCount > 0) { Element.HiddenParentCount--; }  
                    
                if (Element.HiddenParentCount == 0 )
                {
                    if (Math.Abs(Element.DisplayedGroupsCount) > 0)
                    { Element.IsHidden = false; }
                }
            }
        }

        public static void OnParentEnable(IGroupChild Element, bool Enable)
        {
            if (Enable == true)
            {
                if (Element.DisabledParentCount > 0) { Element.DisabledParentCount--; }

                if (Element.DisabledParentCount == 0 )
                {
                    if (Math.Abs(Element.DisplayedGroupsCount) > 0)
                    { Element.IsHidden = false; }

                    if (Math.Abs(Element.EnabledGroupsCount) > 0)
                    { Element.IsEnabled = true; }
                }
            }
            else
            {
                Element.DisabledParentCount++;
                Element.IsEnabled = false;
            }
        }
    }
}
