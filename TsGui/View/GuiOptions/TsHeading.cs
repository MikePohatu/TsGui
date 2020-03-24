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

// TsHeading.cs - Label with no control. Used to add headings and text only 
// amongst the other options. 

using System.Xml.Linq;

namespace TsGui.View.GuiOptions
{
    public class TsHeading : GuiOptionBase, IGuiOption
    {
        private string _controltext;
        
        //Properties

        //Custom stuff for control
        public override string CurrentValue { get { return this._controltext; } }
        public string ControlText
        {
            get { return this._controltext; }
            set { this._controltext = value; this.OnPropertyChanged(this, "ControlText"); }
        }
        public override TsVariable Variable { get { return null; } }

        //constructor
        public TsHeading(XElement InputXml, TsColumn Parent) : base(Parent)
        {
            this.ControlText = string.Empty;
            this.Control = new TsHeadingUI();
            this.Label = new TsLabelUI();
            this.UserControl.DataContext = this;
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.ControlText = XmlHandler.GetStringFromXElement(InputXml, "AltLabel", this.ControlText);
        }
    }
}
