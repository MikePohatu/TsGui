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

// TsHeading.cs - Label with no control. Used to add headings and text only 
// amongst the other options. 

using System.Xml.Linq;
using System.Windows;
using TsGui.Queries;
using TsGui.Linking;

namespace TsGui.View.GuiOptions
{
    public class TsInfoBox : GuiOptionBase, IGuiOption, ILinkTarget
    {
        private string _controltext;
        private QueryList _displayvaluelist;
        //Properties

        //Custom stuff for control
        public override string CurrentValue { get { return this._controltext; } }
        public string ControlText
        {
            get { return this._controltext; }
            set {
                this._controltext = value;
                this.OnPropertyChanged(this, "ControlText");
                this.NotifyUpdate();
            }
        }
        public override TsVariable Variable { get { return new TsVariable(this.VariableName, this.ControlText); } }

        //constructor
        public TsInfoBox(XElement InputXml, TsColumn Parent, IDirector MainController) : base(Parent, MainController)
        {
            this.Control = new TsInfoBoxUI();
            this.Label = new TsLabelUI();
            this.UserControl.DataContext = this;
            this.SetDefaults();
            this._displayvaluelist = new QueryList(this, this._controller);
            this.LoadXml(InputXml);
            this.RefreshControlText();
        }

        public new void LoadXml(XElement InputXml)
        {            
            base.LoadXml(InputXml);

            XElement x;

            x = InputXml.Element("DisplayValue");
            if (x != null)
            { this._displayvaluelist.LoadXml(x); }
        }

        public void RefreshValue()
        { this.RefreshControlText(); }

        private void RefreshControlText()
        {
            this.ControlText = this._displayvaluelist.GetResultWrangler()?.GetString();
        }

        private void SetDefaults()
        {
            this.ControlText = string.Empty;
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
