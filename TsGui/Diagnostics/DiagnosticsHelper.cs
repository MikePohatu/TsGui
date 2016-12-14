using System;
using System.Windows;

namespace TsGui.Diagnostics
{
    public static class DiagnosticsHelper
    {
        public static MessageBoxResult DisplayYesNoDialog(string Message, string Title)
        {
            MessageBoxResult result = MessageBox.Show(Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result;
        }

        public static MessageBoxResult DisplayOkDialog(string Message, string Title)
        {
            MessageBoxResult result = MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            return result;
        }
    }
}
