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

// TsComplianceRefreshButton.cs - button for retrying compliance rules on a page

using System.Xml.Linq;
using System.Windows;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions
{
    public class TsComplianceRefreshButton : GuiOptionBase, IGuiOption
    {
        private IComplianceRoot _rootelement;
        private string _buttontext;
        private TsButtonUI _ui;

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

        private bool _isdefault = false;
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
        public TsComplianceRefreshButton(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
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
        public new void LoadXml(XElement InputXml)
        {
            //load the xml for the base class stuff
            base.LoadXml(InputXml);
            this.ButtonText = XmlHandler.GetStringFromXElement(InputXml, "ButtonText", this.ButtonText);
        }

        public void OnButtonClick(object o, RoutedEventArgs e)
        {
            this._rootelement.RaiseComplianceRetryEvent();
        }

        private void SetDefaults()
        {
            ControlDefaults.SetButtonDefaults(this.ControlFormatting);
            this.ButtonText = "Refresh";
        }
    }
}
