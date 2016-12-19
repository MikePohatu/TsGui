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
