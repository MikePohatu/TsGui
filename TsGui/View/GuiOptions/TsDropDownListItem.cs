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

namespace TsGui.View.GuiOptions
{
    public class TsDropDownListItem: GroupableUIElementBase
    {
        public int Index { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public Formatting ItemFormatting { get; set; }

        public TsDropDownListItem(int Index, string Value, string Text, Formatting Formatting, TsDropDownList Parent, MainController MainController):base(MainController)
        {
            this.Init(Index, Formatting, Parent);
            this.Value = Value;
            this.Text = Text; 
        }

        public TsDropDownListItem(int Index, XElement InputXml, Formatting Formatting, TsDropDownList Parent, MainController MainController) : base(MainController)
        {
            this.Init(Index, Formatting, Parent);
            this.Text = XmlHandler.GetStringFromXElement(InputXml, "Text", this.Text);
            this.Value = XmlHandler.GetStringFromXElement(InputXml, "Value", this.Value);
            base.LoadXml(InputXml);
        }

        private void Init(int Index, Formatting Formatting, TsDropDownList Parent)
        {
            this.ItemFormatting = Formatting;
        }
    }
}

