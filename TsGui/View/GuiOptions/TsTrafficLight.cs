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

// TsTrafficLight.cs - Displays an icon indicating whether a device is compliant with
// a specific condition

using System.Xml.Linq;
using TsGui.Validation;
using System.Windows.Media;
using System;
using TsGui.View.Symbols;

namespace TsGui.View.GuiOptions
{
    public class TsTrafficLight: ComplianceOptionBase
    {

        //constructor
        public TsTrafficLight(XElement InputXml, TsColumn Parent): base (Parent)
        {           
            this.Control = new TsTrafficLightUI();
            this.LoadXml(InputXml);
            this.RefreshValue();
        }

        public TsTrafficLight(TsColumn Parent) : base(Parent)
        {
            this.Control = new TsTrafficLightUI();
            this._validationtooltiphandler.SetTarget(this.Control);
        }

        public TsTrafficLight(TsColumn Parent, IDirector MainController) : base(Parent)
        {
            this.Control = new TsTrafficLightUI();
            this._validationtooltiphandler.SetTarget(this.Control);
        }

        private void SetDefaults()
        {
            this.SetStateColor(ComplianceStateValues.OK);
        }

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
        }

        protected override void UpdateView()
        {
            this.SetStateColor(this._state);
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
    }
}