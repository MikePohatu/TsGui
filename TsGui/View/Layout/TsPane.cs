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

// TsPane.cs - view model class for a pane (right/left) on a page

using System.Xml.Linq;
using System.Windows;


using TsGui.Images;

namespace TsGui.View.Layout
{
    public class TsPane: ViewModelBase
    {
        private IDirector _controller;
        private double _width;
        private double _height;
        private TsPaneUI _paneui;
        private Image _image;


        public Formatting Formatting { get; set; }
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
        public TsPane(IDirector MainController)
        {
            this.Init(MainController);         
        }

        public TsPane(XElement InputXml, IDirector MainController)
        {
            this.Init(MainController);
            this.LoadXml(InputXml);
        }

        private void Init(IDirector MainController)
        {
            this._controller = MainController;
            this.PaneUI = new TsPaneUI();
            this.PaneUI.DataContext = this;
            this.Formatting = new Formatting();
            this.SetDefaults();
        }

        public void LoadXml(XElement InputXml)
        {
            XElement x;
            this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);

            x = InputXml.Element("Formatting");
            if (x != null) { this.Formatting.LoadXml(x); }

            x = InputXml.Element("Image");
            if (x != null) { this.Image = new Image(x, this._controller); }
        }

        private void SetDefaults()
        {
            this.Width = 0;
            this.Height = 0;
            this.Formatting.Padding = new Thickness(0);
            this.Formatting.Margin = new Thickness(0);
        }
    }
}
