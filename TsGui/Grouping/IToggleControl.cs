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

// IToggleControl.cs - defines TsGuiElements that can trigger a toggle object

namespace TsGui
{
    public interface IToggleControl
    {
        string VariableName { get; }
        string CurrentValue { get; }
        bool IsActive { get; }
        //void AttachToggle(Toggle Toggle);
        event ToggleEvent ToggleEvent;
        void InitialiseToggle();
    }
}
