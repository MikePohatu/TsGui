﻿//    Copyright (C) 2016 Mike Pohatu

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

// TsHeading.cs - Label with no control. Used to add headings and text only 
// amongst the other options. 

using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui.View.GuiOptions
{
    public class TsInfoBox : GuiOptionBase, IGuiOption_2
    {
        private string _controltext;

        //Properties

        //Custom stuff for control
        public string ControlText
        {
            get { return this._controltext; }
            set { this._controltext = value; this.OnPropertyChanged(this, "ControlText"); }
        }
        public TsVariable Variable { get { return null; } }

        //constructor
        public TsInfoBox(XElement InputXml, TsColumn Parent, MainController MainController) : base(Parent, MainController)
        {
            this.ControlText = string.Empty;
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.UserControl.DataContext = this;
            this.Control = new TsInfoBoxUI();
            this.Label = new TsLabelUI();
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            //load legacy
            XElement x;
            x = InputXml.Element("Bold");
            if (x != null) { this.LabelFormatting.FontWeight = "Bold"; }

            base.LoadXml(InputXml);

            x = InputXml.Element("DisplayValue");
            if (x != null)
            {
                this.ControlText = this._controller.GetValueFromList(x);
                if (this.ControlText == null) { this.ControlText = string.Empty; }

                ////if required, remove invalid characters and truncate
                //if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.Value = ResultValidator.RemoveInvalid(this.Value, this.DisallowedCharacters); }
                //if (this._maxlength > 0) { this.Value = ResultValidator.Truncate(this.Value, this._maxlength); }
            }
        }
    }
}