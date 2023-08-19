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

// OptionLibrary.cs - class for the MainController to keep track of all the IGuiOption in 
// the app

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TsGui.Linking;
using TsGui.View.GuiOptions;

namespace TsGui.Options
{
    public class OptionLibrary
    {
        public event OptionLibraryOptionAdded OptionAdded;
        public ObservableCollection<IOption> Options { get; } = new ObservableCollection<IOption>();
        public List<IValidationGuiOption> ValidationOptions { get; } = new List<IValidationGuiOption>();

        public void Add(IOption option)
        {
            this.Options.Add(option);
            IValidationGuiOption valop = option as IValidationGuiOption;
            if (valop != null) { this.ValidationOptions.Add(valop); }

            if (string.IsNullOrWhiteSpace(option.ID) == false) 
            { LinkingHub.Instance.AddSource(option); }

            this.OptionAdded?.Invoke(option, new EventArgs());
        }

        public async Task InitialiseOptionsAsync()
        {
            foreach (IOption option in this.Options)
            {
                await option.InitialiseAsync();
            }
        }
    }
}
