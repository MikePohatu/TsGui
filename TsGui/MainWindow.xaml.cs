using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;


namespace TsGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainController _controller;

        public MainWindow()
        {
            
            InitializeComponent();
            this._controller = new MainController(this);
            this.WrapperGrid.DataContext = this._controller;
            //Init.Startup(this);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this._controller.Cancel();
        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            this._controller.MovePrevious();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this._controller.MoveNext();
        }

        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            this._controller.Finish();
        }

        private void windowDrag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
