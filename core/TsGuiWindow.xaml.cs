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

using System.Xml.Linq;

namespace core
{
    /// <summary>
    /// Interaction logic for TsGuiWindow.xaml
    /// </summary>
    public partial class TsGuiWindow : UserControl
    {
        private XmlHandler handler;
        private string exefolder = AppDomain.CurrentDomain.BaseDirectory;

        public TsGuiWindow()
        {
            handler = new XmlHandler();
            XElement x = handler.Read(exefolder + @"\Config.xml");
            InitializeComponent();           
        }
    }
}
