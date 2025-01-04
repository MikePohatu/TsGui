#region license
// Copyright (c) 2025 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Threading;

namespace TsGui.View.Layout
{
    /// <summary>
    /// Interaction logic for PageLayout.xaml
    /// </summary>
    public partial class TsPageUI : Page
    {
        private TsPage _page;

        public TsPageUI(TsPage Page)
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

        public async void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            await this._page.FinishAsync();
        }
    }
}
