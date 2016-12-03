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
            this._mainwindow = new MainWindow(this.Arguments);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string msg = "Error message: " + Environment.NewLine + e.Exception.Message;
            e.Handled = true;
            this._mainwindow.Controller.CloseWithError("Application Runtime Exception",msg);
        }
    }
}
