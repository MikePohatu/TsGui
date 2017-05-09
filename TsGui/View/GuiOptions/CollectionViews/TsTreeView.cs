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


using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using TsGui.View.Symbols;
using TsGui.Validation;
using TsGui.Linking;
using TsGui.Queries;
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

            this._treeviewui.TreeView.SelectedItemChanged += this.OnTreeViewSelectedItemChanged;
        }

        protected override void SetSelected(string value)
        {
            ListItem newdefault = null;
            bool changed = false;

            //foreach (ListItem item in this.VisibleOptions)
            //{
            //    if ((item.Value == value))
            //    {
            //        newdefault = item;
            //        changed = true;
            //        break;
            //    }
            //}

            if (changed == true)
            {
                this.CurrentItem = newdefault;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.CurrentItem = (ListItem)this._treeviewui.TreeView.SelectedItem;
            this.OnSelectionChanged(sender, e);
        }

    }
}
