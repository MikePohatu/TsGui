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

namespace TsGui
{
    public interface IDirector
    {
        event RoutedEventHandler PageLoaded;
        event RoutedEventHandler WindowLoaded;
        event EventHandler WindowMoving;
        event EventHandler WindowMoved;
        event RoutedEventHandler WindowMouseUp;
        event EventHandler ConfigLoadFinished;
        event EventHandler AppClosing;

        //properties
        TsMainWindow TsMainWindow { get; set; }
        bool StartupFinished { get; set; }
        MainWindow ParentWindow { get; set; }
        TsPage CurrentPage { get; set; }
        bool ShowGridLines { get; }
        string DefaultPath { get; }
        bool UseTouchDefaults { get; }

        Task StartupAsync();

        Task ReloadAsync();

        void CloseWithError(string Title, string Message);
        void AddOptionToLibary(IOption Option);
        void MoveNext();
        void MovePrevious();
        void AddToggleControl(IToggleControl ToogleControl);
        void Finish();
        void Cancel();
        void OnWindowClosing(object sender, CancelEventArgs e);
        void OnPageLoaded(object o, RoutedEventArgs e);
        void OnWindowMoving(object o, EventArgs e);
        void OnWindowMouseUp(object o, RoutedEventArgs e);
    }
}
