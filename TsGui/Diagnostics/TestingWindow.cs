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

// TestingWindow.cs - view model for the TestingWindowUI

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Threading;

using TsGui.View;
using TsGui.View.Layout;
using TsGui.View.Helpers;
using TsGui.Helpers;
using TsGui.Diagnostics;
using TsGui.Options;
using Core;
using Core.Logging;
using NLog;
using System.Threading.Tasks;

namespace TsGui.Diagnostics
{
    public class TestingWindow: LoggingUiViewModel
    {
        private int _pendingresize = 0;
        private ObservableCollection<IOption> _options;
        private int _currentscaling;

        public TestingWindowUI Window { get; private set; }
        public ObservableCollection<IOption> Options { get { return this._options; } }
        public double ScreenHeight { get; set; }
        public double ScreenWidth { get; set; }
        public double WindowMaxHeight { get { return SystemParameters.PrimaryScreenHeight - 20; } }
        public int CurrentScaling
        {
            get { return this._currentscaling; }
            set { this._currentscaling = value; this.OnPropertyChanged(this, "CurrentScaling"); }
        }
        public bool Verbose
        {
            get { return LoggingHelpers.GetLoggingLevel("livedata") < 2; }
            set
            {
                if (value) { LoggingHelpers.SetLoggingLevel(0, "livedata"); }
                else { LoggingHelpers.SetLoggingLevel(2, "livedata"); }
                this.OnPropertyChanged(this, "Verbose");
            }
        }
        public ImageSource Icon { get; set; }

        public TestingWindow()
        {
            this.Verbose = Arguments.Instance.Debug;
            this.Window = new TestingWindowUI();
            this._loggingtextbox = this.Window._logtextbox;
            this.Window.DataContext = this;
            this.Window.Closed += this.OnWindowClosed;
            this.Window.ContentRendered += this.OnTestingWindowRendered;

            this.ScreenWidth = SystemParameters.PrimaryScreenWidth;
            this.ScreenHeight = SystemParameters.PrimaryScreenHeight;
            this.Icon = IconHelper.ConvertToImageSource(SystemIcons.Information);
            this._options = OptionLibrary.Options;
            this.Window._logclearbtn.Click += this.OnLogClearClick;

            this.Window._reloadbtn.Click += this.OnReloadClicked;
            Director.Instance.ParentWindow.Loaded += this.OnParentWindowLoaded;
            Director.Instance.AppClosing += this.OnAppClosing;
            this.Window.Show();
        }

        private async void OnReloadClicked(object sender, RoutedEventArgs e)
        {
            await Director.Instance.ReloadAsync();
        }

        public void OnParentWindowLoaded(object o, EventArgs e)
        {
            this.CurrentScaling = DisplayInformation.GetScaling();
        }

        public void OnAppClosing(object o, EventArgs e)
        {
            this.Window.Close();
        }

        public void OnTestingWindowRendered(object sender, EventArgs e)
        {
            this.ResizeGrids();
            //set the logGrid to the same size i.e. half window
            this.Window._logColDef.Width = new GridLength(this.Window._dataGrid.ActualWidth);

            this.Window._optionsgrid.EnableRowVirtualization = false;
            this.Window._valuecolumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            this.Window.SizeChanged += this.OnTestingWindowSizeChanged;
        }

        public void OnTestingWindowSizeChanged(object o, EventArgs e)
        {
            if (this._pendingresize < 2)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => this.ResizeGrids()));
                this._pendingresize++;
            } 
        }

        private void ResizeGrids()
        {
            this.Window._optionswrappergrid.Width = this.Window._dataGrid.ActualWidth;
            this.Window._optionsgrid.Width = this.Window._optionswrappergrid.ActualWidth;
            this._pendingresize--;
        }
    }
}
