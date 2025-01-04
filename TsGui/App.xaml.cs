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
using System.Windows;
using System;
using System.Windows.Threading;

using Core.Logging;
using Core.Diagnostics;
using TsGui.Authentication.LocalConfig;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try { Arguments.Instance.Import(Environment.GetCommandLineArgs()); }
            catch (Exception exc)
            {
                string msg = exc.Message + Environment.NewLine;
                MessageBox.Show(msg, "Command Line Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown(1);
                return;
            }

            LoggingHelpers.InitLogging();

            if (string.IsNullOrWhiteSpace(Arguments.Instance.ToHash) == false)
            {
                string key = Arguments.Instance.Key;

                ConsoleWindow.WriteLine();
                if (string.IsNullOrEmpty(key))
                {
                    key = Password.CreateKey();
                    ConsoleWindow.WriteLine("Key: " + key);
                }

                string pw = Password.HashPassword(Arguments.Instance.ToHash, key);

                ConsoleWindow.WriteLine("Hash: " + pw);
                ConsoleWindow.WriteLine(Environment.NewLine + "Example config:");
                ConsoleWindow.WriteLine(@"
                    <Authentication Type=""Password"" AuthID=""conf_auth"">
                        <Password>
                            <PasswordHash>"+pw+@"</ PasswordHash >
                            <Key>"+key+@"</Key>
                        </Password>
                    </Authentication>");
                ConsoleWindow.Pause();
            }
            else
            {
                this.StartTsGui();
            }
        }

        private void StartTsGui()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
         
            Log.Info("*TsGui started - version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Director.Instance.StartupAsync().ConfigureAwait(false);
        }


        //Exception handler methods
        #region
        public void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            if (args.Exception is System.IO.FileNotFoundException)
            {
                Log.Error("System.IO.FileNotFoundException logged. This is a known issue with DragDrop");
            }
            else
            {
                Log.Fatal("OnDispatcherUnhandledException:" + args.Exception.ToString());
                Log.Fatal("OnDispatcherUnhandledException:" + args.Exception.Message);
                this.HandleException(sender, args.Exception, args.Exception.StackTrace);
            }
            
        }

        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Log.Fatal("OnUnhandledException:" + e.Message);
            this.HandleException(sender, e, e.StackTrace);     
        }

        private void HandleException(object sender, Exception e, string AdditionalText)
        {
            if (e is KnownException) { this.ShowErrorMessageAndClose((KnownException)e); }
            else
            {
                string s = "Source: " + sender.ToString() + Environment.NewLine + Environment.NewLine + "Exception: " + e.Message + Environment.NewLine;
                if (!string.IsNullOrEmpty(AdditionalText)) { s = s + Environment.NewLine + AdditionalText; }
                this.ShowErrorMessageAndClose(s);
            }

        }
        #endregion


        private void ShowErrorMessageAndClose(KnownException e)
        {
            string msg = e.CustomMessage + Environment.NewLine + Environment.NewLine + "Full error message:" + Environment.NewLine + e.Message;
            this.ShowErrorMessageAndClose(msg);
        }

        private void ShowErrorMessageAndClose(string Message)
        {
            Log.Fatal("Closing TsGui. Error message: " + Message);
            string msg = Message;
            Director.Instance.CloseWithError("Application Runtime Exception", msg);
        }

        
    }
}
