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
using System.Windows;
using TsGui.Queries;
using TsGui.Linking;
using TsGui.View.Layout;
using MessageCrap;
using System.Threading.Tasks;
using TsGui.Validation;

namespace TsGui.View.GuiOptions
{
    public class TsInfoBox : GuiOptionBase, IGuiOption, ILinkTarget
    {
        private string _controltext = string.Empty;
        private int _maxlength = 32760;
        private string _disallowedCharacters = string.Empty;

        //Properties
        public override string CurrentValue { get { return this._controltext; } }
        public string ControlText
        {
            get { return this._controltext; }
            set {
                this.SetValue(value, null);
            }
        }
        public override Variable Variable
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.VariableName) == false)
                { return new Variable(this.VariableName, this.ControlText, this.Path); }
                else
                { return null; }
            }
        }

        //constructor
        public TsInfoBox(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            this.Control = new TsInfoBoxUI();
            this.Label = new TsLabelUI();
            this.UserControl.DataContext = this;
            this._querylist = new QueryPriorityList(this);
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {            
            base.LoadXml(InputXml);

            XElement x;

            //load legacy options
            this._disallowedCharacters = XmlHandler.GetStringFromXml(InputXml, "Disallowed", this._disallowedCharacters);
            this._maxlength = XmlHandler.GetIntFromXml(InputXml, "MaxLength", this._maxlength);

            x = InputXml.Element("DisplayValue");
            if (x != null)
            { this.LoadSetValueXml(x,false); }
        }

        public override async Task UpdateValueAsync(Message message)
        {
            string val = (await this._querylist.GetResultWrangler(message))?.GetString();
            this.SetValue(val, message);
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        { await this.UpdateValueAsync(message); }

        private void SetValue(string value, Message message)
        {
            this._controltext = value;
            //if required, remove invalid characters and truncate
            if (!string.IsNullOrEmpty(this._disallowedCharacters)) { this._controltext = ResultValidator.RemoveInvalid(this._controltext, this._disallowedCharacters); }

            if (this._maxlength > 0) { this._controltext = ResultValidator.Truncate(this._controltext, this._maxlength); }

            this.OnPropertyChanged(this, "ControlText");
            this.NotifyViewUpdate();
            LinkingHub.Instance.SendUpdateMessage(this, message);
        }
    }
}
