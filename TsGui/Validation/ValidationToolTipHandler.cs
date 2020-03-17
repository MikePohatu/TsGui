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

// ValidationToolTip.cs - tooltip class to handle tooltip validation events in the gui

using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;

using TsGui.View.GuiOptions;

namespace TsGui.Validation
{
    public class ValidationToolTipHandler
    {
        private Color _bordercolor;
        private Color _mouseoverbordercolor;
        private Color _focusbordercolor;
        private Popup _popup;
        private ValidationErrorToolTip _validationerrortooltip;
        private GuiOptionBase _guioption;
        private bool _windowloaded = false;

        //Constructor
        public ValidationToolTipHandler(GuiOptionBase GuiOption)
        {
            this._guioption = GuiOption;
            Director.Instance.WindowLoaded += OnWindowLoaded;

            //record the default colors
            this._bordercolor = this._guioption.ControlFormatting.BorderBrush.Color;
            this._mouseoverbordercolor = this._guioption.ControlFormatting.MouseOverBorderBrush.Color;
            this._focusbordercolor = this._guioption.ControlFormatting.FocusedBorderBrush.Color;

            this._validationerrortooltip = new ValidationErrorToolTip();
            this._popup = new Popup();
            this._popup.AllowsTransparency = true;
            this._popup.Child = this._validationerrortooltip;
            this.SetTarget(this._guioption.Control);
            this._popup.IsOpen = false;
        }

        public void SetTarget(UserControl Control)
        {
            this._popup.PlacementTarget = Control;
        }

        public void Clear()
        {
            if (this._popup.IsOpen == true)
            {
                Director.Instance.WindowMouseUp -= this.OnWindowMouseUp;
                this._popup.IsOpen = false;
                this._guioption.ControlFormatting.BorderBrush.Color = this._bordercolor;
                this._guioption.ControlFormatting.MouseOverBorderBrush.Color = this._mouseoverbordercolor;
                this._guioption.ControlFormatting.FocusedBorderBrush.Color = this._focusbordercolor;
            }
        }

        public void ShowError()
        {
            if (this._popup.IsOpen == false)
            {
                this.SetPlacement();
                Director.Instance.WindowMouseUp += this.OnWindowMouseUp;
                this._popup.IsOpen = true;
                this._guioption.ControlFormatting.BorderBrush.Color = Colors.Red;
                this._guioption.ControlFormatting.MouseOverBorderBrush.Color = Colors.Red;
                this._guioption.ControlFormatting.FocusedBorderBrush.Color = Colors.Red;                
            }
            this.UpdateArrows();
        }

        public void ShowInformation()
        {
            if (this._popup.IsOpen == false)
            {
                this.SetPlacement();
                Director.Instance.WindowMouseUp += this.OnWindowMouseUp;
                this._popup.IsOpen = true;
            }
            this.UpdateArrows();
        }


        public void OnWindowMouseUp(object o, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.UpdateArrows()));
        }

        public void OnWindowLoaded(object o, RoutedEventArgs e)
        { this._windowloaded = true; }

        private void UpdateArrows()
        {
            if (this._windowloaded == false ) { return; }
            if (this._guioption.IsRendered == false) { return; }
            this.UpdatePopupLocation();
            
            if (this.HasHitRightScreenEdge() == false)
            {
                this._validationerrortooltip.LeftArrow.Visibility = Visibility.Visible;
                this._validationerrortooltip.RightArrow.Visibility = Visibility.Hidden;
            }
            else
            {
                this._validationerrortooltip.LeftArrow.Visibility = Visibility.Hidden;
                this._validationerrortooltip.RightArrow.Visibility = Visibility.Visible;
            }
        }

        private bool HasHitRightScreenEdge()
        {
            Point locationOfControl = this._guioption.Control.PointToScreen(new Point(0, 0));
            Point locationOfPopup = this._validationerrortooltip.PointToScreen(new Point(0, 0));

            double controlRightEdge = locationOfControl.X + this._guioption.Control.ActualWidth;

            if (controlRightEdge <= locationOfPopup.X)
            { return false; }
            else { return true; }
        }

        private void UpdatePopupLocation()
        {
            this._popup.IsOpen = false;
            this._popup.IsOpen = true;
        }

        private void SetPlacement()
        {
            //this is to handle WPF quirks with touch devices
            if (this._guioption.LabelOnRight == false)
            {
                if (SystemParameters.MenuDropAlignment == false) { this._popup.Placement = PlacementMode.Right; }
                else { this._popup.Placement = PlacementMode.Left; }
            }
            else
            {
                if (SystemParameters.MenuDropAlignment == false) { this._popup.Placement = PlacementMode.Left; }
                else { this._popup.Placement = PlacementMode.Right; }
            }
        }
    }
}
