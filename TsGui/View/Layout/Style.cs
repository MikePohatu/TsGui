#region license
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

// Style.cs - view model for the layout of GuiOptions. Controls the layout and 
// formatting options for the associated controls

using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;
using Core;
using Core.Diagnostics;

namespace TsGui.View.Layout
{
    public class Style: ViewModelBase
    {
        /// <summary>
        /// record the elements set by the user in the XML
        /// </summary>
        private Dictionary<string, bool> _setElements = new Dictionary<string, bool>();


        //Properties
        public Border Border { get; set; } = new Border();

        #region 
        private string _fontweight;
        public string FontWeight
        {
            get { return this._fontweight; }
            set { this._fontweight = value; this.OnPropertyChanged(this, "FontWeight"); }
        }

        private string _fontstyle;
        public string FontStyle
        {
            get { return this._fontstyle; }
            set { this._fontstyle = value; this.OnPropertyChanged(this, "FontStyle"); }
        }

        private double _fontsize;
        public double FontSize
        {
            get { return this._fontsize; }
            set { this._fontsize = value; this.OnPropertyChanged(this, "FontSize"); }
        }

        private double _cornerradius;
        public double CornerRadius
        {
            get { return this._cornerradius; }
            set { this._cornerradius = value; this.OnPropertyChanged(this, "Rounding"); }
        }

        private double _height;
        public double Height
        {
            get { return this._height; }
            set { this._height = value; this.OnPropertyChanged(this, "Height"); }
        }

        private double _width;
        public double Width
        {
            get { return this._width; }
            set { this._width = value; this.OnPropertyChanged(this, "Width"); }
        }

        private Thickness _margin;
        public Thickness Margin
        {
            get { return this._margin; }
            set { this._margin = value; this.OnPropertyChanged(this, "Margin"); }
        }

        private Thickness _padding;
        public Thickness Padding
        {
            get { return this._padding; }
            set { this._padding = value; this.OnPropertyChanged(this, "Padding"); }
        }

        private Thickness _borderthickness;
        public Thickness BorderThickness
        {
            get { return this._borderthickness; }
            set { this._borderthickness = value; this.OnPropertyChanged(this, "BorderThickness"); }
        }

        private VerticalAlignment _verticalalign;
        public VerticalAlignment VerticalAlignment
        {
            get { return this._verticalalign; }
            set { this._verticalalign = value; this.OnPropertyChanged(this, "VerticalAlignment"); }
        }

        private HorizontalAlignment _horizontalalign;
        public HorizontalAlignment HorizontalAlignment
        {
            get { return this._horizontalalign; }
            set { this._horizontalalign = value; this.OnPropertyChanged(this, "HorizontalAlignment"); }
        }

        private VerticalAlignment _verticalcontentalign;
        public VerticalAlignment VerticalContentAlignment
        {
            get { return this._verticalcontentalign; }
            set { this._verticalcontentalign = value; this.OnPropertyChanged(this, "VerticalContentAlignment"); }
        }

        private HorizontalAlignment _horizontcontentalalign;
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return this._horizontcontentalalign; }
            set { this._horizontcontentalalign = value; this.OnPropertyChanged(this, "HorizontalContentAlignment"); }
        }

        private TextAlignment _textalign;
        public TextAlignment TextAlignment
        {
            get { return this._textalign; }
            set { this._textalign = value; this.OnPropertyChanged(this, "TextAlignment"); }
        }

        private SolidColorBrush _bordercolorbrush;
        public SolidColorBrush BorderBrush
        {
            get { return this._bordercolorbrush; }
            set { this._bordercolorbrush = value; this.OnPropertyChanged(this, "BorderBrush"); }
        }

        private SolidColorBrush _focusedcolorbrush;
        public SolidColorBrush FocusedBorderBrush
        {
            get { return this._focusedcolorbrush; }
            set { this._focusedcolorbrush = value; this.OnPropertyChanged(this, "FocusedBorderBrush"); }
        }

        private SolidColorBrush _mouseovercolorbrush;
        public SolidColorBrush MouseOverBorderBrush
        {
            get { return this._mouseovercolorbrush; }
            set { this._mouseovercolorbrush = value; this.OnPropertyChanged(this, "MouseOverBorderBrush"); }
        }

        private SolidColorBrush _fontcolorbrush;
        public SolidColorBrush FontColorBrush
        {
            get { return this._fontcolorbrush; }
            set { this._fontcolorbrush = value; this.OnPropertyChanged(this, "FontColorBrush"); }
        }
        #endregion

        //Constructor 
        public Style()
        {
            this.SetDefaults();
        }

        public Style(XElement InputXml)
        {
            this.SetDefaults();
            this.LoadXml(InputXml);
        }

        private void SetDefaults()
        {
            if (Director.Instance.UseTouchDefaults==true)
            {
                this.FontSize = 12;
            }
            else
            {
                this.FontSize = 11;
            }
            this.BorderThickness = new Thickness(1);
            this.FontStyle = "Normal";
            this.FontWeight = "Normal";
            this.CornerRadius = 0;
            
            this.Height = Double.NaN;
            this.Width = Double.NaN;
            
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.HorizontalContentAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.TextAlignment = TextAlignment.Left;
            this.BorderBrush = new SolidColorBrush(Colors.Gray);
            this.MouseOverBorderBrush = new SolidColorBrush(Colors.DarkGray);
            this.FocusedBorderBrush = new SolidColorBrush(Colors.LightBlue);
            this.FontColorBrush = new SolidColorBrush(Colors.Black);
        }

        public void LoadXml(XElement InputXml)
        {
            this.LoadXml(InputXml, true);
        }

        protected void LoadXml(XElement InputXml, bool processimports)
        {
            //Load the XML
            #region
            XElement x;
            this.Border.LoadXml(InputXml.Element("Border"));

            this.Height = XmlHandler.GetDoubleFromXml(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetDoubleFromXml(InputXml, "Width", this.Width);
            this.CornerRadius = XmlHandler.GetDoubleFromXml(InputXml, "CornerRadius", this.CornerRadius);
            this.Padding = XmlHandler.GetThicknessFromXml(InputXml, "Padding", this.Padding);
            this.Margin = XmlHandler.GetThicknessFromXml(InputXml, "Margin", this.Margin);
            this.VerticalAlignment = XmlHandler.GetVerticalAlignmentFromXml(InputXml, "VerticalAlignment", this.VerticalAlignment);
            this.HorizontalAlignment = XmlHandler.GetHorizontalAlignmentFromXml(InputXml, "HorizontalAlignment", this.HorizontalAlignment);
            this.VerticalContentAlignment = XmlHandler.GetVerticalAlignmentFromXml(InputXml, "VerticalContentAlignment", this.VerticalContentAlignment);
            this.HorizontalContentAlignment = XmlHandler.GetHorizontalAlignmentFromXml(InputXml, "HorizontalContentAlignment", this.HorizontalContentAlignment);
            this.TextAlignment = XmlHandler.GetTextAlignmentFromXml(InputXml, "TextAlignment", this.TextAlignment);


            //record the set elements
            foreach (var el in InputXml.Elements())
            {
                string name = el.Name.ToString();
                this.RecordSetElement(name);
            }
            foreach (var a in InputXml.Attributes())
            {
                string name = a.Name.ToString();
                this.RecordSetElement(name);
            }

            x = InputXml.Element("Font");
            if (x != null)
            {
                this.FontWeight = XmlHandler.GetStringFromXml(x, "Weight", this.FontWeight);
                this.FontStyle = XmlHandler.GetStringFromXml(x, "Style", this.FontStyle);
                this.FontSize = XmlHandler.GetDoubleFromXml(x, "Size", this.FontSize);
                this.FontColorBrush = XmlHandler.GetSolidColorBrushFromXml(x, "Color", this.FontColorBrush);

                //record the set elements
                foreach (var el in x.Elements())
                {
                    string name = "Font" + el.Name.ToString();
                    this.RecordSetElement(name);
                }
                foreach (var a in x.Attributes())
                {
                    string name = "Font" + a.Name.ToString();
                    this.RecordSetElement(name);
                }

            }

            if (processimports)
            {
                string styleids = XmlHandler.GetStringFromXml(InputXml, "Import", null);
                if (string.IsNullOrWhiteSpace(styleids) == false)
                {
                    foreach (string id in styleids.Trim().Split(' '))
                    {
                        StyleTree s = StyleLibrary.Get(id);
                        this.Import(s);
                    }
                }
            }
            #endregion
        }

        private void RecordSetElement(string name)
        {
            if (this._setElements.ContainsKey(name) == false)
            {
                this._setElements.Add(name, true);
            }
        }

        public Style Clone() 
        {
            Style s = new Style();
            CloneTo(this, s);
            return s;
        }

        public static void CloneTo(Style source, Style target)
        {
            target.FontWeight = source.FontWeight;
            target.FontStyle = source.FontStyle;
            target.FontSize = source.FontSize;
            target.FontColorBrush = source.FontColorBrush.Clone();

            target.Width = source.Width;
            //target.Height = source.Height; // we don't clone this one because of how it effects the tree e.g. different layers behave differently
            target.CornerRadius = source.CornerRadius;
            target.Padding = source.Padding;
            target.Margin = source.Margin;
            target.HorizontalAlignment = source.HorizontalAlignment;
            target.VerticalAlignment = source.VerticalAlignment;
            target.HorizontalContentAlignment = source.HorizontalContentAlignment;
            target.VerticalContentAlignment = source.VerticalContentAlignment;
            target.TextAlignment = source.TextAlignment;
            target.BorderBrush = source.BorderBrush.Clone();
            target.FocusedBorderBrush = source.FocusedBorderBrush.Clone();
            target.MouseOverBorderBrush = source.MouseOverBorderBrush.Clone();
        }

        /// <summary>
        /// Import a Style, only setting elements that have been set in the config
        /// </summary>
        /// <param name="from"></param>
        /// <exception cref="KnownException"></exception>
        public void Import(Style from)
        {
            if (from == null) { throw new KnownException("Cannot import from null Style. Check for correct ID", null); }

            if (from._setElements.ContainsKey("FontWeight")) { this.FontWeight = from.FontWeight; }
            if (from._setElements.ContainsKey("FontStyle")) { this.FontStyle = from.FontStyle; }
            if (from._setElements.ContainsKey("FontSize")) { this.FontSize = from.FontSize; }
            if (from._setElements.ContainsKey("FontColor")) { this.FontColorBrush = from.FontColorBrush.Clone(); }

            if (from._setElements.ContainsKey("Width")) { this.Width = from.Width; }
            if (from._setElements.ContainsKey("Height")) { this.Height = from.Height; }
            if (from._setElements.ContainsKey("CornerRadius")) { this.CornerRadius = from.CornerRadius; }
            if (from._setElements.ContainsKey("Padding")) { this.Padding = from.Padding; }
            if (from._setElements.ContainsKey("Margin")) { this.Margin = from.Margin; }
            if (from._setElements.ContainsKey("HorizontalAlignment")) { this.HorizontalAlignment = from.HorizontalAlignment; }
            if (from._setElements.ContainsKey("VerticalAlignment")) { this.VerticalAlignment = from.VerticalAlignment; }
            if (from._setElements.ContainsKey("HorizontalContentAlignment")) { this.HorizontalContentAlignment = from.HorizontalContentAlignment; }
            if (from._setElements.ContainsKey("VerticalContentAlignment")) { this.VerticalContentAlignment = from.VerticalContentAlignment; }
            if (from._setElements.ContainsKey("TextAlignment")) { this.TextAlignment = from.TextAlignment; }
        }
    }
}
