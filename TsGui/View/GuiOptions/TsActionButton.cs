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
using System.Xml.Linq;
using System.Windows;
using TsGui.View.Layout;
using TsGui.Actions;
using Core.Logging;
using TsGui.Validation;
using MessageCrap;

namespace TsGui.View.GuiOptions
{
    public class TsActionButton : GuiOptionBase, IGuiOption
    {
        private IComplianceRoot _rootelement;
        private string _buttontext;
        private TsButtonUI _ui;
        private IAction _action;
        private bool _isdefault;

        public override string CurrentValue { get { return null; } }
        public override Variable Variable { get { return null; } }
        public string ButtonText
        {
            get { return this._buttontext; }
            set
            {
                this._buttontext = value;
                this.OnPropertyChanged(this, "ButtonText");
            }
        }
        public bool IsDefault
        {
            get { return this._isdefault; }
            set
            {
                this._isdefault = value;
                this.OnPropertyChanged(this, "IsDefault");
            }
        }

        //Constructor
        public TsActionButton(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            this._rootelement = this.GetComplianceRootElement();

            this.UserControl.DataContext = this;
            this._ui = new TsButtonUI();
            this.Control = this._ui;
            this._ui.button.Click += this.OnButtonClick;

            this.Label = new TsLabelUI();
            this.SetDefaults();
            this.LoadXml(InputXml);
        }


        //Methods
        public new void LoadXml(XElement inputxml)
        {
            //load the xml for the base class stuff
            base.LoadXml(inputxml);
            this.ButtonText = XmlHandler.GetStringFromXElement(inputxml, "ButtonText", this.ButtonText);

            XElement x;
            x = inputxml.Element("Action");
            if (x != null) { this._action = ActionFactory.CreateAction(x); }

            this.IsDefault = XmlHandler.GetBoolFromXAttribute(inputxml, "IsDefault", this.IsDefault);
        }

        public void OnButtonClick(object o, RoutedEventArgs e)
        {
            Log.Info("Action button clicked");
            this._action?.RunAction();
        }

        private void SetDefaults()
        {
            ControlDefaults.SetButtonDefaults(this.ControlFormatting);
            this._isdefault = false;
            this.ButtonText = "Apply";
        }

        public override void UpdateValue(Message message) { }
    }
}
