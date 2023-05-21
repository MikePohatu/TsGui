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
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using TsGui.View.Layout;
using TsGui.Grouping;
using Core.Logging;
using TsGui.View.Symbols;
using System.Windows;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class ListItem : GroupableUIElementBase, IComparable<ListItem>
    {
        private CollectionViewGuiOptionBase _parent;
        private bool _isselected;
        private bool _isexpanded;

        public string Value { get; set; }
        public string Text { get; set; }
        public bool Focusable { get; set; }
        public bool IsSelected
        {
            get { return this._isselected; }
            set {
                if (this.Focusable == true) { this._isselected = value; }
                else { this._isselected = false; }
                this.OnPropertyChanged(this, "IsSelected");
            }
        }
        public bool IsExpanded
        {
            get { return this._isexpanded; }
            set { this._isexpanded = value; this.OnPropertyChanged(this, "IsExpanded"); }
        }
        public Layout.Style ItemStyle { get; set; }
        public List<ListItem> ItemsList { get; set; }
        public UserControl Icon { get; set; }

        public ListItem(string Value, string Text, Layout.Style Style, CollectionViewGuiOptionBase parentlist) : base()
        {
            this.Init(Style, parentlist);
            this.Value = Value;
            if (string.IsNullOrEmpty(Text)) { this.Text = Value; } //if no text is set, just use the value
            else { this.Text = Text; }

            Log.Info("Created ListItem: " + this.Text + ". Value: " + this.Value);
        }

        public ListItem(XElement InputXml, Layout.Style Style, CollectionViewGuiOptionBase parentlist) : base()
        {
            this.Init(Style, parentlist);
            this.LoadXml(InputXml);
            Log.Info("Created ListItem: " + this.Text + ". Value: " + this.Value);
        }

        public ListItem NavigateToValue(string Value)
        {
            if (Value == null) { return null; }
            foreach (ListItem item in this.ItemsList)
            {
                if ((item.Focusable == true) && (Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    this.IsExpanded = true;
                    return item;
                }
                ListItem subitem = item.NavigateToValue(Value);
                if (subitem != null)
                {
                    this.IsExpanded = true;
                    return subitem;
                }
            }
            return null;
        }

        private void Init(Layout.Style Style, CollectionViewGuiOptionBase parentlist)
        {
            this.Focusable = true;
            this.Icon = SymbolFactory.Copy(parentlist.Icon);
            this.ItemsList = new List<ListItem>();
            this.ItemStyle = Style;
            this._parent = parentlist;
        }

        private new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            //Values can be elements or attributes. Attributes get priority
            this.Text = XmlHandler.GetStringFromXml(inputxml, "Text", this.Text);
            this.Value = XmlHandler.GetStringFromXml(inputxml, "Value", this.Value);
            this.Focusable = XmlHandler.GetBoolFromXml(inputxml, "Selectable", this.Focusable);
            this.Text = XmlHandler.GetStringFromXml(inputxml, "Text", this.Text);
            this.Value = XmlHandler.GetStringFromXml(inputxml, "Value", this.Value);
            this.Focusable = XmlHandler.GetBoolFromXml(inputxml, "Selectable", this.Focusable);

            foreach (XElement x in inputxml.Elements())
            {
                if (x.Name == "Toggle")
                {
                    x.Add(new XElement("Enabled", this.Value));
                    Toggle t = new Toggle(this._parent, x);
                    this._parent.IsToggle = true;
                }
                else if (x.Name == "Option")
                {
                    ListItem item = new ListItem(x, this.ItemStyle, this._parent);
                    this.ItemsList.Add(item);
                }
            }

            if (string.IsNullOrEmpty(this.Text)) { this.Text = Value; } //if no text is set, just use the value
        }

        protected override Group AddGroup(string GroupID)
        {
            Group g = base.AddGroup(GroupID);
            this._parent.AddItemGroup(g);
            return g;
        }

        public override string ToString()
        {
            return this.Text;
        }

        public int CompareTo(ListItem item)
        {
            return this.Text.CompareTo(item.Text);
        }

        public void Sort()
        {
            this.ItemsList.Sort();
            foreach (ListItem item in this.ItemsList)
            {
                item.Sort();
            }
        }
    }
}

