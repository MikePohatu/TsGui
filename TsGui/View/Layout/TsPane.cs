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

// TsPane.cs - view model class for a pane (right/left) on a page

using System.Xml.Linq;
using System.Windows;
using TsGui.Images;
using Core;

namespace TsGui.View.Layout
{
    public class TsPane: ViewModelBase
    {
        private double _width;
        private double _height;
        private TsPaneUI _paneui;
        private Image _image;


        public Style Style { get; set; }
        public Image Image
        {
            get { return this._image; }
            set { this._image = value; this.OnPropertyChanged(this, "Image"); }
        }
        public TsPaneUI PaneUI
        {
            get { return this._paneui; }
            set { this._paneui = value; this.OnPropertyChanged(this, "PaneUI"); }
        }
        public double Width
        {
            get { return this._width; }
            set { this._width = value; this.OnPropertyChanged(this, "Width"); }
        }
        public double Height
        {
            get { return this._height; }
            set { this._height = value; this.OnPropertyChanged(this, "Height"); }
        }

        //Constructor
        public TsPane()
        {
            this.Init();         
        }

        public TsPane(XElement InputXml)
        {
            this.Init();
            this.LoadXml(InputXml);
        }

        private void Init()
        {
            this.PaneUI = new TsPaneUI();
            this.PaneUI.DataContext = this;
            this.Style = new Style();
            this.SetDefaults();
        }

        public void LoadXml(XElement InputXml)
        {
            XElement x;
            this.Height = XmlHandler.GetDoubleFromXml(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetDoubleFromXml(InputXml, "Width", this.Width);

            x = InputXml.Element("Style");
            if (x != null) { this.Style.LoadXml(x); }

            x = InputXml.Element("Image");
            if (x != null) { this.Image = new Image(x); }
        }

        private void SetDefaults()
        {
            this.Width = 0;
            this.Height = 0;
            this.Style.Padding = new Thickness(0);
            this.Style.Margin = new Thickness(0);
        }
    }
}
