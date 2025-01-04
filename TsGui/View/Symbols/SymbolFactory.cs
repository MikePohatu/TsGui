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
using System.Windows.Controls;
using System;
using Core.Logging;

namespace TsGui.View.Symbols
{
    public static class SymbolFactory
    {
        public static UserControl GetSymbol(string type)
        {
            if (type == null) { throw new ArgumentException("Missing Type attribute on Symbol"); }

            Log.Info("Creating symbol, type: " + type);

            if (type == "OuUI")
            {
                return new TsFolderUI();
            }
            else if (type == "Cross")
            {
                return new TsCrossUI();
            }
            else if (type == "Tick")
            {
                return new TsTickUI();
            }
            else if (type == "Warn")
            {
                return new TsWarnUI();
            }
            else if (type == "TrafficLight")
            {
                return new TsTrafficLightUI();
            }
            else { return null; }
        }

        public static UserControl Copy(UserControl control)
        {
            if (control == null) { return null; }

            string type = control.GetType().ToString();
            Log.Info("Creating symbol, type: " + type);

            if (type == "TsGui.View.Symbols.TsFolderUI")
            {
                return new TsFolderUI();
            }
            else if (type == "TsGui.View.Symbols.TsCrossUI")
            {
                return new TsCrossUI();
            }
            else if (type == "TsGui.View.Symbols.TsTickUI")
            {
                return new TsTickUI();
            }
            else if (type == "TsGui.View.Symbols.TsWarnUI")
            {
                return new TsWarnUI();
            }
            else if (type == "TsGui.View.Symbols.TsTrafficLightUI")
            {
                return new TsTrafficLightUI();
            }
            else { return null; }
        }
    }
}
