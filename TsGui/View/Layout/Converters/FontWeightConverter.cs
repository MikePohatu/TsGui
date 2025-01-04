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

//    FontWeightConverter.cs: Converts strings to fontweight for databinding.

using System.Windows.Data;
using System.Windows;
using System;

namespace TsGui.View.Layout.Converters
{
    public class FontWeightConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch (value.ToString().ToUpper())
                {
                    case "BOLD":
                        return FontWeights.Bold;
                    case "EXTRABOLD":
                        return FontWeights.ExtraBold;
                    case "NORMAL":
                        return FontWeights.Normal;
                    case "LIGHT":
                        return FontWeights.Light;
                    default:
                        return FontWeights.Normal;
                }
            }
            else { return FontWeights.Normal; }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontWeight f = (FontWeight)value;
            return f.ToString();
        }
    }
}
