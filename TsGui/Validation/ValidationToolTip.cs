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

// ValidationToolTip.cs - tooltip class to handle tooltip validation events in the gui

using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;

using TsGui.View.GuiOptions;

namespace TsGui.Validation
{
    public class ValidationToolTip
    {
        private Color _bordercolor;
        private Color _mouseoverbordercolor;
        private Color _focusbordercolor;
        private ToolTip _tooltip;
        private ValidationErrorToolTip _validationerrortooltip;
        private GuiOptionBase _guioption;

        //Constructor
        public ValidationToolTip(GuiOptionBase GuiOption)
        {
            this._guioption = GuiOption;
            //record the default colors
            this._bordercolor = this._guioption.ControlFormatting.BorderBrush.Color;
            this._mouseoverbordercolor = this._guioption.ControlFormatting.MouseOverBorderBrush.Color;
            this._focusbordercolor = this._guioption.ControlFormatting.FocusedBorderBrush.Color;

            this._validationerrortooltip = new ValidationErrorToolTip();
            this._tooltip = new ToolTip();
            this._tooltip.Padding = new Thickness(0);
            this._tooltip.Background = Brushes.Transparent;
            this._tooltip.BorderBrush = Brushes.Transparent;
            this._tooltip.Content = _validationerrortooltip;
        }

        public void Clear()
        {
            this._tooltip.IsOpen = false;
            this._tooltip.StaysOpen = false;
            this._guioption.UserControl.ToolTip = null;
            this._guioption.ControlFormatting.BorderBrush.Color = this._bordercolor;
            this._guioption.ControlFormatting.MouseOverBorderBrush.Color = this._mouseoverbordercolor;
            this._guioption.ControlFormatting.FocusedBorderBrush.Color = this._focusbordercolor;
        }

        public void Show()
        {
            this._guioption.UserControl.ToolTip = this._tooltip;
            this._tooltip.PlacementTarget = this._guioption.UserControl;
            this._tooltip.Placement = PlacementMode.Right;
            this._tooltip.StaysOpen = true;
            this._tooltip.IsOpen = true;

            //update the colors to red. 
            this._guioption.ControlFormatting.BorderBrush.Color = Colors.Red;
            this._guioption.ControlFormatting.MouseOverBorderBrush.Color = Colors.Red;
            this._guioption.ControlFormatting.FocusedBorderBrush.Color = Colors.Red;
        }
    }
}
