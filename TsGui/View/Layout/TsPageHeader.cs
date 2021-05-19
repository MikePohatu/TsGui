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

// TsPageHeader.cs - view model class for the header on a page

using System.Xml.Linq;
using System.Windows.Media;
using System.Windows;

using TsGui.Events;
using TsGui.Images;
using TsGui.Validation;

namespace TsGui.View.Layout
{
    public class TsPageHeader : ParentLayoutElement, IComplianceRoot
    {
        public event ComplianceRetryEventHandlerAsync ComplianceRetry;

        private SolidColorBrush _bgColor;
        private SolidColorBrush _fontColor;
        private double _titlefontsize;
        private double _textfontsize;

        //Properties
        private Image _image;
        public Image Image
        {
            get { return this._image; }
            set
            {
                this._image = value;
                this.OnPropertyChanged(this, "Image");
            }
        }

        private TsTable _table;
        public TsTable Table
        {
            get { return this._table; }
            set
            {
                this._table = value;
                this.OnPropertyChanged(this, "Table");
            }
        }
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

        private string _title;
        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }

        private string _text;
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

        public double TitleFontSize
        {
            get { return this._titlefontsize; }
            set  { this._titlefontsize = value; this.OnPropertyChanged(this, "TitleFontSize"); }
        }

        public double TextFontSize
        {
            get { return this._textfontsize; }
            set { this._textfontsize = value; this.OnPropertyChanged(this, "TextFontSize"); }
        }

        //Constructors
        public TsPageHeader(ParentLayoutElement Parent, TsPageHeader Template, XElement SourceXml):base(Parent)
        {
            this.ShowGridLines = Director.Instance.ShowGridLines;
            this.SetDefaults();

            this.Formatting.Height = Template.Formatting.Height;
            this.Title = Template.Title;
            this.Text = Template.Text;
            this.FontColor = Template.FontColor;
            this.BgColor = Template.BgColor;
            this.Image = Template.Image;
            this.TitleFontSize = Template.TitleFontSize;
            this.TextFontSize = Template.TextFontSize;
            this.Formatting.Margin = Template.Formatting.Margin;

            this.Init(SourceXml);
        }

        public TsPageHeader(ParentLayoutElement Parent, XElement SourceXml): base (Parent)
        {
            this.ShowGridLines = Director.Instance.ShowGridLines;
            this.SetDefaults();

            this.Init(SourceXml);
        }

        public TsPageHeader() : base()
        {
            this.SetDefaults();
        }

        private void Init(XElement SourceXml)
        {
            this.UI = new TsPageHeaderUI();
            this.UI.DataContext = this;

            this.LoadXml(SourceXml);

            if (string.IsNullOrEmpty(this.Title)) { this.UI.HeaderTitle.Visibility = Visibility.Collapsed; }
            if (string.IsNullOrEmpty(this.Text)) { this.UI.HeaderText.Visibility = Visibility.Collapsed; }
            if (this.Image == null) { this.UI.ImageElement.Visibility = Visibility.Collapsed; }
        }

        public bool OptionsValid()
        {
            if (this.Table != null) { return ResultValidator.OptionsValid(this.Table.ValidationOptions); }
            else { return true; }
        }

        private void SetDefaults()
        {
            if (Director.Instance.UseTouchDefaults)
            {
                this.Formatting.Margin = new Thickness(10, 10, 10, 10);
                this.Formatting.Height = 65;
                this.TitleFontSize = 14;
                this.TextFontSize = 12;
            }
            else
            {
                this.Formatting.Margin = new Thickness(10, 5, 10, 5);
                this.Formatting.Height = 50;
                this.TitleFontSize = 13;
                this.TextFontSize = 12;
            }
            
            this.FontColor = new SolidColorBrush(Colors.White);
            this.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006AD4"));
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            this.LoadXml(InputXml, this);
        }

        public override void LoadXml(XElement InputXml, ParentLayoutElement parent)
        {
            XElement x;

            if (InputXml != null)
            {
                base.LoadXml(InputXml);

                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Font-Color", this.FontColor);
                this.BgColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Bg-Color", this.BgColor);
                this.Title = XmlHandler.GetStringFromXElement(InputXml, "Title", this.Title);
                this.Text = XmlHandler.GetStringFromXElement(InputXml, "Text", this.Text);
                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "TextColor", this.FontColor);
                this.Formatting.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Formatting.Height);

                x = InputXml.Element("Image");
                if (x != null) { this.Image = new Image(x); }

                x = InputXml.Element("Row");
                if (x != null)
                { this.Table = new TsTable(InputXml, this); }
            }
        }

        public void RaiseComplianceRetryEvent()
        {
            this.ComplianceRetry?.Invoke(this, new RoutedEventArgs());
        }
    }
}
