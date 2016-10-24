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

// TsFreeText.cs - TextBox control for entering text. Can be configured to 
// check for the validity of the entered text

using TsGui.Validation;

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace TsGui.View.GuiOptions
{
    public class TsFreeText: GuiOptionBase, IGuiOption_2, IEditableGuiOption
    {
        private ToolTip _validationtooltip;
        private TsFreeTextUI _ui;
        private string _controltext;
        private StringValidation _stringvalidation;
        //private bool _isvalid;
        private Color _bordercolor;
        private Color _hoveroverbordercolor;

        //Properties
        #region
        //standard stuff
        public UserControl Control { get { return this._ui; } }

        //Custom stuff for control
        public string ControlText
        {
            get { return this._controltext; }
            set
            {
                this._controltext = value;
                this.OnPropertyChanged(this, "ControlText");
                this.Validate();
            }
        }
        public bool IsValid { get { return this.Validate(); } }
        public int MaxLength
        {
            get { return this._stringvalidation.MaxLength; }
            set { this._stringvalidation.MaxLength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.ControlText); }
            }
        }
        #endregion

        //Constructor
        public TsFreeText(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this.Init(MainController);
            this.LoadXml(InputXml);
        }

        protected TsFreeText(TsColumn Parent, MainController MainController): base(Parent, MainController)
        {
            this.Init(MainController);
        }

        private void Init(MainController MainController)
        {
            this._controller = MainController;
            this._stringvalidation = new StringValidation();
            this._ui = new TsFreeTextUI();
            this._ui.DataContext = this;
            this._ui.Control.LostFocus += this.onLoseFocus;
            this._stringvalidation.MaxLength = 32760;
            this._stringvalidation.MinLength = 0;
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement x;

            //load legacy options
            x = InputXml.Element("Disallowed");
            if (x != null) { this._stringvalidation.LoadLegacyXml(x); }
            this._stringvalidation.MinLength = XmlHandler.GetIntFromXAttribute(InputXml, "MinLength", this._stringvalidation.MinLength);
            this._stringvalidation.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", this._stringvalidation.MaxLength);

            
            
            //record the colors
            this._bordercolor = this.ControlFormatting.BorderColorBrush.Color;
            this._hoveroverbordercolor = this.ControlFormatting.HoverOverColorBrush.Color;

            //this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", this.LabelText);

            x = InputXml.Element("Validation");
            if (x != null) { this._stringvalidation.LoadXml(x); }

            x = InputXml.Element("DefaultValue");
            if (x != null)
            {
                XAttribute xusecurrent = x.Attribute("UseCurrent");
                if (xusecurrent != null)
                {
                    //default behaviour is to check if the ts variable is already set. If it is, set that as the default i.e. add a query for 
                    //an environment variable to the start of the query list. 
                    if (!string.Equals(xusecurrent.Value, "false", StringComparison.OrdinalIgnoreCase))
                    {
                        XElement xcurrentquery = new XElement("Query", new XElement("Variable", this.VariableName));
                        xcurrentquery.Add(new XAttribute("Type", "EnvironmentVariable"));
                        x.AddFirst(xcurrentquery);
                    }
                }

                this.ControlText = this._controller.GetValueFromList(x);
                if (this.ControlText == null) { this.ControlText = string.Empty; }

                //if required, remove invalid characters and truncate
                //if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.ControlText = ResultValidator.RemoveInvalid(this.ControlText, this.DisallowedCharacters); }
                if (this.MaxLength > 0) { this.ControlText = ResultValidator.Truncate(this.ControlText, this.MaxLength); }
            }
        }

        //Handle UI events
        #region

        public void onLoseFocus(object sender, RoutedEventArgs e)
        { this.Validate(); }
        #endregion


        private bool Validate()
        {
            bool valid = this._stringvalidation.IsValid(this.ControlText);
            string s = this._stringvalidation.ValidationMessage;
            
            if (valid == false)
            {
                if (string.IsNullOrEmpty(s)) { s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine + Environment.NewLine + _stringvalidation.FailedValidationMessage; }
                //{ s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine; }
                this.ShowInvalidToolTip(s);               
            }
            else { this.ClearToolTips(); }

            return valid;
        }


        public void ClearToolTips()
        {
            TsWindowAlerts.HideToolTip(this._validationtooltip);
            this.ControlFormatting.BorderColorBrush.Color = this._bordercolor;
            this.ControlFormatting.HoverOverColorBrush.Color = this._hoveroverbordercolor;
        }


        public void ShowInvalidToolTip(string Message)
        {
            this._validationtooltip = TsWindowAlerts.ShowUnboundToolTip(this._validationtooltip, this._ui.Control, Message);
            this._validationtooltip.Placement = PlacementMode.Right;

            //update the colors to red. 
            this.ControlFormatting.BorderColorBrush.Color = Colors.Red;
            this.ControlFormatting.HoverOverColorBrush.Color = Colors.Red;
        }
    }
}
