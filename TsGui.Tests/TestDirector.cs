//    Copyright (C) 2017 Mike Pohatu

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

using System;
using System.Windows;
using System.ComponentModel;

using TsGui.View.Layout;
using TsGui.Options;
using TsGui.Events;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Authentication;

namespace TsGui.Tests
{
    public class TestDirector: IDirector
    {
        private AuthLibrary _authlib = new AuthLibrary();
        public event TsGuiWindowEventHandler WindowLoaded;
        public event TsGuiWindowMovingEventHandler WindowMoving;
        public event TsGuiWindowEventHandler WindowMouseUp;
        public event ConfigLoadFinishedEventHandler ConfigLoadFinished;

        //properties
        public AuthLibrary AuthLibrary { get { return this._authlib; } }
        public LinkingLibrary LinkingLibrary { get; }
        public GroupLibrary GroupLibrary { get; }
        public TsMainWindow TsMainWindow { get; set; }
        public OptionLibrary OptionLibrary { get; }
        public EnvironmentController EnvironmentController { get; }
        public bool StartupFinished { get; set; }
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }
        public bool ShowGridLines { get; set; }


        public void Init(MainWindow ParentWindow, Arguments Arguments) { }


        public void CloseWithError(string Title, string Message) { }
        public void Startup() { }
        public void AddOptionToLibary(IOption Option) { }
        public void MoveNext() { }
        public void MovePrevious() { }
        public void AddToggleControl(IToggleControl ToogleControl) { }
        public void Finish() { }
        public void Cancel() { }
        public void OnWindowClosing(object sender, CancelEventArgs e) { }
        public void OnWindowLoaded(object o, RoutedEventArgs e) { }
        public void OnWindowMoving(object o, EventArgs e) { }
        public void OnWindowMouseUp(object o, RoutedEventArgs e) { }

    }
}
