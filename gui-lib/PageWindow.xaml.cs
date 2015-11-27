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
        Grid maingrid = new Grid();

        private int padding; 

        public PageWindow(int PageHeight,int PageWidth,int PagePadding)
        {
            InitializeComponent();

            this.Height = PageHeight;
            this.Width = PageWidth;
            this.padding = PagePadding;

            this.maingrid.ShowGridLines = true;
            this.Content = maingrid;
            //this.maingrid.Margin = this.padding;
        }

        public void AddControl(Control NewControl,Label Label,int ColumnIndex)
        {
            if (NewControl == null) { throw new InvalidOperationException("Null control passed to PageWindow: "); }
            else
            {
                Grid.SetColumn(Label, ColumnIndex);
                Grid.SetColumn(NewControl, ColumnIndex + 1);               

                this.maingrid.Children.Add(NewControl);
                this.maingrid.Children.Add(Label);
            }
        }
    }
}
