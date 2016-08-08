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

// TsComputerName.cs - class to create a pre-created TsFreeText object. Designed
// specifically for Windows computer names. Effective FreeText XML below. 

//setup the default values for a computer name i.e.
//<DefaultValue>
//	<Query Type=EnvironmentVariable>
//		<Ignore>MINNT</Ignore>
//      <Variable>_SMSTSMachineName</Variable>
//  </Query>
//	<Query Type=EnvironmentVariable>
//		<Ignore>MINNT</Ignore>
//      <Variable>ComputerName</Variable>
//  </Query>
//	<Query Type=Wmi>
//		<Wql>SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure</Wql>
//      <Ignore>No Asset Tag</Ignore>
//	</Query>
//	<Query Type=Wmi>
//		<Wql>SELECT SerialNumber FROM Win32_BIOS</Wql>
//	</Query>
//</DefaultValue>

using System;
using System.Xml.Linq;
//using System.Diagnostics;

namespace TsGui
{
    public class TsComputerName: TsFreeText
    {
        public TsComputerName(XElement SourceXml, MainController RootController) : base(RootController)
        {
            this.VariableName = "OSDComputerName";
            this.LabelText = "Computer Name:";
            this.HelpText = "Enter a computer name for the device";
            this.MinLength = 1;
            this.MaxLength = 15;
            this.DisallowedCharacters = "`~!@#$%^&*()+={}[]\\/|<>,.? :;\"'";
            this.LoadXml(SourceXml);
        }

        public new void LoadXml(XElement SourceXml)
        {
            if (String.IsNullOrEmpty(SourceXml.Value) == true)
            {
                XElement osdvar = new XElement("Query");
                osdvar.Add(new XAttribute("Type", "EnvironmentVariable"));
                osdvar.Add(new XElement("Variable", "OSDComputerName"));
                osdvar.Add(new XElement("Ignore", "MININT"));
                osdvar.Add(new XElement("Ignore", "MINWIN"));

                XElement envvar = new XElement("Query");
                envvar.Add(new XAttribute("Type","EnvironmentVariable"));
                envvar.Add(new XElement("Variable", "_SMSTSMachineName"));
                envvar.Add(new XElement("Ignore", "MININT"));
                envvar.Add(new XElement("Ignore", "MINWIN"));

                XElement compName = new XElement("Query");
                compName.Add(new XAttribute("Type", "EnvironmentVariable"));
                compName.Add(new XElement("Variable", "computername"));
                compName.Add(new XElement("Ignore", "MININT"));
                compName.Add(new XElement("Ignore", "MINWIN"));

                XElement assettag = new XElement("Query");
                assettag.Add(new XAttribute("Type", "Wmi"));
                assettag.Add(new XElement("Wql", "SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure"));
                assettag.Add(new XElement("Ignore", "No Asset Tag"));

                XElement serial = new XElement("Query");
                serial.Add(new XAttribute("Type", "Wmi"));
                serial.Add(new XElement("Wql", "SELECT SerialNumber FROM Win32_BIOS"));

                XElement x = new XElement("ComputerName");
                XElement def = new XElement("DefaultValue");

                def.Add(new XAttribute("UseCurrent", "False"));
                def.Add(osdvar);
                def.Add(envvar);
                def.Add(compName);
                def.Add(assettag);
                def.Add(serial);

                x.Add(def);

                base.LoadXml(x);               
            }
            else { base.LoadXml(SourceXml); }
        }
    }
}
