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

// GroupDelegates.cs - Delegates for the Grouping namespace

namespace TsGui.Grouping
{

    public enum GroupState { Hidden, Disabled, Enabled };
    public delegate void ToggleEvent();

    /// <summary>
    /// Event for notifying sub controls that the parents visibility has changed.
    /// </summary>
    /// <param name="Parent"></param>
    /// <param name="IsEnabled"></param>
    /// <param name="IsHidden"></param>
    //public delegate void ParentToggleEvent(IGroupParent Parent, bool IsEnabled, bool IsHidden);
    public delegate void GroupStateChange();

    public delegate void GroupableHide(bool Hide);
    public delegate void GroupableEnable(bool Enable);
}
