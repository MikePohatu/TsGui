using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TsGui;

namespace TsGui.Prebuilt.Options
{
    public static class PrebuiltPower
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
