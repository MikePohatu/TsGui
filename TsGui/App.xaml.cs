using System.Windows;
using System;
using System.Windows.Threading;

using TsGui.Diagnostics;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Arguments Arguments;
        MainWindow _mainwindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try { this.Arguments = new Arguments(Environment.GetCommandLineArgs()); }
            catch (Exception exc)
            {
                string msg = exc.Message + Environment.NewLine;
                MessageBox.Show(msg, "Command Line Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
                return;
            }

            this.StartTsGui();
        }

        private void StartTsGui()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);

            this._mainwindow = new MainWindow(this.Arguments);
        }

        public void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            this.HandleException(args.Exception);
        }

        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            this.HandleException( (Exception)args.ExceptionObject);     
        }

        private void HandleException(Exception e)
        {
            if (e is TsGuiKnownException) { this.ShowErrorMessageAndClose((TsGuiKnownException)e); }
            else { this.ShowErrorMessageAndClose(e.Message); }
        }

        private void ShowErrorMessageAndClose(TsGuiKnownException e)
        {
            string msg = e.CustomMessage + Environment.NewLine + Environment.NewLine + "Full error message:" + Environment.NewLine + e.Message;
            this.ShowErrorMessageAndClose(msg);
        }

        private void ShowErrorMessageAndClose(string Message)
        {
            string msg = Message;
            this._mainwindow.Controller.CloseWithError("Application Runtime Exception", msg);
        }
    }
}
