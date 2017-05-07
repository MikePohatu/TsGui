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
using System.Collections.Generic;

using TsGui.View.Layout;
using TsGui.Grouping;
using TsGui.Diagnostics.Logging;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class ListItem: GroupableUIElementBase
    {
        private CollectionViewGuiOptionBase _parent;
        public string Value { get; set; }
        public string Text { get; set; }
        public Formatting ItemFormatting { get; set; }
        public List<ListItem> ListItems { get; set; }

        public ListItem(string Value, string Text, Formatting Formatting, CollectionViewGuiOptionBase parentlist, IDirector MainController):base(MainController)
        { 
            this.Init(Formatting, parentlist);
            this.Value = Value;
            this.Text = Text;
            LoggerFacade.Info("Created ListItem: " + this.Text + ". Value: " + this.Value);
        }

        public ListItem(XElement InputXml, Formatting Formatting, CollectionViewGuiOptionBase parentlist, IDirector MainController) : base(MainController)
        {
            this.Init(Formatting, parentlist);
            this.LoadXml(InputXml);
            LoggerFacade.Info("Created ListItem: " + this.Text + ". Value: " + this.Value);
        }

        private void Init(Formatting Formatting, CollectionViewGuiOptionBase parentlist)
        {
            this.ListItems = new List<ListItem>();
            this.ItemFormatting = Formatting;
            this._parent = parentlist;
        }

        private new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            this.Text = XmlHandler.GetStringFromXElement(inputxml, "Text", this.Text);
            this.Value = XmlHandler.GetStringFromXElement(inputxml, "Value", this.Value);

            foreach (XElement x in inputxml.Elements())
            {
                if (x.Name == "Toggle")
                {
                    x.Add(new XElement("Enabled", this.Value));
                    Toggle t = new Toggle(this._parent, this._director, x);
                    this._parent.IsToggle = true;
                }
                else if (x.Name == "Option")
                {
                    ListItem item = new ListItem(x, this.ItemFormatting, this._parent, this._director);
                    this.ListItems.Add(item);
                }
            }
        }

        protected override Group AddGroup(string GroupID)
        {
            Group g = base.AddGroup(GroupID);
            this._parent.AddItemGroup(g);
            return g;
        }
    }
}

