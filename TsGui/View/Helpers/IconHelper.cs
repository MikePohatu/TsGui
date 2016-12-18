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

// IconHelper.cs - Helper methods for icons e.g. convert icons to imagesource etc.

using System.Windows.Media;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace TsGui.View.Helpers
{
    public static class IconHelper
    {
        public static ImageSource ConvertToImageSource(Icon Icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(Icon.Handle,Int32Rect.Empty,BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
