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

// DisplayInformation.cs - responsible for getting information on the display e.g. scaling and resolution etc

using System;
using System.Windows;
using System.Windows.Media;

namespace TsGui.Helpers
{
    public static class DisplayInformation
    {
        /// <summary>
        /// Return an integer representing the display scaling percentage e.g. 100 = 100%
        /// </summary>
        /// <returns></returns>
        public static int GetScaling()
        {
            Window mainwindow = Application.Current.MainWindow;
            PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual(mainwindow);
            Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
            int returnval = Convert.ToInt32(m.M11) * 100;
            return returnval;
        }
    }
}
