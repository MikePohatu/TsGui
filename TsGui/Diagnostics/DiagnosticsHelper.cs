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

// DiagnosticsHelper.cs - Helper functions for debugging outside of VS e.g. in WinPE

using System.Windows;

namespace TsGui.Diagnostics
{
    public static class DiagnosticsHelper
    {
        public static MessageBoxResult DisplayYesNoDialog(string Message, string Title)
        {
            MessageBoxResult result = MessageBox.Show(Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result;
        }

        public static MessageBoxResult DisplayOkDialog(string Message, string Title)
        {
            MessageBoxResult result = MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            return result;
        }
    }
}
