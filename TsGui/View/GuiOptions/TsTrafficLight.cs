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

// TsTrafficLight.cs - Displays an icon indicating whether a device is compliant with
// a specific condition

using System.Xml.Linq;
using TsGui.Validation;
using System.Windows.Media;
using System;

namespace TsGui.View.GuiOptions
{
    public class TsTrafficLight: ComplianceOptionBase
    {

        //constructor
        public TsTrafficLight(XElement InputXml, TsColumn Parent, IDirector MainController): base (InputXml, Parent, MainController)
        {           
            this.Control = new TsTrafficLightUI();
            this._validationtooltiphandler.SetTarget(this.Control);
            this.LoadXml(InputXml);
            this.ProcessQuery();
            this.Validate();
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