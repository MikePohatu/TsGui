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
using TsGui.Events;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Authentication;
using System.Threading.Tasks;

#pragma warning disable CS0067
namespace TsGui.Tests
{
    public class TestDirector: IDirector
    {

        public event TsGuiWindowEventHandler WindowLoaded;
        public event TsGuiWindowEventHandler PageLoaded;
        public event TsGuiWindowMovingEventHandler WindowMoved;
        public event TsGuiWindowMovingEventHandler WindowMoving;
        public event TsGuiWindowEventHandler WindowMouseUp;
        public event ConfigLoadFinishedEventHandler ConfigLoadFinished;

        //properties
        public GroupLibrary GroupLibrary { get; }
        public TsMainWindow TsMainWindow { get; set; }
        public OptionLibrary OptionLibrary { get; }
        public bool StartupFinished { get; set; }
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }
        public bool ShowGridLines { get; set; }
        public bool UseTouchDefaults { get; set; }
        public string DefaultPath { get; set; }

        public async Task InitAsync(MainWindow ParentWindow, Arguments Arguments) { await Task.Delay(0); }


        public void CloseWithError(string Title, string Message) { }
        public void Startup() { }
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
