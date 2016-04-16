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
    /// Interaction logic for PageLayout.xaml
    /// </summary>
    public partial class PageLayout : Page
    {
        private TsPage _page;

        public PageLayout(TsPage Page)
        {
            InitializeComponent();
            this._page = Page;
        }

        public void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this._page.Cancel();
        }

        public void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            this._page.MovePrevious();
        }

        public void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this._page.MoveNext();
        }

        public void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            this._page.Finish();
        }
    }
}
