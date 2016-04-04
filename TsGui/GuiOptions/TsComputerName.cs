//setup the default values for a computer name i.e.
//<DefaultValue>
//	<EnvironmentVariable>
//		<Ignore>MINNT</Ignore>
//      _SMSTSMachineName
//  </EnvironmentVariable>
//	<EnvironmentVariable>
//		<Ignore>MINNT</Ignore>
//      ComputerName
//  </EnvironmentVariable>
//	<WmiQuery>
//		<Query>SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure</Query>
//      <Ignore>No Asset Tag</Ignore>
//	</WmiQuery>
//	<WmiQuery>
//		<Query>SELECT SerialNumber FROM Win32_BIOS</Query>
//	</WmiQuery>
//</DefaultValue>

using System;
using System.Xml.Linq;
using System.Diagnostics;

namespace TsGui
{
    public class TsComputerName: TsFreeText
    {
        public TsComputerName(XElement SourceXml, MainController RootController):base (RootController)
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
                
                XElement x = new XElement("ComputerName");
                XElement def = new XElement("DefaultValue");

                XElement envvar = new XElement("EnvironmentVariable", "_SMSTSMachineName");
                envvar.Add(new XElement("Ignore", "MINNT"));

                XElement compName = new XElement("EnvironmentVariable", "computername");
                envvar.Add(new XElement("Ignore", "MINNT"));

                XElement assettag = new XElement("WmiQuery");
                assettag.Add(new XElement("Query", "SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure"));
                assettag.Add(new XElement("Ignore", "No Asset Tag"));

                XElement serial = new XElement("WmiQuery");
                serial.Add(new XElement("Query", "SELECT SerialNumber FROM Win32_BIOS"));

                
                def.Add(envvar);
                def.Add(compName);
                def.Add(assettag);
                def.Add(serial);

                x.Add(def);
                

                //Debug.WriteLine("TsComputerName XML: " + Environment.NewLine + x);

                base.LoadXml(x);
                
            }
            else { base.LoadXml(SourceXml); }
        }
    }
}
