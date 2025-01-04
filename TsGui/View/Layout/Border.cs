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
using Core;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace TsGui.View.Layout
{
    public class Border : ViewModelBase
    {
        private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get { return this._brush; }
            set { this._brush = value; this.OnPropertyChanged(this, "Brush"); }
        }

        private Thickness _thickness = new Thickness(0);
        public Thickness Thickness
        {
            get { return this._thickness; }
            set
            {
                this._thickness = value;
                this.OnPropertyChanged(this, "Thickness");
            }
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml != null)
            {
                this.Brush = XmlHandler.GetSolidColorBrushFromXml(InputXml, "Color", this._brush);
                this.Thickness = XmlHandler.GetThicknessFromXml(InputXml, "Thickness", this._thickness);
            }
        }
    }
}
