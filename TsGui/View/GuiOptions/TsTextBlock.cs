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

// TsTextBlock.cs - multi-line textblock for user input


using System.ComponentModel;
using System.Xml.Linq;
using System.Windows.Controls;

namespace TsGui.View.GuiOptions
{
    public class TsTextBlock: GuiOptionBase
    {
        private MainController _controller;
        private TsTextBlockUI _ui;
        private string _value;
        private string _labeltext;
        private string _helptext;

        //standard stuff
        public Grid Grid { get { return this._ui.MainGrid; } }

        //Custom stuff for control
        public string Value
        {
            get { return this._value; }
            set { this._value = value; this.OnPropertyChanged(this, "Value"); }
        }
        public int MaxLength { get; set; }
        public string DisallowedCharacters { get; set; }

        //public TsColumn Parent { get; set; }

        public TsTextBlock (XElement InputXml, MainController MainController): base ()
        {
            this._controller = MainController;
            this._ui = new TsTextBlockUI();
            this._ui.DataContext = this;
            this.LoadXml(InputXml);
        }


        public void LoadXml(XElement InputXml)
        {
            base.LoadBaseXml(InputXml);
            XElement x;

            this.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", 32760);

            x = InputXml.Element("Disallowed");
            if (x != null)
            {
                this.DisallowedCharacters = XmlHandler.GetStringFromXElement(x, "Characters", null);
            }

            x = InputXml.Element("DisplayValue");
            if (x != null)
            {
                this.Value = this._controller.GetValueFromList(x);
                if (this.Value == null) { this.Value = string.Empty; }

                //if required, remove invalid characters and truncate
                if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.Value = ResultValidator.RemoveInvalid(this.Value, this.DisallowedCharacters); }
                if (this.MaxLength > 0) { this.Value = ResultValidator.Truncate(this.Value, this.MaxLength); }
            }
        }
    }
}
