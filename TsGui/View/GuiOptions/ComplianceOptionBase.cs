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

// ComplianceOptionBase.cs - base class for compliance guioptions

using System;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;
using TsGui.Validation;
using TsGui.Queries;
using TsGui.Linking;
using Core.Diagnostics;
using MessageCrap;
using TsGui.View.Layout;
using System.Threading.Tasks;

namespace TsGui.View.GuiOptions
{
    public abstract class ComplianceOptionBase : GuiOptionBase, IGuiOption, IValidationGuiOption, ILinkTarget
    {
        protected string _value;
        protected double _iconheight;
        protected double _iconwidth;
        protected SolidColorBrush _fillcolor;
        protected SolidColorBrush _strokecolor;
        protected int _state;
        protected ValidationToolTipHandler _validationtooltiphandler;
        public ValidationHandler ValidationHandler { get { return null; } }
        protected ComplianceHandler _compliancehandler;
        protected string _validationtext;
        protected IComplianceRoot _rootelement;
        protected bool _showvalueinpopup;
        protected string _okHelpText;

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
        public SolidColorBrush StrokeColor
        {
            get { return this._strokecolor; }
            set { this._strokecolor = value; this.OnPropertyChanged(this, "StrokeColor"); }
        }
        public bool IsValid { get { return this.Validate(); } }
        public override string CurrentValue { get { return ComplianceStateValues.ToString(this._state); } }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }
        public override Variable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new Variable(this.VariableName, this.CurrentValue, this.Path); }
            }
        }

        //constructor
        public ComplianceOptionBase(ParentLayoutElement Parent): base (Parent)
        {
            this.Label = new TsLabelUI();
            this._rootelement = this.GetComplianceRootElement();
            if (this._rootelement == null)
            {
                throw new KnownException("There is problem in the compliance tree. Root is null", string.Empty);
            }
            this._rootelement.ComplianceRetry += this.OnComplianceRetry;

            this.FillColor = new SolidColorBrush(Colors.Blue);
            this.StrokeColor = new SolidColorBrush(Colors.Blue);
            this._compliancehandler = new ComplianceHandler(this);
            this._validationtooltiphandler = new ValidationToolTipHandler(this);
            this._querylist = new QueryPriorityList(this);
            this.UserControl.DataContext = this;
            this.SetDefaults();      
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

        public async void OnComplianceRetry(IComplianceRoot o, EventArgs e)
        {
            await this.UpdateLinkedValueAsync(null);
        }

        public override async Task UpdateLinkedValueAsync(Message message)
        {
            await this._querylist.ProcessAllQueriesAsync(message);
            await this.ProcessQueryAsync(message);
            this.Validate();
            this.InvokeToggleEvent();
            LinkingHub.Instance.SendUpdateMessage(this, message);
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        {
            await this.UpdateLinkedValueAsync(message);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            XElement x;

            this.LoadLegacyXml(InputXml);
            this._okHelpText = this.HelpText;
            this._compliancehandler.AddCompliances(InputXml.Elements("Compliance"));
            this._showvalueinpopup = XmlHandler.GetBoolFromXml(InputXml, "ShowComplianceValue", this._showvalueinpopup);
            this._showvalueinpopup = XmlHandler.GetBoolFromXml(InputXml, "PopupShowValue", this._showvalueinpopup);

            //wrap the query in another to make it suitable for the querylist. 
            x = InputXml.Element("GetValue");
            if (x != null) { this._querylist.LoadXml(x); }

            //load into the validation tooltip
            this._validationtooltiphandler.LoadXml(InputXml);
        } 

        protected async Task ProcessQueryAsync(Message message)
        {
            this._value = (await this._querylist.GetResultWrangler(message))?.GetString();
        }

        protected void UpdateState()
        {
            if (this.IsActive == true) { this._state = this._compliancehandler.EvaluateComplianceState(this._value); }
            else { this._state = ComplianceStateValues.Inactive; }
            

            if ( this._state != 0) { this.HelpText = this._compliancehandler.GetActiveValidationMessages(); }
            else { this.HelpText = this._okHelpText; }

            
            this.NotifyViewUpdate();
            this.UpdateView();
        }

        protected abstract void UpdateView();

        private void SetDefaults()
        {
            this.ControlStyle.Padding = new Thickness(0, 0, 0, 0);
            this.ControlStyle.Margin = new Thickness(2, 1, 2, 1);
            this.ControlStyle.VerticalAlignment = VerticalAlignment.Center;
            this.IconHeight = 15;
            this.IconWidth = 15;
            this._showvalueinpopup = false;
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
                        this.ControlStyle.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "LEFT":
                        this.ControlStyle.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "CENTER":
                        this.ControlStyle.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "STRETCH":
                        this.ControlStyle.HorizontalAlignment = HorizontalAlignment.Stretch;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
;