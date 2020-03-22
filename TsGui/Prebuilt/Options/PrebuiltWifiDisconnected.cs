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
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Prebuilt.Options
{
	public static class PrebuiltWifiDisconnected
	{
		public static List<string> SupportedTypes { get; private set; } = new List<string>() { "TICKCROSS", "TRAFFICLIGHT" };

		public static bool IsSupported(string type)
		{
			return SupportedTypes.Contains(type.ToUpper());
		}

		public static XElement GetXml()
		{
			string xml = @"
				<Prebuilt>
					<Variable>Compliance_WifiStatus</Variable>
					<Label>WiFi connection</Label>
					<ShowComplianceValue>FALSE</ShowComplianceValue>
					<SetValue>
						<Query Type=""Wmi"">
							<Wql>SELECT * FROM Win32_NetworkAdapter WHERE (AdapterType=""Ethernet 802.3"") AND (NetConnectionStatus = 2)</Wql>
							<Property Name=""Name""/>
							<Separator></Separator>
						</Query>
					</SetValue>
					<Compliance>
						<Message>Please disconnect the wifi</Message>	   
						<DefaultState>Warning</DefaultState>
						<Invalid>
							<Rule Type=""Contains"">Wireless</Rule>
							<Rule Type=""Contains"">Wifi</Rule>
							<Rule Type=""Contains"">WLAN</Rule>
						</Invalid>		 
					</Compliance>
				</Prebuilt>";

			XElement x = XElement.Parse(xml);
			return x;
		}


	}
}
