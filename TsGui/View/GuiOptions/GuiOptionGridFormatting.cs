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

// GuiOptionGridFormatting.cs - view model for the layout of GuiOption grids. 
// Adds right and left cell width

using System.Xml.Linq;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions
{
    public class GuiOptionGridFormatting: Formatting
    {
        private double _rightcellwidth;
        private double _leftcellwidth;

        public double RightCellWidth
        {
            get { return this._rightcellwidth; }
            set { this._rightcellwidth = value; this.OnPropertyChanged(this, "RightCellWidth"); }
        }
        public double LeftCellWidth
        {
            get { return this._leftcellwidth; }
            set { this._leftcellwidth = value; this.OnPropertyChanged(this, "LeftCellWidth"); }
        }

        public GuiOptionGridFormatting():base()
        {
            this.SetDefaults();
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            //Load the XML
            #region
            this.RightCellWidth = XmlHandler.GetDoubleFromXElement(InputXml, "RightCellWidth", this.RightCellWidth);
            this.LeftCellWidth = XmlHandler.GetDoubleFromXElement(InputXml, "LeftCellWidth", this.LeftCellWidth);
            #endregion
        }

        private void SetDefaults()
        {
            this.RightCellWidth = double.NaN;
            this.LeftCellWidth = double.NaN;
        }
    }
}
