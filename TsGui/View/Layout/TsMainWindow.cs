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

// TsMainWindow.cs - view model for the MainWindow

using System;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsMainWindow : BaseLayoutElement
    {
        private double _height;        //default page height for the window
        private double _width;         //default page width for the window
        private string _windowTitle;
        private Thickness _pageMargin = new Thickness(0, 0, 0, 0);
        private string _footerText;
        private double _footerHeight;
        private bool _topmost = true;
        private HorizontalAlignment _footerHAlignment;
        private bool _gridlines = false;
        private WindowLocation _windowlocation;
        private MainWindow _parentwindow;

        //Properties
        public WindowLocation WindowLocation { get { return this._windowlocation; } }
        public string WindowTitle
        {
            get { return this._windowTitle; }
            set
            {
                this._windowTitle = value;
                this.OnPropertyChanged(this, "WindowTitle");
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
        public bool TopMost
        {
            get { return this._topmost; }
            set
            {
                this._topmost = value;
                this.OnPropertyChanged(this, "TopMost");
            }
        }

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

        //Constructors
        public TsMainWindow(MainWindow ParentWindow)
        {
            this._parentwindow = ParentWindow;
            this._windowlocation = new WindowLocation(this._parentwindow);

            //set default values
            this.WindowTitle = "TsGui";
            this._width = Double.NaN;
            this._height = Double.NaN;
            this.FooterText = "Powered by TsGui - www.20road.com";
            this.FooterHeight = 15;
            this.FooterHAlignment = HorizontalAlignment.Right;
            
        }

        public new void LoadXml(XElement SourceXml)
        {
            base.LoadXml(SourceXml);
            XElement subx;
            XElement x;

            if (SourceXml != null)
            {
                x = SourceXml.Element("Footer");
                if (x != null)
                {
                    subx = x.Element("Text");
                    if (subx != null) { this.FooterText = subx.Value; }

                    subx = x.Element("Height");
                    if (subx != null) { this.FooterHeight = Convert.ToInt32(subx.Value); }

                    GuiFactory.LoadHAlignment(x, ref this._footerHAlignment);
                }

                this.TopMost = XmlHandler.GetBoolFromXElement(SourceXml, "TopMost", this.TopMost);
                this.WindowTitle = XmlHandler.GetStringFromXElement(SourceXml, "Title", this.WindowTitle);

                x = SourceXml.Element("WindowLocation");
                if (x != null) { this._windowlocation.LoadXml(x); }

                GuiFactory.LoadMargins(SourceXml, this._pageMargin);
            }
        }
    }
}
