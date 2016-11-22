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

// Positioning.cs - view model for window positioning

using System.ComponentModel;
using System.Windows;
using System.Xml.Linq;

namespace TsGui.View.Layout
{
    public class WindowLocation: INotifyPropertyChanged
    {
        private double _left;
        private double _top;
        private WindowStartupLocation _startuplocation;

        public double Left
        {
            get { return this._left; }
            set { this._left = value; this.OnPropertyChanged(this, "Left"); }
        }
        public double Top
        {
            get { return this._top; }
            set { this._top = value; this.OnPropertyChanged(this, "Top"); }
        }
        public WindowStartupLocation StartupLocation
        {
            get { return this._startuplocation; }
            set { this._startuplocation = value; this.OnPropertyChanged(this, "StartupLocation"); }
        }

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

        public WindowLocation(XElement InputXml)
        {
            this.SetDefaults();
            this.LoadXml(InputXml);
        }

        public WindowLocation()
        {
            this.SetDefaults();
        }

        public void LoadXml(XElement InputXml)
        {
            this.StartupLocation = XmlHandler.GetWindowStartupLocationFromXElement(InputXml, "StartupLocation", this.StartupLocation);
            this.Left = XmlHandler.GetDoubleFromXElement(InputXml, "Left", this.Left);
            this.Top = XmlHandler.GetDoubleFromXElement(InputXml, "Top", this.Top);
        }

        private void SetDefaults()
        {
            this.StartupLocation = WindowStartupLocation.CenterScreen;
            this.Left = 0;
            this.Top = 0;
        }
    }
}
