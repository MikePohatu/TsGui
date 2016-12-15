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
using TsGui.Validation;

namespace TsGui.View.GuiOptions
{
    public class TsTextBlock: GuiOptionBase, IGuiOption
    {
        private string _controltext;
        private StringValidation _stringvalidation;

        //Custom stuff for control
        public override string CurrentValue { get { return this.ControlText; } }
        public string ControlText
        {
            get { return this._controltext; }
            set
            {
                this._controltext = value;
                this.OnPropertyChanged(this, "ControlText");
                this.NotifyUpdate();
            }
        }
        public TsVariable Variable { get { return null; } }

        //public TsColumn Parent { get; set; }

        public TsTextBlock (XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            
            this.UserControl.DataContext = this;
            this._stringvalidation = new StringValidation(MainController);
            this.SetDefaults();
            this.Control = new TsTextBlockUI();
            this.Label = new TsLabelUI();
            this.LoadXml(InputXml);
        }

        private void SetDefaults()
        {
            this._stringvalidation.MaxLength = 32760;
            this.LabelText = string.Empty;
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement x;

            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", this.LabelText);

            //load legacy options
            x = InputXml.Element("Disallowed");
            if (x != null) { this._stringvalidation.LoadLegacyXml(x); }
            this._stringvalidation.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", this._stringvalidation.MaxLength);


            x = InputXml.Element("Validation");
            if (x != null) { this._stringvalidation.LoadXml(x); }

            x = InputXml.Element("DisplayValue");
            if (x != null)
            {
                this.ControlText = this._controller.GetValueFromList(x);
                if (this.ControlText == null) { this.ControlText = string.Empty; }

                //if required, remove invalid characters and truncate
                //if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.ControlText = ResultValidator.RemoveInvalid(this.ControlText, this.DisallowedCharacters); }
                if (this._stringvalidation.MaxLength > 0) { this.ControlText = ResultValidator.Truncate(this.ControlText, this._stringvalidation.MaxLength); }
            }
        }
    }
}
