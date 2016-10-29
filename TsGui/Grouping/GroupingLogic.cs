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


namespace TsGui.Grouping
{
    internal static class GroupingLogic
    {
        //group logic
        public static void OnGroupUnhide(IGroupableUIElement Element, bool Display)
        {
            GroupingLogic.EvaluateGroups(Element);
        }

        public static void OnGroupEnable(IGroupableUIElement Element, bool Enable)
        {
            GroupingLogic.EvaluateGroups(Element);
        }


        //Parent/child logic
        public static void OnParentHide(IGroupChild Element, bool Hide)
        {
            if (Hide == true)
            {                
                Element.HiddenParentCount++;
            }
            else
            {               
                if (Element.HiddenParentCount > 0) { Element.HiddenParentCount--; }  
            }
            EvaluateGroups(Element);
        }

        public static void OnParentEnable(IGroupChild Element, bool Enable)
        {
            if (Enable == true)
            {
                if (Element.DisabledParentCount > 0) { Element.DisabledParentCount--; }
            }
            else
            {
                Element.DisabledParentCount++;
            }
            EvaluateGroups(Element);
        }

        public static void EvaluateGroups(IGroupableUIElement Element)
        {       
            bool hiddenset = false;
            bool enabledset = false;
            GroupState state = GroupState.Disabled;

            if (Element.DisabledParentCount > 0)
            {
                if (Element.IsEnabled == true) { Element.IsEnabled = false; }
                enabledset = true;
            }
            if (Element.HiddenParentCount > 0)
            {
                if (Element.IsHidden == false) { Element.IsHidden = true; }
                hiddenset = true;
            }

            if (hiddenset && enabledset) { return; } //done

            //Parents checked, now check groups
            if (Element.GroupCount > 0)
            {
                foreach (Group g in Element.Groups)
                {
                    if (g.State == GroupState.Enabled)
                    {
                        state = GroupState.Enabled;
                        break;
                    }

                    if (g.State == GroupState.Hidden) { state = GroupState.Hidden; }
                }
            }
            else
            {
                state = GroupState.Enabled;
            }

            //now set things based on what is there
            if (state == GroupState.Enabled)
            {
                if (!hiddenset) { Element.IsHidden = false; }
                if (!enabledset) { Element.IsEnabled = true; }
            }
            else if (state == GroupState.Disabled)
            {
                if (!hiddenset) { Element.IsHidden = false; }
                if (!enabledset) { Element.IsEnabled = false; }
            }
            else
            {
                if (!hiddenset) { Element.IsHidden = true; }
                if (!enabledset) { Element.IsEnabled = false; }
            }  
        }
    }
}
