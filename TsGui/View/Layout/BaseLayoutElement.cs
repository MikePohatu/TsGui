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
using TsGui.Validation;
using System.Collections.Generic;

namespace TsGui.View.Layout
{
    public abstract class BaseLayoutElement: GroupableUIElementBase
    {
        private bool _showgridlines;
        private double _leftcellwidth;
        private double _rightcellwidth;

        public Style LabelStyle { get; set; }
        public Style ControlStyle { get; set; }
        public Style Style { get; set; }
        public bool LabelOnRight { get; set; }
        public bool ShowGridLines
        {
            get { return this._showgridlines; }
            set { this._showgridlines = value; this.OnPropertyChanged(this, "ShowGridLines"); }
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

        public ParentLayoutElement Parent { get; set; }

        //constructors
        public BaseLayoutElement():base ()
        {
            this.SetDefaults();
        }

        public BaseLayoutElement(ParentLayoutElement Parent):base (Parent)
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
            this.Style.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Style.Width);
            this.Style.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Style.Height);

            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.ShowGridLines);
            
            XElement x;
            XElement subx;
            
            List<string> styleLabels = new List<string>{ "Formatting", "Style"};
            foreach (string label in styleLabels)
            {
                x = InputXml.Element(label);
                if (x != null)
                {
                    this.LeftCellWidth = XmlHandler.GetDoubleFromXElement(x, "LeftCellWidth", this.LeftCellWidth);
                    this.RightCellWidth = XmlHandler.GetDoubleFromXElement(x, "RightCellWidth", this.RightCellWidth);
                    this.LabelOnRight = XmlHandler.GetBoolFromXElement(x, "LabelOnRight", this.LabelOnRight);

                    this.Style.LoadXml(x);

                    subx = x.Element("Label");
                    if (subx != null)
                    { this.LabelStyle.LoadXml(subx); }

                    subx = x.Element("Control");
                    if (subx != null)
                    { this.ControlStyle.LoadXml(subx); }
                }
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
                this.LabelStyle = new Style();
                this.ControlStyle = new Style();
                this.Style = new Style();
                this.LeftCellWidth = double.NaN;
                this.RightCellWidth = double.NaN;
                this.ShowGridLines = false;
                this.LabelOnRight = false;
                this.LabelStyle.Padding = new Thickness(1);
                this.LabelStyle.Margin = new Thickness(2);
                this.ControlStyle.Margin = new Thickness(2);
                this.ControlStyle.Padding = new Thickness(1);
            }
            else
            {
                this.LabelStyle = this.Parent.LabelStyle.Clone();
                this.ControlStyle = this.Parent.ControlStyle.Clone();
                this.Style = this.Parent.Style.Clone();
                this.LeftCellWidth = this.Parent.LeftCellWidth;
                this.RightCellWidth = this.Parent.RightCellWidth;
                this.ShowGridLines = this.Parent.ShowGridLines;
                this.LabelOnRight = this.Parent.LabelOnRight;
            }
        }
    }
}
