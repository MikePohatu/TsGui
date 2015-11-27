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

namespace core
{
    /// <summary>
    /// Interaction logic for PageWindow.xaml
    /// </summary>
    public partial class PageWindow : UserControl
    {
        StackPanel stackPanel = new StackPanel { Orientation = Orientation.Vertical };

        public PageWindow()
        {
            InitializeComponent();
            //code to be added to make sure config file exists
            this.Content = stackPanel;
        }

        public void AddControl(Control NewControl)
        {
            if (NewControl == null) { throw new InvalidOperationException("Null control passed to PageWindow: "); }
            else { stackPanel.Children.Add(NewControl); }         
        }
    }
}
