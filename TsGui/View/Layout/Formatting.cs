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

// Formatting.cs - view model for the layout of GuiOptions. Controls the layout and 
// formatting options for the associated controls

using System;
using System.Windows;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media;

namespace TsGui.View.Layout
{
    public class Formatting: INotifyPropertyChanged
    {
        //Fields
        #region
        private string _fontweight;
        private string _fontstyle;
        private double _fontsize;
        private double _height;
        private double _width;
        private Thickness _margin;
        private Thickness _padding;        
        private VerticalAlignment _verticalalign;
        private HorizontalAlignment _horizontalalign;
        private TextAlignment _textalign;
        private SolidColorBrush _bordercolorbrush;
        private SolidColorBrush _focusedcolorbrush;
        private SolidColorBrush _mouseovercolorbrush;
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
        #endregion

        //Event handling
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        public void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
        #endregion

        //Constructor 
        public Formatting()
        {
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this._fontstyle = "Normal";
            this._fontweight = "Normal";
            this._fontsize = 11;
            this._height = Double.NaN;
            this._width = Double.NaN;
            this._margin = new Thickness(2, 2, 2, 2);
            this._padding = new Thickness(2, 2, 2, 2);
            this._verticalalign = VerticalAlignment.Bottom;
            this._horizontalalign = HorizontalAlignment.Left;
            this.TextAlignment = TextAlignment.Left;
            this._bordercolorbrush = new SolidColorBrush();
            this._mouseovercolorbrush = new SolidColorBrush();
            this._focusedcolorbrush = new SolidColorBrush();
            this.BorderBrush.Color = Colors.Gray; 
            this.MouseOverBorderBrush.Color = Colors.DarkGray;
            this.FocusedBorderBrush.Color = Colors.LightBlue;  
        }

        public void LoadXml(XElement InputXml)
        {
            //Load the XML
            #region
            XElement x;
            this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);
            this.Padding = XmlHandler.GetThicknessFromXElement(InputXml, "Padding", 2);
            this.Margin = XmlHandler.GetThicknessFromXElement(InputXml, "Margin", 2);
            this.VerticalAlignment = XmlHandler.GetVerticalAlignmentFromXElement(InputXml, "VerticalAlignment", this.VerticalAlignment);
            this.HorizontalAlignment = XmlHandler.GetHorizontalAlignmentFromXElement(InputXml, "HorizontalAlignment", this.HorizontalAlignment);
            this.TextAlignment = XmlHandler.GetTextAlignmentFromXElement(InputXml, "TextAlignment", this.TextAlignment);

            x = InputXml.Element("Font");
            if (x != null)
            {
                this.FontWeight = XmlHandler.GetStringFromXElement(x, "Weight", this.FontWeight);
                this.FontStyle = XmlHandler.GetStringFromXElement(x, "Style", this.FontStyle);
                this.FontSize = XmlHandler.GetDoubleFromXElement(x, "Size", this.FontSize);
            }
            #endregion
        }

        public Formatting Clone()
        {
            Formatting f = new Formatting();
            f.FontWeight = this.FontWeight;
            f.FontStyle = this.FontStyle;
            f.Width = this.Width;
            f.Height = this.Height;
            f.Padding = this.Padding;
            f.Margin = this.Margin;
            f.HorizontalAlignment = this.HorizontalAlignment;
            f.VerticalAlignment = this.VerticalAlignment;
            f.TextAlignment = this.TextAlignment;
            f.BorderBrush = this.BorderBrush.Clone();
            f.FocusedBorderBrush = this.FocusedBorderBrush.Clone();
            f.MouseOverBorderBrush = this.MouseOverBorderBrush.Clone();

            return f;
        }
    }
}
