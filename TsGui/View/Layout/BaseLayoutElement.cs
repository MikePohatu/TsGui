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
    public abstract class BaseLayoutElement: GroupableUIElementBase
    {
        private bool _showgridlines;
        private double _labelwidth;
        private double _controlwidth;
        private double _width;
        private double _height;
        private BaseLayoutElement _parent;

        public Formatting LabelFormatting { get; set; }
        public Formatting ControlFormatting { get; set; }
        public bool ShowGridLines
        {
            get { return this._showgridlines; }
            set { this._showgridlines = value; this.OnPropertyChanged(this, "ShowGridLines"); }
        }
        public double Width
        {
            get { return this._width; }
            set { this._width = value; this.OnPropertyChanged(this, "Width"); }
        }
        public double LabelWidth
        {
            get { return this._labelwidth; }
            set { this._labelwidth = value; this.OnPropertyChanged(this, "LabelWidth"); }
        }
        public double ControlWidth
        {
            get { return this._controlwidth; }
            set { this._controlwidth = value; this.OnPropertyChanged(this, "ControlWidth"); }
        }
        public double Height
        {
            get { return this._height; }
            set { this._height = value; this.OnPropertyChanged(this, "Height"); }
        }
        //constructors
        public BaseLayoutElement(MainController MainController):base (MainController)
        {
            this.SetDefaults();
        }

        public BaseLayoutElement(BaseLayoutElement Parent, MainController MainController):base (MainController)
        {
            this._parent = Parent;
            this.SetDefaults();

            //register grouping events from the parent element
            Parent.GroupableEnable += this.OnParentEnable;
            Parent.GroupableHide += this.OnParentHide;
        }

        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.ShowGridLines);
            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);
            this.LabelWidth = XmlHandler.GetDoubleFromXElement(InputXml, "LabelWidth", this.LabelWidth);
            this.ControlWidth = XmlHandler.GetDoubleFromXElement(InputXml, "ControlWidth", this.ControlWidth);
            this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);

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
            }
        }

        private void SetDefaults()
        {
            if (this._parent == null)
            {
                this.LabelFormatting = new Formatting();
                this.ControlFormatting = new Formatting();
                this.Height = double.NaN;
                this.Width = double.NaN;
                this.LabelWidth = double.NaN;
                this.ControlWidth = double.NaN;
                this.ShowGridLines = false;
            }
            else
            {
                this.LabelFormatting = this._parent.LabelFormatting.Clone();
                this.ControlFormatting = this._parent.ControlFormatting.Clone();
                this.Height = this._parent.Height;
                this.Width = this._parent.Width;
                this.LabelWidth = this._parent.LabelWidth;
                this.ControlWidth = this._parent.ControlWidth;
                this.ShowGridLines = this._parent.ShowGridLines;
            }
        }
    }
}
