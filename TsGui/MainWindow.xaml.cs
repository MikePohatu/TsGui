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


namespace TsGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewController controller;

        public MainWindow()
        {
            InitializeComponent();
            this.controller = new ViewController(this);
            this.controller.Startup();
        }

        public MainWindow(bool TestingMode)
        {
            InitializeComponent();
            this.controller = new ViewController(this);
            this.controller.TestingMode = TestingMode;
            this.controller.Startup();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //this.Hide();
            this.controller.Cancel();
        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            //this.Hide();
            this.controller.MovePrevious();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            //this.Hide();
            this.controller.MoveNext();
        }

        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            //this.Hide();
            this.controller.Finish();
        }
    }
}
