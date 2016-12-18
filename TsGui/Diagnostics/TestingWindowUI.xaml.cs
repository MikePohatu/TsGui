using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TsGui.Diagnostics
{
    /// <summary>
    /// Interaction logic for TestingWindow.xaml
    /// </summary>
    public partial class TestingWindowUI : Window
    {
        public TestingWindowUI()
        {
            InitializeComponent();
            this._optionsgrid.Loaded += this.OnWindowLoaded;
        }

        private void OnWindowLoaded(object o, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.SetWidths()));
        }

        private void SetWidths()
        {
            this._optionsgrid.Width = this._optionsgrid.ActualWidth;
            this._optionsgrid.MinWidth = this._optionsgrid.ActualWidth;
            
        }
    }
}
