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

// IGroupableUIElement.cs - interface defines elements that can be added to a Group object
using System.Collections.Generic;
using System.ComponentModel;

namespace TsGui.Grouping
{
    public interface IGroupableUIElement
    {
        bool IsActive { get; }
        bool IsEnabled { get; set; }
        bool IsHidden { get; set; }
        List<Group> Groups { get; }

        void OnGroupStateChange();
        event PropertyChangedEventHandler PropertyChanged;
    }
}
