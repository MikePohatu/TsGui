#region license
// Copyright (c) 2020 Mike Pohatu
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
using System.Xml.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using TsGui.View.Symbols;
using TsGui.Validation;
using TsGui.Linking;
using System.Windows;
using TsGui.View.Layout;
using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class TsTreeView : CollectionViewGuiOptionBase, IGuiOption, ILinkTarget
    {
        private TsTreeViewUI _treeviewui;
        public List<ListItem> VisibleOptions { get { return this._builder.Items; } }

        //Constructor
        public TsTreeView(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            this._treeviewui = new TsTreeViewUI();
            this.Control = this._treeviewui;
            this.Label = new TsLabelUI();
            this.Icon = new TsFolderUI();
            this.UserControl.DataContext = this;
            this.ValidationHandler = new ValidationHandler(this);
            this._validationtooltiphandler = new ValidationToolTipHandler(this);

            this.SetDefaults();      
            this.LoadXml(InputXml);
            this._builder.RebuildAsync(null).ConfigureAwait(false);
            this.SetListViewDefaultAsync().ConfigureAwait(false);

            this._treeviewui.TreeView.SelectedItemChanged += this.OnTreeViewSelectedItemChanged;
            this.UserControl.IsEnabledChanged += this.OnActiveChanged;
            this.UserControl.IsVisibleChanged += this.OnActiveChanged;
        }

        protected override void SetSelected(string value, Message message)
        {
            if (string.IsNullOrWhiteSpace(value) == true ) { return; }
            ListItem newdefault = null;

            foreach (ListItem item in this.VisibleOptions)
            {
                if ((item.Focusable == true) && (item.Value == value))
                {
                    newdefault = item;
                    break;
                }
                ListItem subitem = item.NavigateToValue(value);
                if (subitem != null)
                {
                    newdefault = subitem;
                    break;
                }
            }

            if (newdefault != null)
            {
                this.SetValue(newdefault, message);
                newdefault.IsSelected = true;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ListItem item = (ListItem)this._treeviewui.TreeView.SelectedItem;
            if (item.Focusable == true) { this.CurrentItem = item; }
            else { this.CurrentItem = null; }
            this.OnSelectionChanged(sender, e);
        }

        private async Task SetListViewDefaultAsync()
        {
            if (this._nodefaultvalue == false)
            {
                string defaultval = (await this._querylist.GetResultWrangler(null))?.GetString();
                this.SetSelected(defaultval, null);
            }
            this.NotifyViewUpdate();
        }
    }
}
