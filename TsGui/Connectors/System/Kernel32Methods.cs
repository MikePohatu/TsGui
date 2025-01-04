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

// Kernel32Methods.cs - provide easier access to Kernel32.dll methods

using System.Runtime.InteropServices;

namespace TsGui.Connectors.System
{
    public static class Kernel32Methods
    {
        [DllImport("Kernel32.dll")]
        internal static extern bool AttachConsole(int processId);

        [DllImport("Kernel32.dll")]
        internal static extern int FreeConsole();
    }
}
