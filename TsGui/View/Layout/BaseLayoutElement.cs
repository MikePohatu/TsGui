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

// BaseLayoutElement.cs - base class for elements in the UI tree (page, row, column, guioptions

using System.Windows;

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
        private Thickness _margin;

        public Formatting LabelFormatting { get; set; }
        public Formatting ControlFormatting { get; set; }
        public bool LabelOnRight { get; set; }
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
        public Thickness Margin
        {
            get { return this._margin; }
            set { this._margin = value; this.OnPropertyChanged(this, "Margin"); }
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
        public BaseLayoutElement Parent { get; set; }

        //constructors
        public BaseLayoutElement():base ()
        {
            this.SetDefaults();
        }

        public BaseLayoutElement(BaseLayoutElement Parent):base (Parent)
        {
            this.Parent = Parent;
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
                this.Margin = XmlHandler.GetThicknessFromXElement(x, "Margin", this.Margin);
                this.Width = XmlHandler.GetDoubleFromXElement(x, "Width", this.Width);
                this.Height = XmlHandler.GetDoubleFromXElement(x, "Height", this.Height);
                this.LeftCellWidth = XmlHandler.GetDoubleFromXElement(x, "LeftCellWidth", this.LeftCellWidth);
                this.RightCellWidth = XmlHandler.GetDoubleFromXElement(x, "RightCellWidth", this.RightCellWidth);
                this.LabelOnRight = XmlHandler.GetBoolFromXElement(x, "LabelOnRight", this.LabelOnRight);

                subx = x.Element("Label");
                if (subx != null)
                { this.LabelFormatting.LoadXml(subx); }

                subx = x.Element("Control");
                if (subx != null)
                { this.ControlFormatting.LoadXml(subx); }
            }
        }

        //public GetComplianceRootElement i.e. get the root without having any existing root
        public IComplianceRoot GetComplianceRootElement()
        {
            return this.GetComplianceRootElement(null);
        }

        //private GetComplianceRootElement. Keep the last root element as traversing up the tree. When 
        //at the top, return the last comliance root element. 
        private IComplianceRoot GetComplianceRootElement(IComplianceRoot currentroot)
        {
            IComplianceRoot thisAsRoot = this as IComplianceRoot;

            if (this.Parent == null)
            {
                if (thisAsRoot != null) { return thisAsRoot; }
                return currentroot;
            }
            else
            {
                if (thisAsRoot != null) { return this.Parent.GetComplianceRootElement(thisAsRoot); }
                else { return this.Parent.GetComplianceRootElement(currentroot); }
            }
        }

        private void SetDefaults()
        {
            if (this.Parent == null)
            {
                this.LabelFormatting = new Formatting();
                this.ControlFormatting = new Formatting();
                this.Height = double.NaN;
                this.Width = double.NaN;
                this.LeftCellWidth = double.NaN;
                this.RightCellWidth = double.NaN;
                this.ShowGridLines = false;
                this.LabelOnRight = false;
            }
            else
            {
                this.LabelFormatting = this.Parent.LabelFormatting.Clone();
                this.ControlFormatting = this.Parent.ControlFormatting.Clone();
                this.Height = double.NaN;
                this.Width = double.NaN;
                this.LeftCellWidth = this.Parent.LeftCellWidth;
                this.RightCellWidth = this.Parent.RightCellWidth;
                this.ShowGridLines = this.Parent.ShowGridLines;
                this.LabelOnRight = this.Parent.LabelOnRight;
            }
        }
    }
}
