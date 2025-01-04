#region license
// Copyright (c) 2025 Mike Pohatu
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
using System;
using TsGui.View.Layout.Events;

namespace TsGui.View.Layout
{
    public abstract class BaseLayoutElement: GroupableUIElementBase, IEventer
    {
        private bool _showgridlines;
        public Style LabelStyle { get { return this.Style.LabelStyle; } }
        public Style ControlStyle { get { return this.Style.ControlStyle; } }
        public StyleTree Style { get; private set; } = new StyleTree();

        public LayoutEvents Events { get; private set; }
        public bool ShowGridLines
        {
            get { return this._showgridlines; }
            set { this._showgridlines = value; this.OnPropertyChanged(this, "ShowGridLines"); }
        }


        protected bool _controlenabled = true;
        public bool ControlEnabled
        {
            get { return this._controlenabled; }
            set { this._controlenabled = value; this.OnPropertyChanged(this, "ControlEnabled"); }
        }

        public ParentLayoutElement Parent { get; set; }

        //constructors
        public BaseLayoutElement():base ()
        {
            this.Events = new LayoutEvents(this);
            this.SetDefaults();
        }

        public BaseLayoutElement(ParentLayoutElement Parent):base (Parent)
        {
            this.Parent = Parent;
            this.Events = new LayoutEvents(this,Parent);
            if (Parent != null)
            {
                this._controlenabled = Parent.ControlEnabled;
            }
            this.SetDefaults();
        }

        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this.ShowGridLines = XmlHandler.GetBoolFromXml(InputXml, "ShowGridLines", this.ShowGridLines);
            this.ControlEnabled = !XmlHandler.GetBoolFromXml(InputXml, "ReadOnly", !this._controlenabled);

            //Load legacy options
            this.Style.LeftCellWidth = XmlHandler.GetDoubleFromXml(InputXml, "LabelWidth", this.Style.LeftCellWidth);
            this.Style.RightCellWidth = XmlHandler.GetDoubleFromXml(InputXml, "ControlWidth", this.Style.RightCellWidth);

            this.Style.Width = XmlHandler.GetDoubleFromXml(InputXml, "Width", this.Style.Width);
            this.Style.Height = XmlHandler.GetDoubleFromXml(InputXml, "Height", this.Style.Height);


            //import any styles
            var stylesEl = InputXml.Attribute("Styles");
            if (stylesEl != null)
            {
                foreach (string id in stylesEl.Value.Split(','))
                {
                    if (string.IsNullOrWhiteSpace(id) == false) { this.Style.Import(id.Trim()); }
                }
            }

            
            //We need to also check for Formatting options for backwards compatibility
            List<string> formattings = new List<string>{ "Formatting", "Style"};
            foreach (string label in formattings)
            {
                var stylesX = InputXml.Elements(label);
                if (stylesX != null)
                {
                    foreach (var stylex in stylesX)
                    {
                        this.Style.LoadXml(stylex);
                    }
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
                this.Style.LeftCellWidth = double.NaN;
                this.Style.RightCellWidth = double.NaN;
                this.ShowGridLines = false;
                this.Style.LabelOnRight = false;
                this.LabelStyle.Padding = new Thickness(1);
                this.LabelStyle.Margin = new Thickness(2);
                this.ControlStyle.Margin = new Thickness(2);
                this.ControlStyle.Padding = new Thickness(1);
            }
            else
            {
                this.Style = this.Parent.Style.Clone();
                this.ShowGridLines = this.Parent.ShowGridLines;
                this.Style.LabelOnRight = this.Parent.Style.LabelOnRight;
            }
        }
    }
}
