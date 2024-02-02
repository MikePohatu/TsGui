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

using System.Collections.Generic;
using System.Threading.Tasks;
using TsGui.Diagnostics;
using TsGui.Grouping;
using TsGui.Options.NoUI;
using TsGui.Options;
using TsGui.View.Layout;
using TsGui.Linking;
using System.Xml.Linq;
using TsGui.Authentication;
using TsGui.Scripts;
using MessageCrap;

namespace TsGui
{
    public static class CoreData
    {
        public static bool ProdMode { get; set; } = false;
        public static TsButtons Buttons { get; set; }

        public static List<TsPage> Pages { get; set; }
        public static List<IToggleControl> Toggles { get; set; }
        public static NoUIContainer NouiContainer { get; set; }
        public static TestingWindow TestingWindow { get; set; }

        public static void Reset()
        {
            MessageHub.Reset();
            LinkingHub.Instance.Reset();
            StyleLibrary.Reset();
            GroupLibrary.Reset();
            OptionLibrary.Reset();
            ScriptLibrary.Reset();
            Buttons = new TsButtons();
            Pages = new List<TsPage>();
            Toggles = new List<IToggleControl>();
            NouiContainer = null;
        }
    }
}
