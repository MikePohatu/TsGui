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

// Formatting.cs - view model for the layout of GuiOptions. Controls the layout and 
// formatting options for the associated controls

using System;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;

namespace TsGui.View.Layout
{
    public class Formatting: ViewModelBase
    {
        //Fields
        #region
        private string _fontweight;
        private string _fontstyle;
        private double _fontsize;
        private double _height;
        private double _width;
        private double _cornerradius;
        private Thickness _margin;
        private Thickness _padding;
        private Thickness _borderthickness;
        private VerticalAlignment _verticalalign;
        private HorizontalAlignment _horizontalalign;
        private VerticalAlignment _verticalcontentalign;
        private HorizontalAlignment _horizontcontentalalign;
        private TextAlignment _textalign;
        private SolidColorBrush _bordercolorbrush;
        private SolidColorBrush _focusedcolorbrush;
        private SolidColorBrush _mouseovercolorbrush;
        private SolidColorBrush _fontcolorbrush;
        #endregion

        //Properties
        #region 
        public string FontWeight
        {
            get { return this._fontweight; }
            set { this._fontweight = value; this.OnPropertyChanged(this, "FontWeight"); }
        }
        public string FontStyle
        {
            get { return this._fontstyle; }
            set { this._fontstyle = value; this.OnPropertyChanged(this, "FontStyle"); }
        }
        public double FontSize
        {
            get { return this._fontsize; }
            set { this._fontsize = value; this.OnPropertyChanged(this, "FontSize"); }
        }
        public double CornerRadius
        {
            get { return this._cornerradius; }
            set { this._cornerradius = value; this.OnPropertyChanged(this, "Rounding"); }
        }
        public double Height
        {
            get { return this._height; }
            set { this._height = value; this.OnPropertyChanged(this, "Height"); }
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

        public Thickness Padding
        {
            get { return this._padding; }
            set { this._padding = value; this.OnPropertyChanged(this, "Padding"); }
        }

        public Thickness BorderThickness
        {
            get { return this._borderthickness; }
            set { this._borderthickness = value; this.OnPropertyChanged(this, "BorderThickness"); }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return this._verticalalign; }
            set { this._verticalalign = value; this.OnPropertyChanged(this, "VerticalAlignment"); }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return this._horizontalalign; }
            set { this._horizontalalign = value; this.OnPropertyChanged(this, "HorizontalAlignment"); }
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get { return this._verticalcontentalign; }
            set { this._verticalcontentalign = value; this.OnPropertyChanged(this, "VerticalContentAlignment"); }
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return this._horizontcontentalalign; }
            set { this._horizontcontentalalign = value; this.OnPropertyChanged(this, "HorizontalContentAlignment"); }
        }

        public TextAlignment TextAlignment
        {
            get { return this._textalign; }
            set { this._textalign = value; this.OnPropertyChanged(this, "TextAlignment"); }
        }
        public SolidColorBrush BorderBrush
        {
            get { return this._bordercolorbrush; }
            set { this._bordercolorbrush = value; this.OnPropertyChanged(this, "BorderBrush"); }
        }
        public SolidColorBrush FocusedBorderBrush
        {
            get { return this._focusedcolorbrush; }
            set { this._focusedcolorbrush = value; this.OnPropertyChanged(this, "FocusedBorderBrush"); }
        }
        public SolidColorBrush MouseOverBorderBrush
        {
            get { return this._mouseovercolorbrush; }
            set { this._mouseovercolorbrush = value; this.OnPropertyChanged(this, "MouseOverBorderBrush"); }
        }
        public SolidColorBrush FontColorBrush
        {
            get { return this._fontcolorbrush; }
            set { this._fontcolorbrush = value; this.OnPropertyChanged(this, "FontColorBrush"); }
        }
        #endregion

        //Constructor 
        public Formatting()
        {
            this.SetDefaults();
        }

        public Formatting(XElement InputXml)
        {
            this.SetDefaults();
            this.LoadXml(InputXml);
        }

        private void SetDefaults()
        {
            if (Director.Instance.UseTouchDefaults==true)
            {
                this.FontSize = 12;
            }
            else
            {
                this.FontSize = 11;
                
            }
            this.Margin = new Thickness(1);
            this.Padding = new Thickness(2);
            this.BorderThickness = new Thickness(1);
            this.FontStyle = "Normal";
            this.FontWeight = "Normal";
            this.CornerRadius = 0;
            
            this.Height = Double.NaN;
            this.Width = Double.NaN;
            
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.HorizontalContentAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.TextAlignment = TextAlignment.Left;
            this.BorderBrush = new SolidColorBrush(Colors.Gray);
            this.MouseOverBorderBrush = new SolidColorBrush(Colors.DarkGray);
            this.FocusedBorderBrush = new SolidColorBrush(Colors.LightBlue);
            this.FontColorBrush = new SolidColorBrush(Colors.Black);
        }

        public void LoadXml(XElement InputXml)
        {
            //Load the XML
            #region
            XElement x;
            this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);
            this.CornerRadius = XmlHandler.GetDoubleFromXElement(InputXml, "CornerRadius", this.CornerRadius);
            this.Padding = XmlHandler.GetThicknessFromXElement(InputXml, "Padding", this.Padding);
            this.Margin = XmlHandler.GetThicknessFromXElement(InputXml, "Margin", this.Margin);
            this.VerticalAlignment = XmlHandler.GetVerticalAlignmentFromXElement(InputXml, "VerticalAlignment", this.VerticalAlignment);
            this.HorizontalAlignment = XmlHandler.GetHorizontalAlignmentFromXElement(InputXml, "HorizontalAlignment", this.HorizontalAlignment);
            this.VerticalContentAlignment = XmlHandler.GetVerticalAlignmentFromXElement(InputXml, "VerticalContentAlignment", this.VerticalContentAlignment);
            this.HorizontalContentAlignment = XmlHandler.GetHorizontalAlignmentFromXElement(InputXml, "HorizontalContentAlignment", this.HorizontalContentAlignment);
            this.TextAlignment = XmlHandler.GetTextAlignmentFromXElement(InputXml, "TextAlignment", this.TextAlignment);

            x = InputXml.Element("Font");
            if (x != null)
            {
                this.FontWeight = XmlHandler.GetStringFromXElement(x, "Weight", this.FontWeight);
                this.FontStyle = XmlHandler.GetStringFromXElement(x, "Style", this.FontStyle);
                this.FontSize = XmlHandler.GetDoubleFromXElement(x, "Size", this.FontSize);
                this.FontColorBrush = XmlHandler.GetSolidColorBrushFromXElement(x, "Color", this.FontColorBrush);
            }
            #endregion
        }

        public Formatting Clone()
        {
            Formatting f = new Formatting();
            f.FontWeight = this.FontWeight;
            f.FontStyle = this.FontStyle;
            f.FontSize = this.FontSize;
            //f.Width = this.Width;
            //f.Height = this.Height;
            f.CornerRadius = this.CornerRadius;
            f.Padding = this.Padding;
            f.Margin = this.Margin;
            f.HorizontalAlignment = this.HorizontalAlignment;
            f.VerticalAlignment = this.VerticalAlignment;
            f.HorizontalContentAlignment = this.HorizontalContentAlignment;
            f.VerticalContentAlignment = this.VerticalContentAlignment;
            f.TextAlignment = this.TextAlignment;
            f.BorderBrush = this.BorderBrush.Clone();
            f.FocusedBorderBrush = this.FocusedBorderBrush.Clone();
            f.MouseOverBorderBrush = this.MouseOverBorderBrush.Clone();
            f.FontColorBrush = this.FontColorBrush.Clone();

            return f;
        }
    }
}
