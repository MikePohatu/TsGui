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

// BaseLayoutElement.cs - base class for elements in the UI tree (page, row, column, guioptions

using TsGui.Grouping;
using System.Xml.Linq;

namespace TsGui.View.Layout
{
    public abstract class BaseLayoutElement: GroupableBase
    {
        private bool _showgridlines = false;

        public Formatting LabelFormatting { get; set; }
        public Formatting ControlFormatting { get; set; }
        public Formatting GridFormatting { get; set; }
        public bool ShowGridLines
        {
            get { return this._showgridlines; }
            set { this._showgridlines = value; this.OnPropertyChanged(this, "ShowGridLines"); }
        }

        //constructors
        public BaseLayoutElement(MainController MainController):base (MainController)
        {
            this.LabelFormatting = new Formatting();
            this.ControlFormatting = new Formatting();
            this.GridFormatting = new Formatting();
        }

        public BaseLayoutElement(BaseLayoutElement Parent, MainController MainController):base (MainController)
        {
            this.LabelFormatting = Parent.LabelFormatting.Clone();
            this.ControlFormatting = Parent.ControlFormatting.Clone();
            this.GridFormatting = Parent.GridFormatting.Clone();
            this.ShowGridLines = Parent.ShowGridLines;
        }

        

        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.ShowGridLines);

            XElement x;
            XElement subx;
            
            x = InputXml.Element("Formatting");
            if (x != null)
            {
                subx = x.Element("Label");
                if (subx != null)
                { this.LabelFormatting.LoadXml(subx); }

                subx = x.Element("Control");
                if (subx != null)
                { this.ControlFormatting.LoadXml(subx); }

                subx = x.Element("Grid");
                if (subx != null)
                { this.GridFormatting.LoadXml(subx); }
            }
        }
    }
}
