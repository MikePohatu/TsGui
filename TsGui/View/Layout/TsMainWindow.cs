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

// TsMainWindow.cs - view model for the MainWindow

using System;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsMainWindow : INotifyPropertyChanged
    {
        private double _height;        //default page height for the window
        private double _width;         //default page width for the window
        private string _headingTitle;
        private string _headingText;
        private int _headingHeight;
        private Thickness _pageMargin = new Thickness(0, 0, 0, 0);
        private string _footerText;
        private double _footerHeight;
        private HorizontalAlignment _footerHAlignment;
        private bool _gridlines = false;
        private WindowLocation _positioning = new WindowLocation();

        //Properties
        public WindowLocation Positioning { get { return this._positioning; } }
        public string HeadingText { get { return this._headingText; } }
        public int HeadingHeight { get { return this._headingHeight; } }
        public string HeadingTitle
        {
            get { return this._headingTitle; }
            set
            {
                this._headingTitle = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }
        public double Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "Height");
            }
        }
        public double Width
        {
            get { return this._width; }
            set
            {
                this._width = value;
                this.OnPropertyChanged(this, "Width");
            }
        }
        public Thickness PageMargin
        {
            get { return this._pageMargin; }
            set
            {
                this._pageMargin = value;
                this.OnPropertyChanged(this, "PageMargin");
            }
        }
        public SolidColorBrush HeadingBgColor { get; set; }
        public SolidColorBrush HeadingFontColor { get; set; }

        public double FooterHeight
        {
            get { return this._footerHeight; }
            set
            {
                this._footerHeight = value;
                this.OnPropertyChanged(this, "FooterHeight");
            }
        }
        public string FooterText
        {
            get { return this._footerText; }
            set
            {
                this._footerText = value;
                this.OnPropertyChanged(this, "FooterText");
            }
        }
        public HorizontalAlignment FooterHAlignment
        {
            get { return this._footerHAlignment; }
            set
            {
                this._footerHAlignment = value;
                this.OnPropertyChanged(this, "FooterHAlignment");
            }
        }
        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this._gridlines = value;
                this.OnPropertyChanged(this, "ShowGridLines");
            }
        }

        //Constructors
        public TsMainWindow()
        {
            //set default values
            this._width = Double.NaN;
            this._height = Double.NaN;
            this._headingHeight = 50;
            this.HeadingTitle = "TsGui";
            this.HeadingFontColor = new SolidColorBrush(Colors.White);
            this.HeadingBgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006AD4"));
            this.FooterText = "Powered by TsGui - www.20road.com";
            this.FooterHeight = 15;
            this.FooterHAlignment = HorizontalAlignment.Right;
        }

        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
        }

        public void LoadXml(XElement SourceXml)
        {

            XElement subx;
            XElement x;

            if (SourceXml != null)
            {

                x = SourceXml.Element("Heading");
                if (x != null)
                {
                    subx = x.Element("Title");
                    if (subx != null) { this.HeadingTitle = subx.Value; }

                    subx = x.Element("Text");
                    if (subx != null) { this._headingText = subx.Value; }

                    subx = x.Element("Height");
                    if (subx != null) { this._headingHeight = Convert.ToInt32(subx.Value); }

                    subx = x.Element("Bg-Color");
                    if (subx != null)
                    {
                        this.HeadingBgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(subx.Value));
                    }

                    subx = x.Element("Font-Color");
                    if (subx != null)
                    {
                        this.HeadingFontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(subx.Value));
                    }

                }

                x = SourceXml.Element("Footer");
                if (x != null)
                {
                    subx = x.Element("Text");
                    if (subx != null) { this.FooterText = subx.Value; }

                    subx = x.Element("Height");
                    if (subx != null) { this.FooterHeight = Convert.ToInt32(subx.Value); }

                    GuiFactory.LoadHAlignment(x, ref this._footerHAlignment);
                }

                x = SourceXml.Element("WindowLocation");
                if (x != null) { this._positioning.LoadXml(x); }

                this.Width = XmlHandler.GetDoubleFromXElement(SourceXml, "Width", this.Width);
                this.Height = XmlHandler.GetDoubleFromXElement(SourceXml, "Height", this.Height);
                GuiFactory.LoadMargins(SourceXml, this._pageMargin);

                //Set show grid lines after pages and columns have been created.
                x = SourceXml.Element("ShowGridLines");
                if (x != null)
                { this.ShowGridLines = true; }
            }
        }
    }
}
