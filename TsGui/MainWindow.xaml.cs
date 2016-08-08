using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

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
            
        }

        private void windowDrag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
