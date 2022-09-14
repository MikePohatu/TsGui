#region license
// Copyright (c) 2021 20Road Limited
//
// This file is part of DevChecker.
//
// DevChecker is free software: you can redistribute it and/or modify
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

// DiagnosticsHelper.cs - Helper functions for debugging outside of VS e.g. in WinPE

using System.Windows;

namespace Core.Diagnostics
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
