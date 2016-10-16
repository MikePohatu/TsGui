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

// Formatting.cs - view model for the layout of GuiOptions. Controls the layout and 
// formatting options for the associated controls

using System.Windows;
using System.Xml.Linq;
using System.ComponentModel;

namespace TsGui.View.GuiOptions
{
    public class Formatting: INotifyPropertyChanged
    {
        //Fields
        #region
        private IGroupable _parent;
        private int _height;
        private int _width;

        private Thickness _margin;
        private Thickness _padding;
        private VerticalAlignment _verticalalign;
        private HorizontalAlignment _horizontalalign;

        private bool _isenabled = true;
        private bool _ishidden = false;
        private Visibility _visibility = Visibility.Visible;
        #endregion

        //constructors
        public Formatting(IGroupable Parent)
        {
            //register for property changes in the parent. 
            this._parent = Parent;
            this._parent.PropertyChanged += this.OnParentPropertyChanged;
        }

        //Properties
        #region 
        public bool IsEnabled
        {
            get { return this._isenabled; }
            set { this._isenabled = value; this.OnPropertyChanged(this, "IsEnabled"); }
        }

        public bool IsHidden
        {
            get { return this._ishidden; }
            set { this.HideUnhide(value); this.OnPropertyChanged(this, "IsHidden"); }
        }

        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false)) { return true; }
                else { return false; }
            }
        }
        public Visibility Visibility
        {
            get { return this._visibility; }
            set { this._visibility = value; this.OnPropertyChanged(this, "Visibility"); }
        }
        public int Height
        {
            get { return this._height; }
            set { this._height = value; this.OnPropertyChanged(this, "Height"); }
        }

        public int Width
        {
            get { return this._width; }
            set { this._width = value; this.OnPropertyChanged(this, "Width"); }
        }

        public Thickness Margin
        {
            get { return this._margin; }
            set { this._margin = value; this.OnPropertyChanged(this, "Margin"); }
        }

        public Thickness Padding
        {
            get { return this._padding; }
            set { this._padding = value; this.OnPropertyChanged(this, "Padding"); }
        }

        public VerticalAlignment VerticalAlign
        {
            get { return this._verticalalign; }
            set { this._verticalalign = value; this.OnPropertyChanged(this, "VerticalAlign"); }
        }

        public HorizontalAlignment HorizontalAlign
        {
            get { return this._horizontalalign; }
            set { this._horizontalalign = value; this.OnPropertyChanged(this, "HorizontalAlign"); }
        }
        #endregion


        //Event handling
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        public void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
        #endregion

        public void LoadXml(XElement InputXml)
        {
            //Load the XML
            #region
            this.Height = XmlHandler.GetIntFromXElement(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetIntFromXElement(InputXml, "Width", this.Width);
            this.Padding = XmlHandler.GetThicknessFromXElement(InputXml, "Padding", 0);
            this.Margin = XmlHandler.GetThicknessFromXElement(InputXml, "Margin", 0);

            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);
            #endregion
        }

        private void HideUnhide(bool Hidden)
        {
            this._ishidden = Hidden;
            if (Hidden == true)
            { this.Visibility = Visibility.Collapsed; }
            else
            { this.Visibility = Visibility.Visible; }
        }

        //handle changes to properties from the parent object. required to handle
        //group change events
        public void OnParentPropertyChanged(object o, PropertyChangedEventArgs e)
        {
            //PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
            IGroupable option = (IGroupable)o;
            switch (e.PropertyName)
            {
                case "IsEnabled":
                    this.IsEnabled = option.IsEnabled;
                    break;
                case "IsHidden":
                    this.IsHidden = option.IsHidden;
                    break;
                default:
                    break;
            }
        }
    }
}
