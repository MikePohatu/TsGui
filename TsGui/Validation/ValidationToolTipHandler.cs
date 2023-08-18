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
using TsGui.View.Layout.Events;
using TsGui.View.Layout;

namespace TsGui.Validation
{
    public class ValidationToolTipHandler: IEventer
    {
        private bool _active = false;
        private bool _shouldbeopen = false;
        private bool _isonright = true;
        private SolidColorBrush _borderbrush;
        private SolidColorBrush _mouseoverborderbrush;
        private SolidColorBrush _focusborderbrush;
        private ValidationErrorToolTip _validationerrortooltip;
        private GuiOptionBase _guioption;
        private bool _windowloaded = false;

        private SolidColorBrush _redbrush = new SolidColorBrush(Colors.Red);

        public LayoutEvents Events { get; private set; }

        /// <summary>
        /// Set true to open the tooltip immediately, false for on page next/finish
        /// </summary>
        public bool OpenToolTipImmediately { get; set; } = true;

        //Constructor
        public ValidationToolTipHandler(GuiOptionBase GuiOption)
        {
            this._guioption = GuiOption;
            this.Events = new LayoutEvents(this, GuiOption);
            this.Events.Subscribe(LayoutTopics.NextPageClicked, this.OnPageFinished);
            this.Events.Subscribe(LayoutTopics.FinishedClicked, this.OnPageFinished);
            Director.Instance.WindowLoaded += OnWindowLoaded;

            //record the default colors
            this._borderbrush = this._guioption.ControlStyle.BorderBrush;
            this._mouseoverborderbrush = this._guioption.ControlStyle.MouseOverBorderBrush;
            this._focusborderbrush = this._guioption.ControlStyle.FocusedBorderBrush;

            this._validationerrortooltip = new ValidationErrorToolTip();
            this._validationerrortooltip.PlacementTarget = this._guioption.UserControl;
            SetIconVisibilies(true);
        }

        public void OnPageFinished(object sender, LayoutEventArgs e)
        {
            this._validationerrortooltip.IsOpen = this._shouldbeopen;
        }

        public void SetTarget(UserControl Control)
        {
            this._validationerrortooltip.PlacementTarget = Control;
        }

        public void Close()
        {
            this.SetOpen(false);
        }

        public void Clear()
        {
            this.SetOpen(false);
            this._guioption.ControlStyle.BorderBrush = this._borderbrush;
            this._guioption.ControlStyle.MouseOverBorderBrush = this._mouseoverborderbrush;
            this._guioption.ControlStyle.FocusedBorderBrush = this._focusborderbrush;
        }

        public void ShowError()
        {
            this.SetOpen(true);
            this.SetPlacement();
            this._guioption.ControlStyle.BorderBrush = _redbrush;
            this._guioption.ControlStyle.MouseOverBorderBrush = _redbrush;
            this._guioption.ControlStyle.FocusedBorderBrush = _redbrush;
            this.UpdateArrows();
        }

        public void ShowInformation()
        {
            this.SetPlacement();
            this.SetOpen(true);
            this.UpdateArrows();
        }

        public void OnWindowMoving(object o, EventArgs e)
        {
            if (this._validationerrortooltip.IsOpen == true)
            {
                Director.Instance.WindowMouseUp += this.OnWindowMoved;
                this._validationerrortooltip.IsOpen = false;
            }
        }

        //reopen the popup if it was before
        public void OnWindowMoved(object o, RoutedEventArgs e)
        {
            if (this._shouldbeopen) {
                Director.Instance.WindowMouseUp -= this.OnWindowMoved;
                this._validationerrortooltip.IsOpen = true;
                UpdateArrows();
            }
        }

        /// <summary>
        /// Open the tooltip if OpenToolTipImmediately has been set to false
        /// </summary>
        public void ForceIfShouldBeOpen()
        {
            this._validationerrortooltip.IsOpen = this._shouldbeopen;
        }

        private void SetOpen(bool isopen)
        {
            //if not OpenToolTipImmediately, don't open, but always close
            if (this.OpenToolTipImmediately || isopen == false)
            {
                this._validationerrortooltip.IsOpen = isopen;
                this._active = isopen;
            }
            this._shouldbeopen = isopen;
        }

        public void OnWindowLoaded(object o, RoutedEventArgs e)
        {
            this._windowloaded = true;
            Director.Instance.WindowMoving += this.OnWindowMoving;
            this._validationerrortooltip.MouseEnter += this.OnHover;
            this._validationerrortooltip.MouseLeave += this.OnHoverLeave;
            this._validationerrortooltip.TouchEnter += this.OnHover;
            this._validationerrortooltip.TouchLeave += this.OnHoverLeave;
        }

        private void UpdateArrows()
        {
            if (this._active == false) { return; }
            if (this._windowloaded == false) { return; }
            if (this._guioption.IsRendered == false) { return; }
            this.UpdatePopupLocation();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                SetIconVisibilies(!this.HasHitRightScreenEdge());
            }));
        }

        private void SetIconVisibilies(bool isonright)
        {
            
            if (isonright)
            {
                this._validationerrortooltip.LeftArrow.Visibility = Visibility.Visible;
                this._validationerrortooltip.RightArrow.Visibility = Visibility.Collapsed;
            }
            else
            {
                this._validationerrortooltip.LeftArrow.Visibility = Visibility.Collapsed;
                this._validationerrortooltip.RightArrow.Visibility = Visibility.Visible;
            }

            this._isonright = isonright;
        }

        public void OnHover(object o, RoutedEventArgs e)
        {
            if (this._isonright)
            {
                this._validationerrortooltip.RightX.Visibility = Visibility.Visible;
            }
            else
            {
                this._validationerrortooltip.LeftX.Visibility = Visibility.Visible;
            }
        }

        public void OnHoverLeave(object o, RoutedEventArgs e)
        {
            this._validationerrortooltip.RightX.Visibility = Visibility.Collapsed;
            this._validationerrortooltip.LeftX.Visibility = Visibility.Collapsed;
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
            if (this._guioption.Style.LabelOnRight == false)
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
