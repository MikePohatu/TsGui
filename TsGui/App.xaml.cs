using System.Windows;
using System;
using System.Windows.Threading;

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

        public void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            this.ShowErrorMessageAndClose(e.Exception.Message);
        }

        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;
            this.ShowErrorMessageAndClose(exception.Message);
        }

        private void ShowErrorMessageAndClose(string Message)
        {
            string msg = "Error message: " + Environment.NewLine + Message;
            this._mainwindow.Controller.CloseWithError("Application Runtime Exception", msg);
        }
    }
}
