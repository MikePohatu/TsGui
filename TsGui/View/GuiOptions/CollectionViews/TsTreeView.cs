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

using System.Xml.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using TsGui.View.Symbols;
using TsGui.Validation;
using TsGui.Linking;
using System.Windows;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class TsTreeView : CollectionViewGuiOptionBase, IGuiOption, ILinkTarget
    {
        private TsTreeViewUI _treeviewui;
        public List<ListItem> VisibleOptions { get { return this._builder.Items; } }

        //Constructor
        public TsTreeView(XElement InputXml, TsColumn Parent, IDirector director) : base(Parent, director)
        {
            this._treeviewui = new TsTreeViewUI();
            this.Control = this._treeviewui;
            this.Label = new TsLabelUI();
            this.Icon = new TsFolderUI();
            this.UserControl.DataContext = this;
            this._validationhandler = new ValidationHandler(this, director);
            this._validationtooltiphandler = new ValidationToolTipHandler(this, this._director);

            this.SetDefaults();      
            this.LoadXml(InputXml);
            this._builder.Rebuild();
            this.SetListViewDefault();

            this._treeviewui.TreeView.SelectedItemChanged += this.OnTreeViewSelectedItemChanged;
            this.UserControl.IsEnabledChanged += this.OnActiveChanged;
            this.UserControl.IsVisibleChanged += this.OnActiveChanged;
        }

        protected override void SetSelected(string value)
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
                this.CurrentItem = newdefault;
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

        private void SetListViewDefault()
        {
            if (this._nodefaultvalue == false)
            {
                string defaultval = this._setvaluequerylist.GetResultWrangler()?.GetString();
                this.SetSelected(defaultval);
            }
            this.NotifyUpdate();
        }
    }
}
