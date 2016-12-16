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

// TestingWindow.cs - view model for the TestingWindowUI

using System.Collections.ObjectModel;
using System.Windows;
using System;

using TsGui.View.Layout;

namespace TsGui.Diagnostics
{
    public class TestingWindow
    {
        private MainController _controller;
        private TestingWindowUI _testingwindowui;
        private ObservableCollection<IOption> _options;

        public ObservableCollection<IOption> Options { get { return this._options; } }
        public TsMainWindow TsMainWindow { get; set; }
        public double ScreenHeight { get; set; }
        public double ScreenWidth { get; set; }

        public TestingWindow(MainController Controller)
        {
            this._controller = Controller;
            this.ScreenWidth = SystemParameters.PrimaryScreenWidth;
            this.ScreenHeight = SystemParameters.PrimaryScreenHeight;
            this._testingwindowui = new TestingWindowUI();
            this._testingwindowui.DataContext = this;
            this._options = this._controller.OptionLibrary.Options;
            this.TsMainWindow = this._controller.TsMainWindow;
            this._controller.ParentWindow.Closed += this.OnParentWindowClosing;
            this._testingwindowui.Show();
        }

        public void OnParentWindowClosing(object o, EventArgs e)
        {
            this._testingwindowui.Close();
        }
    }
}
