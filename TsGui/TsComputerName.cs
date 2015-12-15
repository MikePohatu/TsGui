using System;
using System.Xml.Linq;
using System.Diagnostics;

namespace TsGui
{
    public class TsComputerName: TsFreeText
    {
        public TsComputerName(XElement SourceXml, MainController RootController):base (RootController)
        {
            base._name = "OSDComputerName";
            base._label = "Computer Name:";
            base._minlength = 1;
            base._maxlength = 15;
            base._disallowedChars = "`~!@#$%^&*()+={}[]\\/|<>,.? :;\"'";
            this.LoadXml(SourceXml);
            base.Build();
        }

        public new void LoadXml(XElement SourceXml)
        {
            if (String.IsNullOrEmpty(SourceXml.Value) == true)
            {
                //setup the default values e.g
                //<Disallowed>
                //    <Characters>`~!@#$%^&*()+={}[]\/|<>,.? :;"'</Characters>
                //</Disallowed>
                //<Variable>OSDComputerName</Variable>
                //<Label>Computer name:</Label>
                //<DefaultValue>
                //	<EnvironmentVariable>
                //		<Ignore>MINNT</Ignore>
                //      _SMSTSMachineName
                //      </EnvironmentVariable>
                //	<WmiQuery>
                //		<Query>SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure</Query>
                //      <Ignore>No Asset Tag</Ignore>
                //	</WmiQuery>
                //	<WmiQuery>
                //		<Query>SELECT SerialNumber FROM Win32_BIOS</Query>
                //	</WmiQuery>
                //</DefaultValue>
                XElement x = new XElement("ComputerName");

                XElement def = new XElement("DefaultValue");

                XElement envvar = new XElement("EnvironmentVariable", "_SMSTSMachineName");
                envvar.Add(new XElement("Ignore", "MINNT"));

                XElement assettag = new XElement("WmiQuery");
                assettag.Add(new XElement("Query", "SELECT SMBIOSAssetTag FROM Win32_SystemEnclosure"));
                assettag.Add(new XElement("Ignore", "No Asset Tag"));

                XElement serial = new XElement("WmiQuery");
                serial.Add(new XElement("Query", "SELECT SerialNumber FROM Win32_BIOS"));

                
                def.Add(envvar);
                def.Add(assettag);
                def.Add(serial);

                x.Add(def);

                Debug.WriteLine("TsComputerName XML: " + x);

                base.LoadXml(x);
                
            }
            else { base.LoadXml(SourceXml); }
        }
    }
}
