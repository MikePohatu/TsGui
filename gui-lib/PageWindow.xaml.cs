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
using System.Windows.Shapes;

namespace gui_lib
{
    /// <summary>
    /// Interaction logic for PageWindow.xaml
    /// </summary>
    public partial class PageWindow : Window
    {
        //StackPanel controlPanel = new StackPanel { Orientation = Orientation.Vertical };
        //StackPanel labelPanel = new StackPanel { Orientation = Orientation.Vertical };
        //Grid maingrid = new Grid();

        private int padding;
        

        public PageWindow(int PageHeight,int PageWidth,int PagePadding)
        {
            InitializeComponent();

            this.Height = PageHeight;
            this.Width = PageWidth;
            this.padding = PagePadding;

            //this.MainGrid.ShowGridLines = true;
            //this.Content = maingrid;
            this.MainGrid.Margin = new Thickness(this.padding);
            this.Title = "TsGui";
            //this.WindowStyle = WindowStyle.None;
        }

        public void AddPanel(Panel NewPanel)
        {
            if (NewPanel == null) { throw new InvalidOperationException("null control passed to pagewindow: "); }
            else
            {
                Grid.SetRow(NewPanel,0);
                this.MainGrid.Children.Add(NewPanel);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
