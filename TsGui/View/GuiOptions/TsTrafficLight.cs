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

// TsComplianceCheck.cs - Displays an icon indicating whether a device is compliant with
// a specific condition

using System;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;
using TsGui.Validation;

namespace TsGui.View.GuiOptions
{
    public class TsTrafficLight: GuiOptionBase, IGuiOption, IValidationGuiOption
    {
        private string _currentvalue;
        private XElement _queryxml;
        private double _iconheight;
        private double _iconwidth;
        private SolidColorBrush _fillcolor;
        private TsTrafficLightUI _trafficlight;
        private ComplianceState _state;
        private ValidationToolTipHandler _validationtooltip;
        private ValidationHandler _validationhandler;
        private string _validationtext;

        //properties
        public double IconHeight
        {
            get { return this._iconheight; }
            set { this._iconheight = value; this.OnPropertyChanged(this, "IconHeight"); }
        }
        public double IconWidth
        {
            get { return this._iconwidth; }
            set { this._iconwidth = value; this.OnPropertyChanged(this, "IconWidth"); }
        }
        public SolidColorBrush FillColor
        {
            get { return this._fillcolor; }
            set { this._fillcolor = value; this.OnPropertyChanged(this, "FillColor"); }
        }
        public bool IsValid { get { return this.Validate(); } }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this._state.ToString()); }
            }
        }

        //constructor
        public TsTrafficLight(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this._controller = MainController;

            this._trafficlight = new TsTrafficLightUI();
            this.Control = this._trafficlight;
            this.Label = new TsLabelUI();

            this.FillColor = new SolidColorBrush(Colors.Blue);
            this._validationhandler = new ValidationHandler(this, MainController);
            this._validationtooltip = new ValidationToolTipHandler(this, this._controller);

            this.UserControl.DataContext = this;
            this.SetDefaults();
            this.LoadXml(InputXml);
            this.ProcessQuery();
            this.Validate();
        }

        //methods
        public bool Validate()
        {
            //if (this._controller.StartupFinished == false) { return true; }
            if (this.IsActive == false) { this._validationtooltip.Clear(); return true; }

            bool newvalid = this._validationhandler.IsValid(this._currentvalue);

            if (newvalid == false)
            {
                string validationmessage = this._validationhandler.ValidationMessage;
                string s = "\"" + this._currentvalue + "\" is invalid" + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + _validationhandler.FailedValidationMessage; }
                else { s = s + validationmessage; }

                this.ValidationText = s;
                this._validationtooltip.Show();
                this.SetState(ComplianceState.NotCompliant);
            }
            else
            {
                this._validationtooltip.Clear();
                this.SetState(ComplianceState.Compliant);
            }

            return newvalid;
        }

        public void ClearToolTips()
        { this._validationtooltip.Clear(); }

        public void OnValidationChange()
        { this.Validate(); }

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.LoadLegacyXml(InputXml);
            this._validationhandler.LoadLegacyXml(InputXml);
            this._validationhandler.AddValidations(InputXml.Elements("Validation"));

            //wrap the query in another to make it suitable for the controller. 
            this._queryxml = new XElement("GetValue",InputXml.Element("Query"));
        } 

        private void ProcessQuery()
        {
            if (this._queryxml != null) { this._currentvalue = this._controller.GetValueFromList(this._queryxml); }
        }

        private void SetDefaults()
        {
            this.SetState(ComplianceState.Compliant);
            this.ControlFormatting.Padding = new Thickness(0, 0, 0, 0);
            this.ControlFormatting.Margin = new Thickness(2, 1, 2, 1);
            this.ControlFormatting.VerticalAlignment = VerticalAlignment.Center;
            this.IconHeight = 15;
            this.IconWidth = 15;
        }

        private void SetState(ComplianceState State)
        {
            this._state = State;
            switch (State)
            {
                case ComplianceState.Compliant:
                    this.FillColor.Color = Colors.Green;
                    break;
                case ComplianceState.NotCompliant:
                    this.FillColor.Color = Colors.Red;
                    break;
                case ComplianceState.Warning:
                    this.FillColor.Color = Colors.Orange;
                    break;
                default:
                    throw new ArgumentException("State is not valid");
            }
        }

        private void LoadLegacyXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("HAlign");
            if (x != null)
            {
                string s = x.Value.ToUpper();
                switch (s)
                {
                    case "RIGHT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "LEFT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "CENTER":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "STRETCH":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
;