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

using gui_lib;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Builder builder;

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            this.Startup();
        }

        private void Startup()
        {
            builder = new Builder();
            builder.ParentWindow = this;
            builder.Start();
        }
    }
}
