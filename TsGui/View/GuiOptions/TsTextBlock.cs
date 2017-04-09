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
using TsGui.Queries;
using TsGui.Linking;

namespace TsGui.View.GuiOptions
{
    public class TsTextBlock: GuiOptionBase, IGuiOption, ILinkTarget
    {
        private string _controltext;
        private StringValidation _stringvalidation;
        private QueryList _displayvaluelist;

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
        public override TsVariable Variable { get { return null; } }

        public TsTextBlock (XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this._controller = MainController;
            this._displayvaluelist = new QueryList(this,this._controller);
            this.UserControl.DataContext = this;
            this._stringvalidation = new StringValidation(MainController);
            this.SetDefaults();
            this.Control = new TsTextBlockUI();
            this.Label = new TsLabelUI();
            this.LoadXml(InputXml);
            this.RefreshValue();
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
                this._displayvaluelist.LoadXml(x);
            }
        }

        public void RefreshValue()
        {
            this.ControlText = this._displayvaluelist.GetResultWrangler()?.GetString();
            if (this.ControlText == null) { this.ControlText = string.Empty; }
            else
            {
                //if required, remove invalid characters and truncate
                //if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.ControlText = ResultValidator.RemoveInvalid(this.ControlText, this.DisallowedCharacters); }
                if (this._stringvalidation.MaxLength > 0) { this.ControlText = ResultValidator.Truncate(this.ControlText, this._stringvalidation.MaxLength); }
            }
        }
    }
}
