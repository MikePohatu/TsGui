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

// TsPageHeader.cs - view model class for the heading on a page



using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Media;

namespace TsGui.View.Layout
{
    public class TsPageHeader: INotifyPropertyChanged
    {
        private double _height;
        private string _title;
        private string _text;
        private SolidColorBrush _bgColor;
        private SolidColorBrush _fontColor;

        //Properties
        public TsTable Table { get; set; }
        public SolidColorBrush BgColor
        {
            get { return this._bgColor; }
            set
            {
                this._bgColor = value;
                this.OnPropertyChanged(this, "HeadingBgColor");
            }
        }
        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }
        public string Text
        {
            get { return this._text; }
            set
            {
                this._text = value;
                this.OnPropertyChanged(this, "HeadingText");
            }
        }
        public double Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "HeadingHeight");
            }
        }

        public SolidColorBrush FontColor
        {
            get { return this._fontColor; }
            set
            {
                this._fontColor = value;
                this.OnPropertyChanged(this, "HeadingFontColor");
            }
        }

        //Constructors
        public TsPageHeader(TsPageHeader Template, XElement SourceXml, MainController MainController)
        {
            this.Height = Template.Height;
            this.Title = Template.Title;
            this.Text = Template.Text;
            this.FontColor = Template.FontColor;
            this.BgColor = Template.BgColor;

            this.Init(SourceXml, MainController);
        }

        public TsPageHeader(XElement SourceXml, MainController MainController)
        {
            //set default values
            this.Height = 50;
            this.Title = "TsGui";
            this.FontColor = new SolidColorBrush(Colors.White);
            this.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006AD4"));
            this.Init(SourceXml, MainController);
        }

        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }

        private void Init (XElement SourceXml, MainController MainController)
        {
            //this.ShowGridLines = MainController.ShowGridLines;
            this.LoadXml(SourceXml);
        }

        //Methods
        public void LoadXml(XElement InputXml)
        {
            //base.LoadXml(InputXml);
            IEnumerable<XElement> xlist;

            if (InputXml != null)
            {
                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Font-Color", this.FontColor);
                this.BgColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Bg-Color", this.BgColor);
                this.Title = XmlHandler.GetStringFromXElement(InputXml, "Title", this.Title);
                this.Text = XmlHandler.GetStringFromXElement(InputXml, "Text", this.Text);
                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "TextColor", this.FontColor);
                this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);

                xlist = InputXml.Elements("Row");
                //if (xlist != null) { this.Table = new TsTable(xlist,null,this._con)}
            }

            

        }
    }
}
