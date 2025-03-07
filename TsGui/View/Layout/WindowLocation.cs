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

// WindowLocation.cs - view model for window positioning

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
        public WindowStartupLocation StartupLocation
        {
            get { return this._startuplocation; }
            set { this._startuplocation = value; }
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

        public WindowLocation(XElement InputXml, Window ParentWindow)
        {
            this.Init(ParentWindow);
            this.LoadXml(InputXml);
        }

        public WindowLocation(Window ParentWindow)
        {
            this.Init(ParentWindow);
        }

        public void LoadXml(XElement InputXml)
        {
            this.StartupLocation = XmlHandler.GetWindowStartupLocationFromXml(InputXml, "StartupLocation", this.StartupLocation);
            this.Left = XmlHandler.GetDoubleFromXml(InputXml, "Left", this.Left);
            this.Top = XmlHandler.GetDoubleFromXml(InputXml, "Top", this.Top);
        }

        private void Init(Window ParentWindow)
        {
            this._parentwindow = ParentWindow;
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this.StartupLocation = WindowStartupLocation.CenterScreen;
            if (Arguments.Instance.TestingLeft) 
            { 
                this.StartupLocation = WindowStartupLocation.Manual;
                this.Left = 300;
                this.Top = 40;
            }
            else if (Arguments.Instance.TestingRight)
            {
                this.StartupLocation = WindowStartupLocation.Manual;
                this.Left = 900;
                this.Top = 40;
            }
        }
    }
}
