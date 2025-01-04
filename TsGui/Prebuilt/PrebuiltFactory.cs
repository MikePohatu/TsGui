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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Core.Logging;
using TsGui.Prebuilt.Options;
using Core.Diagnostics;

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
                Log.Trace("No Prebuilt attribute set");
            }
            else
            {
                string prebuilttype = xprebuilt.Value.ToUpper();

                switch (prebuilttype) 
                {
                    case "DISKINDEX":
                        if (PrebuiltDiskIndex.IsSupported(xtype.Value))
                        { return PrebuiltDiskIndex.GetXml(); }
                        else { throw new KnownException(string.Format("Prebuilt type {0} not supported on {1}", xprebuilt.Value, xtype.Value), null); }

                    case "POWERCONNECTED":
                        if (PrebuiltPowerConnected.IsSupported(xtype.Value))
                        { return PrebuiltPowerConnected.GetXml(); }
                        else { throw new KnownException(string.Format("Prebuilt type {0} not supported on {1}", xprebuilt.Value, xtype.Value), null); }
                    case "WIFIDISCONNECTED":
                        if (PrebuiltPowerConnected.IsSupported(xtype.Value))
                        { return PrebuiltWifiDisconnected.GetXml(); }
                        else { throw new KnownException(string.Format("Prebuilt type {0} not supported on {1}", xprebuilt.Value, xtype.Value), null); }
                    default:
                        break;
                }

            }

            return null;
        }
    }
}
