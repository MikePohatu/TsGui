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
        private bool _active = false;
        private SolidColorBrush _borderbrush;
        private SolidColorBrush _mouseoverborderbrush;
        private SolidColorBrush _focusborderbrush;
        private ValidationErrorToolTip _validationerrortooltip;
        private GuiOptionBase _guioption;
        private bool _windowloaded = false;

        private SolidColorBrush _redbrush = new SolidColorBrush(Colors.Red);

        //Constructor
        public ValidationToolTipHandler(GuiOptionBase GuiOption)
        {
            this._guioption = GuiOption;
            Director.Instance.WindowLoaded += OnWindowLoaded;

            //record the default colors
            this._borderbrush = this._guioption.ControlFormatting.BorderBrush;
            this._mouseoverborderbrush = this._guioption.ControlFormatting.MouseOverBorderBrush;
            this._focusborderbrush = this._guioption.ControlFormatting.FocusedBorderBrush;

            this._validationerrortooltip = new ValidationErrorToolTip();
            this._validationerrortooltip.PlacementTarget = this._guioption.UserControl;
            this._validationerrortooltip.LeftArrow.Visibility = Visibility.Visible;
            this._validationerrortooltip.RightArrow.Visibility = Visibility.Hidden;
            Director.Instance.WindowMoving += this.OnWindowMoving;
        }

        public void SetTarget(UserControl Control)
        {
            this._validationerrortooltip.PlacementTarget = Control;
        }

        public void Close()
        {
            this._validationerrortooltip.IsOpen = false;
        }

        public void Clear()
        {
            this._validationerrortooltip.IsOpen = false;
            this._guioption.ControlFormatting.BorderBrush = this._borderbrush;
            this._guioption.ControlFormatting.MouseOverBorderBrush = this._mouseoverborderbrush;
            this._guioption.ControlFormatting.FocusedBorderBrush = this._focusborderbrush;
            this._active = false;
        }

        public void ShowError()
        {
            this._validationerrortooltip.IsOpen = true;
            this.SetPlacement();
            this._guioption.ControlFormatting.BorderBrush = _redbrush;
            this._guioption.ControlFormatting.MouseOverBorderBrush = _redbrush;
            this._guioption.ControlFormatting.FocusedBorderBrush = _redbrush;
            this._active = true;
            this.UpdateArrows();
        }

        public void ShowInformation()
        {
            this.SetPlacement();
            this._validationerrortooltip.IsOpen = true;
            this._active = true;
            this.UpdateArrows();
        }

        public void OnWindowMoving(object o, EventArgs e)
        {
            this.Close();
        }

        public void OnWindowLoaded(object o, RoutedEventArgs e)
        {
            this._windowloaded = true;
        }

        private void UpdateArrows()
        {
            if (this._active == false) { return; }
            if (this._windowloaded == false) { return; }
            if (this._guioption.IsRendered == false) { return; }
            this.UpdatePopupLocation();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
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
            }));
        }

        private bool HasHitRightScreenEdge()
        {
            //make sure guioption control is visible to avoid 'This Visual is not connected to a PresentationSource' exceptions
            if (this._guioption.Control.IsVisible == false) { return false; }
            if (this._validationerrortooltip.IsOpen == false) { return false; }

            Point locationOfControl = this._guioption.Control.PointToScreen(new Point(0, 0));
            Point locationOfPopup = this._validationerrortooltip.grid.PointToScreen(new Point(0, 0));

            double controlRightEdge = locationOfControl.X + this._guioption.Control.ActualWidth;

            if (controlRightEdge <= locationOfPopup.X)
            { return false; }
            else { return true; }
        }

        private void UpdatePopupLocation()
        {
            this._validationerrortooltip.IsOpen = false;
            this._validationerrortooltip.IsOpen = true;
        }

        private void SetPlacement()
        {
            //this is to handle WPF quirks with touch devices
            if (this._guioption.LabelOnRight == false)
            {
                if (SystemParameters.MenuDropAlignment == false) { this._validationerrortooltip.Placement = PlacementMode.Right; }
                else { this._validationerrortooltip.Placement = PlacementMode.Left; }
            }
            else
            {
                if (SystemParameters.MenuDropAlignment == false) { this._validationerrortooltip.Placement = PlacementMode.Left; }
                else { this._validationerrortooltip.Placement = PlacementMode.Right; }
            }
        }
    }
}
