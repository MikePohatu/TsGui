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

// TsPageHeader.cs - view model class for the header on a page

using System.Xml.Linq;
using System.Windows.Media;
using System.Windows;

using TsGui.Events;
using TsGui.Images;
using TsGui.Validation;

namespace TsGui.View.Layout
{
    public class TsPageHeader : BaseLayoutElement, IRootLayoutElement
    {
        public event ComplianceRetryEventHandler ComplianceRetry;

        private string _title;
        private string _text;
        private SolidColorBrush _bgColor;
        private SolidColorBrush _fontColor;

        //Properties
        public Image Image { get; set; }
        public TsTable Table { get; set; }
        public TsPageHeaderUI UI { get; set; }
        public SolidColorBrush BgColor
        {
            get { return this._bgColor; }
            set
            {
                this._bgColor = value;
                this.OnPropertyChanged(this, "BgColor");
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
        public TsPageHeader(BaseLayoutElement Parent, TsPageHeader Template, XElement SourceXml, MainController MainController):base(Parent, MainController)
        {
            this.Height = Template.Height;
            this.Title = Template.Title;
            this.Text = Template.Text;
            this.FontColor = Template.FontColor;
            this.BgColor = Template.BgColor;
            this.Image = Template.Image;

            this.Init(SourceXml, MainController);
        }

        public TsPageHeader(XElement SourceXml, MainController MainController): base (MainController)
        {
            this.ShowGridLines = _controller.ShowGridLines;
            this.SetDefaults();

            this.Init(SourceXml, MainController);
        }

        public TsPageHeader(MainController MainController) : base(MainController)
        {
            this.SetDefaults();
        }

        public bool OptionsValid()
        {
            if (this.Table != null) { return ResultValidator.OptionsValid(this.Table.ValidationOptions); }
            else { return true; }
        }

        private void SetDefaults()
        {
            this.Height = 50;
            this.FontColor = new SolidColorBrush(Colors.White);
            this.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006AD4"));
        }

        private void Init (XElement SourceXml, MainController MainController)
        {
            this.UI = new TsPageHeaderUI();
            this.UI.DataContext = this;
            
            this.LoadXml(SourceXml);

            if (string.IsNullOrEmpty(this.Title)) { this.UI.HeaderTitle.Visibility = Visibility.Collapsed; }
            if (string.IsNullOrEmpty(this.Text)) { this.UI.HeaderText.Visibility = Visibility.Collapsed; }
            if (this.Image == null) { this.UI.ImageElement.Visibility = Visibility.Collapsed; }
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            XElement x;

            if (InputXml != null)
            {
                base.LoadXml(InputXml);

                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Font-Color", this.FontColor);
                this.BgColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Bg-Color", this.BgColor);
                this.Title = XmlHandler.GetStringFromXElement(InputXml, "Title", this.Title);
                this.Text = XmlHandler.GetStringFromXElement(InputXml, "Text", this.Text);
                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "TextColor", this.FontColor);
                this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);

                x = InputXml.Element("Image");
                if (x != null) { this.Image = new Image(x, this._controller); }

                x = InputXml.Element("Row");
                if (x != null)
                { this.Table = new TsTable(InputXml, this, this._controller); }
            }
        }

        public void RaiseComplianceRetryEvent()
        {
            this.ComplianceRetry?.Invoke(this, new RoutedEventArgs());
        }
    }
}
