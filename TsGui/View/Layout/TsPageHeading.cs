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

// TsPageHeading.cs - view model class for the heading on a page




using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using TsGui.Grouping;
using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsPageHeading : BaseLayoutElement
    {
        private double _headingHeight;
        private string _headingTitle;
        private string _headingText;
        private SolidColorBrush _headingBgColor;
        private SolidColorBrush _headingFontColor;
        private TsTable _table;

        //Properties
        public TsPageHeadingUI HeadingUI { get; set; }
        public SolidColorBrush HeadingBgColor
        {
            get { return this._headingBgColor; }
            set
            {
                this._headingBgColor = value;
                this.OnPropertyChanged(this, "HeadingBgColor");
            }
        }
        public string HeadingTitle
        {
            get { return this._headingTitle; }
            set
            {
                this._headingTitle = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }
        public string HeadingText
        {
            get { return this._headingText; }
            set
            {
                this._headingText = value;
                this.OnPropertyChanged(this, "HeadingText");
            }
        }
        public double HeadingHeight
        {
            get { return this._headingHeight; }
            set
            {
                this._headingHeight = value;
                this.OnPropertyChanged(this, "HeadingHeight");
            }
        }

        public SolidColorBrush HeadingFontColor
        {
            get { return this._headingFontColor; }
            set
            {
                this._headingFontColor = value;
                this.OnPropertyChanged(this, "HeadingFontColor");
            }
        }
        //Constructors
        public TsPageHeading(XElement SourceXml, PageDefaults Defaults, MainController MainController) : base(MainController)
        {
            this.HeadingUI = new TsPageHeadingUI();
            this.HeadingUI.DataContext = this;
            this.ShowGridLines = MainController.ShowGridLines;
            this.HeadingHeight = 40;
            this.HeadingTitle = Defaults.HeadingTitle;
            this.HeadingText = Defaults.HeadingText;
            this.HeadingFontColor = Defaults.HeadingFontColor;
            this.HeadingBgColor = Defaults.HeadingBgColor;
            //this._table = new TsTable(SourceXml, this, MainController);
            this.LoadXml(SourceXml);
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement x;

            if (InputXml != null)
            {
                this._headingTitle = XmlHandler.GetStringFromXElement(InputXml, "Title", this._headingTitle);
                this._headingText = XmlHandler.GetStringFromXElement(InputXml, "Text", this._headingText);
                this._headingHeight = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this._headingHeight);

                x = InputXml.Element("BgColor");
                if (x != null)
                {
                    this.HeadingBgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(x.Value));
                }

                x = InputXml.Element("TextColor");
                if (x != null)
                {
                    this.HeadingFontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(x.Value));
                }
            }


        }
    }
}
