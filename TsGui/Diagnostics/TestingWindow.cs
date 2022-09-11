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

namespace TsGui.Diagnostics
{
    public class TestingWindow: LoggingUiViewModel
    {
        private int _pendingresize = 0;
        private TestingWindowUI _testingwindowui;
        private ObservableCollection<IOption> _options;
        private int _currentscaling;

        public ObservableCollection<IOption> Options { get { return this._options; } }
        public TsMainWindow TsMainWindow { get; set; }
        public double ScreenHeight { get; set; }
        public double ScreenWidth { get; set; }
        public double WindowMaxHeight { get { return SystemParameters.PrimaryScreenHeight - 20; } }
        public int CurrentScaling
        {
            get { return this._currentscaling; }
            set { this._currentscaling = value; this.OnPropertyChanged(this, "CurrentScaling"); }
        }
        public ImageSource Icon { get; set; }

        public TestingWindow()
        {
            this.SubscribeToLogs();
            this._testingwindowui = new TestingWindowUI();
            this._loggingtextbox = this._testingwindowui._logtextbox;
            this._testingwindowui.DataContext = this;
            this._testingwindowui.Closed += this.OnWindowClosed;
            this._testingwindowui.ContentRendered += this.OnTestingWindowRendered;

            this.ScreenWidth = SystemParameters.PrimaryScreenWidth;
            this.ScreenHeight = SystemParameters.PrimaryScreenHeight;
            this.Icon = IconHelper.ConvertToImageSource(SystemIcons.Information);
            this._options = Director.Instance.OptionLibrary.Options;
            this.TsMainWindow = Director.Instance.TsMainWindow;
            this._testingwindowui._logclearbtn.Click += this.OnLogClearClick;
            Director.Instance.ParentWindow.Loaded += this.OnParentWindowLoaded;
            Director.Instance.ParentWindow.Closed += this.OnParentWindowClosed;
            this._testingwindowui.Show();
        }

        public void OnParentWindowLoaded(object o, EventArgs e)
        {
            this.CurrentScaling = DisplayInformation.GetScaling();
        }

        public void OnParentWindowClosed(object o, EventArgs e)
        {
            this._testingwindowui.Close();
        }

        public void OnTestingWindowRendered(object sender, EventArgs e)
        {
            this.ResizeGrids();
            //set the logGrid to the same size i.e. half window
            this._testingwindowui._logColDef.Width = new GridLength(this._testingwindowui._dataGrid.ActualWidth);

            this._testingwindowui._optionsgrid.EnableRowVirtualization = false;
            this._testingwindowui._valuecolumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            this._testingwindowui.SizeChanged += this.OnTestingWindowSizeChanged;
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
            this._testingwindowui._optionswrappergrid.Width = this._testingwindowui._dataGrid.ActualWidth;
            this._testingwindowui._optionsgrid.Width = this._testingwindowui._optionswrappergrid.ActualWidth;
            this._pendingresize--;
        }
    }
}
