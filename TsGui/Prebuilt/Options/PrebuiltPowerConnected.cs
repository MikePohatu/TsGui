#region license
// Copyright (c) 2025 Mike Pohatu
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
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Prebuilt.Options
{
    public static class PrebuiltPowerConnected
    {
		public static List<string> SupportedTypes { get; private set; } = new List<string>() { "TICKCROSS", "TRAFFICLIGHT" };

		public static bool IsSupported(string type)
		{
			return SupportedTypes.Contains(type.ToUpper());
		}

		public static XElement GetXml()
        {
            string xml= @"
				<Prebuilt>
					<Variable>Compliance_PowerStatus</Variable>
					<Label>Power connection</Label>
					<ShowComplianceValue>FALSE</ShowComplianceValue>
					<SetValue>
						<Query Type=""Wmi"">
							<Wql>SELECT BatteryStatus FROM Win32_Battery</Wql>
							<Property Name=""BatteryStatus"" />
							<Separator></Separator>
						</Query>
					</SetValue>
		 
					<Compliance>
						<Message>Please connect the power</Message>
						<DefaultState>Warning</DefaultState>
						<OK>
							<Rule Type=""Contains"">2</Rule>
							<Rule Type=""Equals"">*NULL</Rule>
						</OK>
					</Compliance>
				</Prebuilt>";

			XElement x = XElement.Parse(xml);
			return x;
        }
    }
}
