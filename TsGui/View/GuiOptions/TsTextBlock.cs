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


using System.Xml.Linq;
using System.Windows.Controls;
using TsGui.Validation;

namespace TsGui.View.GuiOptions
{
    public class TsTextBlock: GuiOptionBase, IGuiOption_2
    {
        private TsTextBlockUI _ui;
        private string _controltext;

        //standard stuff
        public UserControl Control { get { return this._ui; } }

        //Custom stuff for control
        public string ControlText
        {
            get { return this._controltext; }
            set { this._controltext = value; this.OnPropertyChanged(this, "ControlText"); }
        }
        public int MaxLength { get; set; }
        public string DisallowedCharacters { get; set; }
        public TsVariable Variable { get { return null; } }

        //public TsColumn Parent { get; set; }

        public TsTextBlock (XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this._controller = MainController;
            this._ui = new TsTextBlockUI();
            this._ui.DataContext = this;
            this.LoadXml(InputXml);
        }


        public void LoadXml(XElement InputXml)
        {
            this.LoadBaseXml(InputXml);
            XElement x;

            this.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", 32760);
            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", string.Empty);

            x = InputXml.Element("Disallowed");
            if (x != null) { this.DisallowedCharacters = XmlHandler.GetStringFromXElement(x, "Characters", null); }

            x = InputXml.Element("DisplayValue");
            if (x != null)
            {
                this.ControlText = this._controller.GetValueFromList(x);
                if (this.ControlText == null) { this.ControlText = string.Empty; }

                //if required, remove invalid characters and truncate
                if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.ControlText = ResultValidator.RemoveInvalid(this.ControlText, this.DisallowedCharacters); }
                if (this.MaxLength > 0) { this.ControlText = ResultValidator.Truncate(this.ControlText, this.MaxLength); }
            }
        }
    }
}
