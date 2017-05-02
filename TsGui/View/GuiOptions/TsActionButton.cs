//    Copyright (C) 2017 Mike Pohatu

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

using System.Xml.Linq;
using System.Windows;
using TsGui.View.Layout;
using TsGui.Actions;
using TsGui.Diagnostics.Logging;

namespace TsGui.View.GuiOptions
{
    public class TsActionButton : GuiOptionBase, IGuiOption
    {
        private IRootLayoutElement _rootelement;
        private string _buttontext;
        private TsButtonUI _ui;
        private IAction _action;

        public override string CurrentValue { get { return null; } }
        public override TsVariable Variable { get { return null; } }
        public string ButtonText
        {
            get { return this._buttontext; }
            set
            {
                this._buttontext = value;
                this.OnPropertyChanged(this, "ButtonText");
            }
        }

        //Constructor
        public TsActionButton(XElement InputXml, TsColumn Parent, IDirector MainController) : base(Parent,MainController)
        {
            this._rootelement = this.GetRootElement();

            this.UserControl.DataContext = this;
            this._ui = new TsButtonUI();
            this.Control = this._ui;
            this._ui.RetryButton.Click += this.OnButtonClick;

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
            if (x != null) { this._action = ActionFactory.CreateAction(x, this._director); }
        }

        public void OnButtonClick(object o, RoutedEventArgs e)
        {
            LoggerFacade.Info("Action button clicked");
            this._action.RunAction();
        }

        private void SetDefaults()
        {
            this.ButtonText = "Apply";
            this.ControlFormatting.Height = 25;
            this.ControlFormatting.Width = 60;
            this.ControlFormatting.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
