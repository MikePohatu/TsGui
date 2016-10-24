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

// IGroupable.cs - interface defines elements that can be added to a Group object
using System.Collections.Generic;
using System.ComponentModel;

namespace TsGui.Grouping
{
    public interface IGroupable
    {
        bool IsActive { get; }
        bool IsEnabled { get; set; }
        bool IsHidden { get; set; }
        List<Group> Groups { get; }
        int DisabledParentCount { get; set; }
        int HiddenParentCount { get; set; }
        int GroupCount { get; }

        void OnGroupStateChange();
        event PropertyChangedEventHandler PropertyChanged;
        //void OnGroupDisplay(bool Display);
        //void OnGroupEnable(bool Enable);
    }
}
