﻿#region license
// Copyright (c) 2025 Mike Pohatu
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

// MainWindow.xaml.cs - MainWindow backing class. Creates a MainController on 
// instantiation which starts and controls the application. 

using System.Windows;
using System.Windows.Input;
using Core.Logging;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {            
            InitializeComponent();
            Log.Trace("MainWindow initialized");
            MessageCrap.MessageHub.Error += Log.Error;
            MessageCrap.MessageHub.Warn += Log.Warn;
            MessageCrap.MessageHub.Info += Log.Info;
            MessageCrap.MessageHub.Debug += Log.Debug;
            MessageCrap.MessageHub.Trace += Log.Trace;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        { this.DragMove(); }
    }
}
