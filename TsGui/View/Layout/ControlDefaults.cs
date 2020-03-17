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

using System.Windows;
using TsGui.View.Layout;

namespace TsGui.View.Layout
{
    public static class ControlDefaults
    {
        public static void SetButtonDefaults(Formatting formatting)
        {
            formatting.VerticalAlignment = VerticalAlignment.Center;
            formatting.VerticalContentAlignment = VerticalAlignment.Center;
            formatting.HorizontalContentAlignment = HorizontalAlignment.Center;

            if (Director.Instance.UseTouchDefaults == true)
            {
                formatting.Width = 75;
                formatting.Height = 45;
            }
            else
            {
                formatting.Width = 75;
                formatting.Height = 30;
            }
        }
    }
}
