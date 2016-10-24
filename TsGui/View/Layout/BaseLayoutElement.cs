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

// TsColumn.cs - class for columns in the gui window

using TsGui.Grouping;

namespace TsGui.View.Layout
{
    public abstract class BaseLayoutElement: GroupableBase
    {
        protected bool _showgridlines = false;

        public Formatting LabelFormatting { get; set; }
        public Formatting ControlFormatting { get; set; }
        public Formatting GridFormatting { get; set; }

        public BaseLayoutElement(MainController MainController):base (MainController)
        {

        }

        public bool ShowGridLines
        {
            get { return this._showgridlines; }
            set { this._showgridlines = value; this.OnPropertyChanged(this, "ShowGridLines"); }
        }
    }
}
