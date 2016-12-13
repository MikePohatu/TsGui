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
        private Window _parentwindow;

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
        public WindowStartupLocation StartupLocation { get; set; }

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

        public WindowLocation(XElement InputXml, Window ParentWindow)
        {
            this._parentwindow = ParentWindow;
            this.SetDefaults();
            this.LoadXml(InputXml);
        }

        public WindowLocation(Window ParentWindow)
        {
            this._parentwindow = ParentWindow;
            this.SetDefaults();
        }

        public void LoadXml(XElement InputXml)
        {
            this.StartupLocation = XmlHandler.GetWindowStartupLocationFromXElement(InputXml, "StartupLocation", this.StartupLocation);
            this.Left = XmlHandler.GetDoubleFromXElement(InputXml, "Left", this.Left);
            this.Top = XmlHandler.GetDoubleFromXElement(InputXml, "Top", this.Top);

            if (this.StartupLocation == WindowStartupLocation.CenterScreen) { this.SetCenter(); }
        }

        private void SetDefaults()
        {
            this.StartupLocation = WindowStartupLocation.CenterScreen;
            this.SetCenter();
            //this.Left = 40;
            //this.Top = 40;
        }

        private void SetCenter()
        {
            double screenwidth = SystemParameters.WorkArea.Width;
            double screenheight = SystemParameters.WorkArea.Height;
            this.Left = (screenwidth / 2) - (this._parentwindow.Width / 2);
            this.Top = (screenheight / 2) - (this._parentwindow.Height / 2);
        }
    }
}
