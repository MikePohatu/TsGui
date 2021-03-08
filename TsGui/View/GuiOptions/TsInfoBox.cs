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

namespace TsGui.View.GuiOptions
{
    public class TsInfoBox : GuiOptionBase, IGuiOption, ILinkTarget
    {
        private string _controltext;
        
        //Properties
        public override string CurrentValue { get { return this._controltext; } }
        public string ControlText
        {
            get { return this._controltext; }
            set {
                this._controltext = value;
                this.OnPropertyChanged(this, "ControlText");
                this.NotifyViewUpdate();
            }
        }
        public override TsVariable Variable
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.VariableName) == false)
                { return new TsVariable(this.VariableName, this.ControlText); }
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
            this.SetDefaults();
            this._querylist = new QueryPriorityList(this);
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {            
            base.LoadXml(InputXml);

            XElement x;

            x = InputXml.Element("DisplayValue");
            if (x != null)
            { this.LoadSetValueXml(x,false); }
        }

        public override void UpdateValue(Message message)
        {
            this.ControlText = this._querylist.GetResultWrangler(message)?.GetString();
            Director.Instance.LinkingHub.SendUpdateMessage(this, message);
        }

        public void OnSourceValueUpdated(Message message)
        { this.UpdateValue(message); }

        private void SetDefaults()
        {
            this.ControlText = string.Empty;
            //this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
