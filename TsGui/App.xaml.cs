using System.Windows;
using System;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Arguments Arguments;

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

            //this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            this.StartTsGui();
        }

        private void StartTsGui()
        {
            MainWindow mainWindow = new MainWindow(this.Arguments);
            //mainWindow.Show();
        }
    }
}
