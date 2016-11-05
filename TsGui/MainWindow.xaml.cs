//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// MainWindow.xaml.cs - MainWindow backing class. Creates a MainController on 
// instantiation which starts and controls the application. 

using System.Windows;
using System.Windows.Input;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainController _controller;

        public MainWindow(Arguments Arguments)
        {            
            InitializeComponent();
            this._controller = new MainController(this, Arguments);
        }

        private void windowDrag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
