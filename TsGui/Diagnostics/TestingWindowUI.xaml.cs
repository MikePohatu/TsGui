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

// TestingWindowUI.cs - shows live data at runtime  

using System;
using System.Windows;
using System.Windows.Threading;


namespace TsGui.Diagnostics
{
    /// <summary>
    /// Interaction logic for TestingWindow.xaml
    /// </summary>
    public partial class TestingWindowUI : Window
    {
        public TestingWindowUI()
        {
            InitializeComponent();
            this._optionsgrid.Loaded += this.OnWindowLoaded;
        }

        private void OnWindowLoaded(object o, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.SetWidths()));
        }

        private void SetWidths()
        {
            this._optionsgrid.Width = this._optionsgrid.ActualWidth;
            this._optionsgrid.MinWidth = this._optionsgrid.ActualWidth;
        }
    }
}
