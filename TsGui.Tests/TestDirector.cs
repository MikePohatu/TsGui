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
using System;
using System.Windows;
using System.ComponentModel;

using TsGui.View.Layout;
using TsGui.Options;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Authentication;
using System.Threading.Tasks;

#pragma warning disable CS0067
namespace TsGui.Tests
{
    public class TestDirector: IDirector
    {

        public event RoutedEventHandler WindowLoaded;
        public event RoutedEventHandler PageLoaded;
        public event EventHandler WindowMoved;
        public event EventHandler WindowMoving;
        public event RoutedEventHandler WindowMouseUp;
        public event EventHandler ConfigLoadFinished;
        public event EventHandler AppClosing;

        //properties
        public bool StartupFinished { get; set; }
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }
        public bool ShowGridLines { get; set; }
        public bool UseTouchDefaults { get; set; }
        public string DefaultPath { get; set; }

        public async Task StartupAsync() { await Task.CompletedTask; }

        public async Task ReloadAsync() { await Task.CompletedTask; }
        public void CloseWithError(string Title, string Message) { }

        public void AddOptionToLibary(IOption Option) { }
        public void MoveNext() { }
        public void MovePrevious() { }
        public void AddToggleControl(IToggleControl ToogleControl) { }
        public void Finish() { }
        public void Cancel() { }
        public void OnWindowClosing(object sender, CancelEventArgs e) { }
        public void OnPageLoaded(object o, RoutedEventArgs e) { }
        public void OnWindowMoving(object o, EventArgs e) { }
        public void OnWindowMouseUp(object o, RoutedEventArgs e) { }

    }
}
