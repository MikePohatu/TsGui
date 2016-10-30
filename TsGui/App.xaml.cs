using System.Windows;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            string msg = "Test startup";
            MessageBox.Show(msg, "Startup test", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
