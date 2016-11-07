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

// GuiOptionBase.cs - base parts for all GuiOptions

using System.Xml.Linq;
using TsGui.View.Layout;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace TsGui.View.GuiOptions
{
    public abstract class GuiOptionBase : BaseLayoutElement
    {
        private string _labeltext = string.Empty;
        private string _helptext = null;
        private string _inactivevalue = "TSGUI_INACTIVE";
        private GuiOptionBaseUI _ui;
        private bool _invertlayout = false;

        //standard stuff
        public TsColumn Parent { get; set; }
        public UserControl Control { get; set; }
        public UserControl Label { get; set; }
        public GuiOptionBaseUI UserControl
        {
            get { return this._ui; }
            set { this._ui = value; }
        }
        public string InactiveValue
        {
            get { return this._inactivevalue; }
            set { this._inactivevalue = value; }
        }
        public string VariableName { get; set; }
        public string LabelText
        {
            get { return this._labeltext; }
            set { this._labeltext = value; this.OnPropertyChanged(this, "LabelText"); }
        }
        public string HelpText
        {
            get { return this._helptext; }
            set { this._helptext = value; this.OnPropertyChanged(this, "HelpText"); }
        }
        
        
        public GuiOptionBase(TsColumn Parent, MainController MainController):base(Parent,MainController)
        {
            this.Parent = Parent;
            this._controller = MainController;
            this.UserControl = new GuiOptionBaseUI();
        }

        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            //load legacy
            XElement x;
            x = InputXml.Element("Bold");
            if (x != null) { this.LabelFormatting.FontWeight = "Bold"; }

            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", this.VariableName);
            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", this.LabelText);
            this.HelpText = XmlHandler.GetStringFromXElement(InputXml, "HelpText", this.HelpText);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.Parent.ShowGridLines);
            this.InactiveValue = XmlHandler.GetStringFromXElement(InputXml, "InactiveValue", this.InactiveValue);
            this.SetLayoutRightLeft();
        }

        private void SetDefaults()
        {
            this.LabelFormatting.Padding = new Thickness(3, 0, 0, 0);
        }

        private void SetLayoutRightLeft()
        {
            if (this.InvertColumns == false)
            {
                this.UserControl.RightPresenter.Content = this.Control;
                this.UserControl.LeftPresenter.Content = this.Label;
            }
            else
            {
                this.UserControl.RightPresenter.Content = this.Label;
                this.UserControl.LeftPresenter.Content = this.Control;
            }
        }
    }
}
