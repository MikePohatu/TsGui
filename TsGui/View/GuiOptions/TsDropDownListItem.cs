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

// TsDropDownListItem.cs - class that is primarily to work around an issue where using a list of
// keyvaluepairs doesn't disable/enable properly. The text doesn't grey out. This class uses a label 
// control to make this work properly. 

namespace TsGui.View.GuiOptions
{
    public class TsDropDownListItem
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public TsDropDownListItem(string Value, string Text)
        {
            this.Value = Value;
            this.Text = Text;
        }
    }
}

