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

// Delegates.cs - Delegates for control parts of the app

using System;
using System.Windows;
using TsGui.View.Layout;

namespace TsGui.Events
{
    public delegate void TsGuiWindowEventHandler(object sender, RoutedEventArgs e);
    public delegate void ComplianceRetryEventHandler(IRootLayoutElement sender, EventArgs e);
}
