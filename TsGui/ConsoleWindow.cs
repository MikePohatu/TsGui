using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

using TsGui.View.Layout;
using TsGui.View;

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
