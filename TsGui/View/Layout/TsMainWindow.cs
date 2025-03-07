﻿#region license
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

// TsMainWindow.cs - view model for the MainWindow

using System;
using System.Xml.Linq;
using System.Windows;

using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsMainWindow : ParentLayoutElement
    {
        private string _windowTitle;
        private Thickness _pageMargin = new Thickness(0, 0, 0, 0);
        private string _footerText;
        private double _footerHeight;
        private bool _topmost = true;
        private HorizontalAlignment _footerHAlignment;
        private MainWindow _parentwindow;

        //values for configured height and width. These can be overridden, so need to be saved. 
        private double _configuredHeight = double.NaN;
        private double _configuredWidth = double.NaN;

        //Properties
        public Border Border { get; private set; } = new Border();
        public WindowLocation WindowLocation { get; private set; }
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
        public TsMainWindow(MainWindow Parent)
        {
            this.Init(Parent);
        }

        public TsMainWindow(MainWindow Parent, XElement Config)
        {
            this.Init(Parent);
            this.LoadXml(Config);
        }

        public void Init(MainWindow ParentWindow)
        {
            this._parentwindow = ParentWindow;
            this.WindowLocation = new WindowLocation(this._parentwindow);

            //set default values
            this.WindowTitle = "TsGui";
            this.Style.Width = Double.NaN;
            this.Style.Height = Double.NaN;
            this.FooterText = "Powered by TsGui - www.20road.com";
            this.FooterHeight = 15;
            this.FooterHAlignment = HorizontalAlignment.Right;
        }

        public override void LoadXml(XElement InputXml, ParentLayoutElement parent)
        {
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement subx;
            XElement x;

            if (InputXml != null)
            {
                x = InputXml.Element("Footer");
                if (x != null)
                {
                    subx = x.Element("Text");
                    if (subx != null) { this.FooterText = subx.Value; }

                    subx = x.Element("Height");
                    if (subx != null) { this.FooterHeight = Convert.ToInt32(subx.Value); }

                    GuiFactory.LoadHAlignment(x, ref this._footerHAlignment);
                }

                this.Border.LoadXml(InputXml.Element("Border"));

                this.TopMost = XmlHandler.GetBoolFromXml(InputXml, "TopMost", this.TopMost);
                this.WindowTitle = XmlHandler.GetStringFromXml(InputXml, "Title", this.WindowTitle);

                x = InputXml.Element("WindowLocation");
                if (x != null) { this.WindowLocation.LoadXml(x); }

                GuiFactory.LoadMargins(InputXml, this._pageMargin);
            }

            this._configuredHeight = this.Style.Height;
            this._configuredWidth = this.Style.Width;
        }

        /// <summary>
        /// Set a temporary height for the Window. If you don't want to set one of the values, pass double.NaN.
        /// The NaN will be reset to the default value set during configuration.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public void ChangeSize(double height, double width)
        {
            if (double.IsNaN(height) == false) { this.Style.Height = height; }
            else { this.Style.Height = this._configuredHeight; }

            if (double.IsNaN(width) == false) { this.Style.Width = width; }  
            else { this.Style.Width = this._configuredWidth; }
        }
    }
}
