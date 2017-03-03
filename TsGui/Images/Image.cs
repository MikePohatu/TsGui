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

// Image.cs - view model class for the image on a page

using System.Xml.Linq;
using System.Windows.Media;
using TsGui.View;

namespace TsGui.Images
{
    public class Image : ViewModelBase
    {
        private Stretch _stretchmode;
        private double _width;
        private double _height;
        private MultiImage _multiimage;
        private MainController _controller;
        private string _file;

        public string File
        {
            get { return this._file; }
            set { this._file = value; this.OnPropertyChanged(this, "File"); }
        }

        public MultiImage MultiImage
        {
            get { return this._multiimage; }
            set { this._multiimage = value; this.OnPropertyChanged(this, "MultiImage"); }
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
        //https://msdn.microsoft.com/en-us/library/system.windows.media.stretch(v=vs.110).aspx
        public Stretch StretchMode
        {
            get { return this._stretchmode; }
            set { this._stretchmode = value; this.OnPropertyChanged(this, "StretchMode"); }
        }

        //constructors
        public Image(XElement InputXml, MainController MainController)
        {
            this._controller = MainController;
            this.SetDefaults();
            this.LoadXml(InputXml);
        }

        public Image(MainController MainController)
        {
            this._controller = MainController;
            this.SetDefaults();
        }

        //Methods
        public void LoadXml(XElement InputXml)
        {
            if (InputXml != null)
            {
                this._file = XmlHandler.GetStringFromXElement(InputXml, "File", string.Empty);
                this.MultiImage = new MultiImage(_file, this._controller);
                this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);
                this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);
                this.StretchMode = XmlHandler.GetStretchFromXElement(InputXml, "Stretch", this.StretchMode);
            }

        }

        private void SetDefaults()
        {
            this.Height = double.NaN;
            this.Width = double.NaN;
            this.StretchMode = Stretch.UniformToFill;
        }
    }
}
