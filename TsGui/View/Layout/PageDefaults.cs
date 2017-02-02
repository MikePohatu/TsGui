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

// PageDefaults.cs - Default settings for new TsPages.

using System.Windows;
using System.Windows.Media;

namespace TsGui.View.Layout
{
    public class PageDefaults
    {
        public string HeadingTitle { get; set; }
        public string HeadingText { get; set; }
        public SolidColorBrush HeadingBgColor { get; set; }
        public SolidColorBrush HeadingFontColor { get; set; }
        public TsButtons Buttons { get; set; }
        public MainController RootController { get; set; }
        public TsMainWindow Parent { get; set; }
        public TsHeading Heading { get; set; }
    }
}
