using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Prebuilt.Options
{
    public static class PrebuiltDisk
    {
		public static List<string> SupportedTypes { get; private set; } = new List<string>() { "DROPDOWNLIST" };

		public static bool IsSupported(string type)
		{
			return SupportedTypes.Contains(type.ToUpper());
		}

		public static XElement GetXml()
		{
			string xml = @"<DiskIndex>
					<Variable>OSDDiskIndex</Variable>
					<Label>Disk</Label>
					<SetValue>
						<Value>0</Value>
					</SetValue>

					<Query Type=""Wmi"">
						<Wql>select Index,Caption,Size FROM Win32_DiskDrive where MediaType='Fixed hard disk media'</Wql>
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
						<Separator>, </Separator>
					</Query>
				</DiskIndex>";

			XElement x = XElement.Parse(xml);
			return x;
		}

		
	}
}
