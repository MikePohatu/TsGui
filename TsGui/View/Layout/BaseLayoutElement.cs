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
        private double _leftcellwidth;
        private double _rightcellwidth;
        private double _width;
        private double _height;
        private BaseLayoutElement _parent;

        public Formatting LabelFormatting { get; set; }
        public Formatting ControlFormatting { get; set; }
        public bool InvertColumns { get; set; }
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
        public double LeftCellWidth
        {
            get { return this._leftcellwidth; }
            set { this._leftcellwidth = value; this.OnPropertyChanged(this, "LeftCellWidth"); }
        }
        public double RightCellWidth
        {
            get { return this._rightcellwidth; }
            set { this._rightcellwidth = value; this.OnPropertyChanged(this, "RightCellWidth"); }
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

        public BaseLayoutElement(BaseLayoutElement Parent, MainController MainController):base (Parent,MainController)
        {
            this._parent = Parent;
            this.SetDefaults();
        }

        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            //Load legacy options
            this.LeftCellWidth = XmlHandler.GetDoubleFromXElement(InputXml, "LabelWidth", this.LeftCellWidth);
            this.RightCellWidth = XmlHandler.GetDoubleFromXElement(InputXml, "ControlWidth", this.RightCellWidth);

            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.ShowGridLines);
            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);           
            this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);

            XElement x;
            XElement subx;
            

            x = InputXml.Element("Formatting");
            if (x != null)
            {
                this.LeftCellWidth = XmlHandler.GetDoubleFromXElement(x, "LeftCellWidth", this.LeftCellWidth);
                this.RightCellWidth = XmlHandler.GetDoubleFromXElement(x, "RightCellWidth", this.RightCellWidth);
                this.InvertColumns = XmlHandler.GetBoolFromXElement(x, "InvertColumns", this.InvertColumns);

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
                this.LeftCellWidth = double.NaN;
                this.RightCellWidth = double.NaN;
                this.ShowGridLines = false;
                this.InvertColumns = false;
            }
            else
            {
                this.LabelFormatting = this._parent.LabelFormatting.Clone();
                this.ControlFormatting = this._parent.ControlFormatting.Clone();
                this.Height = double.NaN;
                this.Width = double.NaN;
                this.LeftCellWidth = this._parent.LeftCellWidth;
                this.RightCellWidth = this._parent.RightCellWidth;
                this.ShowGridLines = this._parent.ShowGridLines;
                this.InvertColumns = this._parent.InvertColumns;
            }
        }
    }
}
