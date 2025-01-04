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

// OptionLibrary.cs - class for the MainController to keep track of all the IGuiOption in 
// the app

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TsGui.Linking;
using TsGui.View.GuiOptions;

namespace TsGui.Options
{
    public static class OptionLibrary
    {
        public static event OptionLibraryOptionAdded OptionAdded;
        public static ObservableCollection<IOption> Options { get; } = new ObservableCollection<IOption>();
        public static List<IValidationGuiOption> ValidationOptions { get; } = new List<IValidationGuiOption>();

        public static void Add(IOption option)
        {
            Options.Add(option);
            IValidationGuiOption valop = option as IValidationGuiOption;
            if (valop != null) { ValidationOptions.Add(valop); }

            if (string.IsNullOrWhiteSpace(option.ID) == false) 
            { LinkingHub.Instance.AddSource(option); }

            OptionAdded?.Invoke(option, new EventArgs());
        }

        public static async Task InitialiseOptionsAsync()
        {
            foreach (IOption option in Options)
            {
                await option.InitialiseAsync();
            }
        }

        /// <summary>
        /// Clear the loaded options
        /// </summary>
        public static void Reset()
        {
            Options.Clear();
            ValidationOptions.Clear();
        }
    }
}
