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

// TsDropDownListItem.cs - class to hold entries in dropdownlist
using System.Xml.Linq;

using TsGui.View.Layout;
using TsGui.Grouping;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class ListItem: GroupableUIElementBase
    {
        private CollectionViewGuiOptionBase _parentlist;
        public string Value { get; set; }
        public string Text { get; set; }
        public Formatting ItemFormatting { get; set; }

        public ListItem(string Value, string Text, Formatting Formatting, CollectionViewGuiOptionBase parentlist, IDirector MainController):base(MainController)
        { 
            this.Init(Formatting, parentlist);
            this.Value = Value;
            this.Text = Text; 
        }

        public ListItem(XElement InputXml, Formatting Formatting, CollectionViewGuiOptionBase parentlist, IDirector MainController) : base(MainController)
        {
            this.Init(Formatting, parentlist);
            this.Text = XmlHandler.GetStringFromXElement(InputXml, "Text", this.Text);
            this.Value = XmlHandler.GetStringFromXElement(InputXml, "Value", this.Value);
            base.LoadXml(InputXml);
        }

        private void Init(Formatting Formatting, CollectionViewGuiOptionBase parentlist)
        {
            this.ItemFormatting = Formatting;
            this._parentlist = parentlist;
        }

        protected override Group AddGroup(string GroupID)
        {
            Group g = base.AddGroup(GroupID);
            this._parentlist.AddItemGroup(g);
            return g;
        }
    }
}

