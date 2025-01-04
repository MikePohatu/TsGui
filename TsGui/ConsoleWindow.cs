#region license
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
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

using TsGui.View.Layout;
using TsGui.View;
using Core;

namespace TsGui
{
    public class ConsoleWindow: ViewModelBase
    {
        private const string Kernel32 = "kernel32.dll";
        private static ConsoleWindow _instance = null;

        [DllImport(Kernel32)]
        private static extern IntPtr GetConsoleWindow();

        private Window _modal;

        private string _text;
        public string Text
        {
            get { return this._text; }
            set { this._text = value; this.OnPropertyChanged(this, "Text"); }
        }

        public static ConsoleWindow Instance
        {
            get 
            { 
                if (ConsoleWindow._instance == null) { ConsoleWindow._instance = new ConsoleWindow(); }
                return ConsoleWindow._instance;
            }
        }

        private ConsoleWindow() 
        {
            if (ConsoleWindow.HasConsole == false)
            {
                this._modal = new ConsoleOutput();
                this._modal.DataContext = this;
                this._modal.Show();
            }
        }


        public static bool HasConsole {
            get
            {
                return GetConsoleWindow() != IntPtr.Zero;
            }
        }

        public static void WriteLine(string input)
        {
            Console.WriteLine(input);
            ConsoleWindow.Instance.Text = ConsoleWindow.Instance.Text + input + Environment.NewLine;
        }

        public static void WriteLine()
        {
            Console.WriteLine();
            ConsoleWindow.Instance.Text = ConsoleWindow.Instance.Text + Environment.NewLine;
        }

        public static void Pause()
        {
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
}
