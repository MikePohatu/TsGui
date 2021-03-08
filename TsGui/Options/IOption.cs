#region license
// Copyright (c) 2020 Mike Pohatu
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

using TsGui.Linking;

namespace TsGui.Options
{
    public interface IOption: ILinkSource
    {
        TsVariable Variable { get; }
        string LiveValue { get; }
        string VariableName { get; }
        string InactiveValue { get; }
        bool PurgeInactive { get; set; }
        bool IsActive { get; }

        /// <summary>
        /// The path property can be set by other output methods e.g. registry key. It is not used for Task Sequence output
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Initialise the option. This is run after the Director has finished loading config, so all options 
        /// will be in place. This is where the first updates of values should be run
        /// </summary>
        void Initialise();
    }
}
