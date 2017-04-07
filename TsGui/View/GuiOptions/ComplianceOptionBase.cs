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

// ComplianceOptionBase.cs - base class for compliance guioptions

using System;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;
using TsGui.Validation;
using TsGui.View.Layout;
using TsGui.Queries;

namespace TsGui.View.GuiOptions
{
    public abstract class ComplianceOptionBase : GuiOptionBase, IGuiOption, IValidationGuiOption
    {
        protected string _value;
        protected double _iconheight;
        protected double _iconwidth;
        protected SolidColorBrush _fillcolor;
        protected int _state;
        protected ValidationToolTipHandler _validationtooltiphandler;
        protected ComplianceHandler _compliancehandler;
        protected string _validationtext;
        protected IRootLayoutElement _rootelement;
        protected bool _showvalueinpopup;
        protected string _okHelpText;
        protected QueryList _getvaluelist;

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
        public override string CurrentValue { get { return ComplianceStateValues.ToString(this._state); } }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }
        public override TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.CurrentValue); }
            }
        }

        //constructor
        public ComplianceOptionBase(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this.Label = new TsLabelUI();
            this._rootelement = this.GetRootElement();
            this._rootelement.ComplianceRetry += this.OnComplianceRetry;

            this.FillColor = new SolidColorBrush(Colors.Blue);
            this._compliancehandler = new ComplianceHandler(this, MainController);
            this._validationtooltiphandler = new ValidationToolTipHandler(this, this._controller);

            this.UserControl.DataContext = this;
            this.SetDefaults();
            this._getvaluelist = new QueryList(MainController);       
        }

        //methods
        public bool Validate()
        {
            bool returnval = false;
            this.UpdateState();

            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }          

            if (this._state == ComplianceStateValues.Invalid)
            {
                string validationmessage = this._compliancehandler.ValidationMessage;
                string s = string.Empty;

                
                if (string.IsNullOrEmpty(validationmessage)) { s = s + _compliancehandler.FailedValidationMessage; }
                else { s = validationmessage; }

                if ((this._showvalueinpopup == true) || string.IsNullOrEmpty(s)) { s = "\"" + this._value + "\" is invalid" + Environment.NewLine + s; }

                this.ValidationText = s;
                this._validationtooltiphandler.ShowError();
                returnval = false;
            }
            else
            {
                this.ClearToolTips();
                returnval = true;
            }
            
            return returnval;
        }

        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

        public void OnValidationChange()
        { this.Validate(); }

        public void OnComplianceRetry(IRootLayoutElement o, EventArgs e)
        {
            this._getvaluelist.ProcessAllQueries();
            this.ProcessQuery();
            this.Validate();
        }

        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            XElement x;

            this.LoadLegacyXml(InputXml);
            this._okHelpText = this.HelpText;
            this._compliancehandler.AddCompliances(InputXml.Elements("Compliance"));
            this._showvalueinpopup = XmlHandler.GetBoolFromXElement(InputXml, "PopupShowValue", this._showvalueinpopup);

            //wrap the query in another to make it suitable for the controller. 
            x = InputXml.Element("GetValue");
            if (x != null) { this._getvaluelist.LoadXml(x); }  
        } 

        protected void ProcessQuery()
        {
            this._value = this._getvaluelist.GetResultWrangler()?.GetString();
            this.UpdateState();
        }

        private void RefreshValue()
        { }

        private void UpdateState()
        {
            if (this.IsActive == true) { this._state = this._compliancehandler.EvaluateComplianceState(this._value); }
            else { this._state = ComplianceStateValues.Inactive; }
            this.SetStateColor(this._state);

            if ( this._state != 0) { this.HelpText = this._compliancehandler.GetActiveValidationMessages(); }
            else { this.HelpText = this._okHelpText; }

            this.NotifyUpdate();
        }

        private void SetDefaults()
        {
            this.SetStateColor(ComplianceStateValues.OK);
            this.ControlFormatting.Padding = new Thickness(0, 0, 0, 0);
            this.ControlFormatting.Margin = new Thickness(2, 1, 2, 1);
            this.ControlFormatting.VerticalAlignment = VerticalAlignment.Center;
            this.IconHeight = 15;
            this.IconWidth = 15;
            this._showvalueinpopup = false;
        }

        private void SetStateColor(int State)
        {
            this._state = State;
            switch (State)
            {
                case ComplianceStateValues.Inactive:
                    this.FillColor.Color = Colors.LightGray;
                    break;
                case ComplianceStateValues.OK:
                    this.FillColor.Color = Colors.Green;
                    break;               
                case ComplianceStateValues.Warning:
                    this.FillColor.Color = Colors.Orange;
                    break;
                case ComplianceStateValues.Error:
                    this.FillColor.Color = Colors.Red;
                    break;
                case ComplianceStateValues.Invalid:
                    this.FillColor.Color = Colors.Red;
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