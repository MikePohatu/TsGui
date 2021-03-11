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

// TsComputerName.cs - class to create a pre-created TsFreeText object. Designed
// specifically for Windows computer names. Effective FreeText XML below. 

//setup the default values for a computer name i.e.
//	<GuiOption Type="FreeText" MaxLength="15">
//		<Variable>OSDComputerName_FullQuery</Variable>
//		<Label>Computer Name:</Label>
//		<HelpText>Enter a computer name for the device</HelpText>
//		<CharacterCasing>Normal</CharacterCasing> <!-- Sets and enforces the case of text. Options are Normal, Upper, and Lower -->

//		<!-- Query for the default value. Return the first valid value. Note
//		the Ignore values for evaluating the value returned by the query -->
//		<SetValue UseCurrent="False">
//			<Query Type="EnvironmentVariable">
//				<Variable Name="OSDComputerName"/>
//				<Ignore>MINNT</Ignore>
//				<Ignore>MINWIN</Ignore>						
//			</Query>				
//			<Query Type="EnvironmentVariable">
//				<Variable Name="_SMSTSMachineName"/>
//				<Ignore>MINNT</Ignore>
//				<Ignore>MINWIN</Ignore>					
//			</Query>					
//			<Query Type="EnvironmentVariable">
//				<Variable Name="ComputerName"/>		
//				<Ignore>MINNT</Ignore>
//				<Ignore>MINWIN</Ignore>						
//			</Query>					
//			<Query Type="Wmi">
//				<Wql>SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure</Wql>
//				<Ignore>No Asset Tag</Ignore>
//              <Ignore>NoAssetTag</Ignore>
//              <Ignore>No Asset Information</Ignore>
//              <Ignore>NoAssetInformat</Ignore>
//			</Query>					
//			<Query Type="Wmi">
//				<Wql>SELECT SerialNumber FROM Win32_BIOS</Wql>
//			</Query>
//		</SetValue>

//      <!-- For validation examples, see Config_Validation.xml and the associated how-to video -->
//      <Validation ValidateEmpty="TRUE">
//          <MaxLength>15</MaxLength>
//          <MinLength>1</MinLength>
//          <Invalid>
//              <Rule Type="Characters">`~!@#$%^*()+={}[]\\/|,.? :;"'>&amp;&lt;</Rule>
//          </Invalid>
//      </Validation>
//  </GuiOption>	

using System.Xml.Linq;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions
{
    public class TsComputerName : TsFreeText
    {
        public TsComputerName(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            base.LoadXml(this.BuildDefaultXml());
            base.LoadXml(InputXml);
        }

        public XElement BuildDefaultXml()
        {
            XElement osdvar = new XElement("Query");
            osdvar.Add(new XAttribute("Type", "EnvironmentVariable"));
            osdvar.Add(new XElement("Variable", "OSDComputerName"));
            osdvar.Add(new XElement("Ignore", "MININT"));
            osdvar.Add(new XElement("Ignore", "MINWIN"));

            XElement envvar = new XElement("Query");
            envvar.Add(new XAttribute("Type", "EnvironmentVariable"));
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
            assettag.Add(new XElement("Ignore", "NoAssetTag"));
            assettag.Add(new XElement("Ignore", "NoAssetInformation"));
            assettag.Add(new XElement("Ignore", "NoAssetInformat"));
            assettag.Add(new XElement("Ignore", "No Asset Information"));

            XElement serial = new XElement("Query");
            serial.Add(new XAttribute("Type", "Wmi"));
            serial.Add(new XElement("Wql", "SELECT SerialNumber FROM Win32_BIOS"));

            XElement validation = new XElement("Validation");
            validation.Add(new XAttribute("ValidateEmpty", "TRUE"));

            XElement invalid = new XElement("Invalid");
            XElement rule = new XElement("Rule", "`~!@#$%^&*()+={}[]\\/|<>,.? :;\"'");
            rule.Add(new XAttribute("Type", "Characters"));
            invalid.Add(rule);
            validation.Add(invalid);
            validation.Add(new XElement("MinLength", "1"));
            validation.Add(new XElement("MaxLength", "15"));

            XElement def = new XElement("SetValue");

            def.Add(new XAttribute("UseCurrent", "False"));
            def.Add(osdvar);
            def.Add(envvar);
            def.Add(compName);
            def.Add(assettag);
            def.Add(serial);

            XElement x = new XElement("GuiOption");
            x.Add(new XAttribute("Type", "FreeText"));
            x.Add(def);
            x.Add(validation);
            x.Add(new XElement("Variable", "OSDComputerName"));
            x.Add(new XElement("Label", "Computer Name:"));
            x.Add(new XElement("HelpText", "Enter a computer name for the device"));
            x.Add(new XAttribute("MaxLength", "15"));

            return x;
        }
    }
}