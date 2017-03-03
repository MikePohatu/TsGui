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

// TsComplianceRefreshButton.cs - button for retrying compliance rules on a page

using System.Xml.Linq;
using System.Windows;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions
{
    public class TsComplianceRefreshButton : GuiOptionBase, IGuiOption
    {
        private IRootLayoutElement _rootelement;
        private string _buttontext;
        private TsComplianceRefreshButtonUI _ui;

        public override string CurrentValue { get { return null; } }
        public TsVariable Variable { get { return null; } }
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
        public TsComplianceRefreshButton(XElement InputXml, TsColumn Parent, MainController MainController) : base(Parent,MainController)
        {
            this._rootelement = this.GetRootElement();

            this.UserControl.DataContext = this;           
            this._ui = new TsComplianceRefreshButtonUI();
            this.Control = this._ui;
            this._ui.RetryButton.Click += this.OnButtonClick;

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
            this.ButtonText = "Refresh";
            this.ControlFormatting.Height = 25;
            this.ControlFormatting.Width = 60;
            this.ControlFormatting.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
