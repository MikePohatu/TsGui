//    Copyright (C) 2017 Mike Pohatu

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
