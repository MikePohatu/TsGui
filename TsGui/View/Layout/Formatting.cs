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
        private double _height;
        private double _width;
        private Thickness _margin;
        private Thickness _padding;        
        private VerticalAlignment _verticalalign;
        private HorizontalAlignment _horizontalalign;
        private SolidColorBrush _bordercolor;
        private SolidColorBrush _hoverovercolor;
        #endregion

        //Properties
        #region 

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
        public SolidColorBrush BorderColorBrush
        {
            get { return this._bordercolor; }
            set { this._bordercolor = value; this.OnPropertyChanged(this, "BorderColorBrush"); }
        }
        public SolidColorBrush HoverOverColorBrush
        {
            get { return this._hoverovercolor; }
            set { this._hoverovercolor = value; this.OnPropertyChanged(this, "HoverOverColorBrush"); }
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
            this._height = Double.NaN;
            this._width = Double.NaN;
            this._margin = new Thickness(2, 2, 2, 2);
            this._padding = new Thickness(2, 2, 2, 2);
            this._verticalalign = VerticalAlignment.Bottom;
            this._horizontalalign = HorizontalAlignment.Left;
            this._bordercolor = new SolidColorBrush();
            this._hoverovercolor = new SolidColorBrush();
            this.BorderColorBrush.Color = (Color)ColorConverter.ConvertFromString("#FFABADB3");
            this.HoverOverColorBrush.Color = (Color)ColorConverter.ConvertFromString("#FF3399FF");
        }

        public void LoadXml(XElement InputXml)
        {
            //Load the XML
            #region
            this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);
            this.Padding = XmlHandler.GetThicknessFromXElement(InputXml, "Padding", 2);
            this.Margin = XmlHandler.GetThicknessFromXElement(InputXml, "Margin", 2);
            #endregion
        }

        public Formatting Clone()
        {
            Formatting f = new Formatting();
            f.Width = this.Width;
            f.Height = this.Height;
            f.Padding = this.Padding;
            f.Margin = this.Margin;
            f.HorizontalAlignment = this.HorizontalAlignment;
            f.VerticalAlignment = this.VerticalAlignment;

            return f;
        }
    }
}
