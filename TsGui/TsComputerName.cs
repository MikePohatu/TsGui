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
            base._invalidchars = "/\\[]\":;|&lt;>+=,?* _";
            this.LoadXml(SourceXml);
            base.Build();
        }

        public new void LoadXml(XElement SourceXml)
        {
            if (String.IsNullOrEmpty(SourceXml.Value) == true)
            {
                //base.LoadXml();
                XElement x = new XElement("ComputerName",this._invalidchars);
                x.Add(new XElement("Variable", "OSDComputerName"));
                x.Add(new XElement("Label", "Computer Name:"));

                XElement def = new XElement("DefaultValue");

                XElement envvar = new XElement("EnvironmentVariable", "_SMSTSMachineName");
                //envvar.Add(new XElement("Ignore", "MINNT"));

                XElement wmiquery = new XElement("WmiQuery");
                wmiquery.Add(new XElement("Query", "SELECT SerialNumber FROM Win32_BIOS"));

                
                def.Add(envvar);
                def.Add(wmiquery);

                x.Add(def);

                Debug.WriteLine("TsComputerName XML: " + x);

                base.LoadXml(x);
                //< Invalid >/\[]":;|&lt;>+=,?* _</Invalid>
                //<Variable>OSDComputerName</Variable>
                //<Label>Computer name:</Label>
                //<DefaultValue>
                //	<EnvironmentVariable>
                //		<Ignore>MINNT</Ignore>
                //                    _SMSTSMachineName
                //                </EnvironmentVariable>
                //	<WmiQuery>
                //		<Query>SELECT SerialNumber FROM Win32_BIOS</Query>
                //	</WmiQuery>
                //</DefaultValue>
            }
            else { base.LoadXml(SourceXml); }
        }
    }
}
