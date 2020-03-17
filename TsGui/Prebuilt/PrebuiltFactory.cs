using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TsGui.Diagnostics.Logging;
using TsGui.Prebuilt.Options;
using TsGui.Diagnostics;

namespace TsGui.Prebuilt
{
    public static class PrebuiltFactory
    {
        public static XElement GetPrebuiltXElement(XElement OptionXml)
        {
            XAttribute xtype = OptionXml.Attribute("Type");
            XAttribute xprebuilt = OptionXml.Attribute("Prebuilt");
            string name = OptionXml.Name.LocalName;
            if (xprebuilt == null)
            {
                LoggerFacade.Trace("No Prebuilt attribute set");
            }
            else
            {
                string prebuilttype = xprebuilt.Value.ToUpper();

                switch (prebuilttype) 
                {
                    case "DISKINDEX":
                        if (PrebuiltDisk.IsSupported(xtype.Value))
                        { return PrebuiltDisk.GetXml(); }
                        else { throw new TsGuiKnownException(string.Format("Prebuilt type {0} not supported on {1}", xprebuilt.Value, xtype.Value), null); }

                    case "POWER":
                        if (PrebuiltPower.IsSupported(xtype.Value))
                        { return PrebuiltPower.GetXml(); }
                        else { throw new TsGuiKnownException(string.Format("Prebuilt type {0} not supported on {1}", xprebuilt.Value, xtype.Value), null); }
                    default:
                        break;
                }

            }

            return null;
        }
    }
}
