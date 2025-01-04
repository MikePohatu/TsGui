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
    public static class PrebuiltDiskIndex
    {
		public static List<string> SupportedTypes { get; private set; } = new List<string>() { "DROPDOWNLIST" };

		public static bool IsSupported(string type)
		{
			return SupportedTypes.Contains(type.ToUpper());
		}

		public static XElement GetXml()
		{
			string xml = @"
				<Prebuilt>
					<Variable>OSDDiskIndex</Variable>
					<Label>Disk</Label>
					<SetValue>
						<Value>0</Value>
					</SetValue>

					<Query Type=""Wmi"">
						<Wql>SELECT Index,Caption,Size FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
						<NameSpace>root\CIMV2</NameSpace>
						<Property Name=""Index""/>
						<Property Name=""Index"">
							<Prefix>ID: </Prefix>
						</Property>
						<Property Name=""Size"">
							<Calculate DecimalPlaces=""2"">VALUE/1073741824</Calculate>
							<Prefix></Prefix>
							<Append>GB</Append>
						</Property>
						<Property Name=""Caption"" />
						<Separator> - </Separator>
					</Query>
				</Prebuilt>";

			XElement x = XElement.Parse(xml);
			return x;
		}

		
	}
}
