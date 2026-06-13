#region license
// Copyright (c) 2026 Mike Pohatu
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

// IOption.cs - option interface. Base interface to apply to both GuiOptions and BlindOptions

using System.Collections.Generic;
using System.Threading.Tasks;
using TsGui.Linking;
using TsGui.Lists;

namespace TsGui.Options
{
    public interface IOption: ILinkSource
    {
        IEnumerable<Variable> GetVariables();
        string LiveValue { get; }

        /// <summary>
        /// The value that will go to the Lists feature, defaults to the VariableName unless set in XML
        /// </summary>
        string ListsOutput { get; }
        string VariableName { get; }
        string InactiveValue { get; }
        /// <summary>
        /// whether the variable will be marked as hidden in logs by ConfigMgr. Creates %Variable%_HiddenValueFlag% variable prior to the variable
        /// </summary>
        bool HiddenValueFlag { get; }
        bool PurgeInactive { get; set; }
        bool IsActive { get; }
        string Lists { get; }
        /// <summary>
        /// The path property can be set by other output methods e.g. registry key. It is not used for Task Sequence output
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Initialise the option. This is run after the Director has finished loading config, so all options 
        /// will be in place. This is where the first updates of values should be run
        /// </summary>
        Task InitialiseAsync();
    }
}
