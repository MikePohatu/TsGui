//    Copyright (C) 2017 Mike Pohatu

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

using System.Xml.Linq;
using TsGui.Diagnostics;
using TsGui.Linking;

namespace TsGui.Queries
{
    public static class QueryFactory
    {

        public static IQuery GetQueryObject(XElement InputXml, MainController controller, ILinkTarget owner )
        {
            if (InputXml == null) { return null; }

            XAttribute xtype;

            xtype = InputXml.Attribute("Type");
            if (xtype != null)
            {
                switch (xtype.Value)
                {
                    case "Wmi":
                        return new WmiQuery(InputXml);
                    case "EnvironmentVariable":
                        return new EnvironmentVariableQuery(InputXml, controller.EnvironmentController.SccmConnector);
                    case "OptionValue":
                        return new OptionValueQuery(InputXml, controller, owner);
                    case "IF":
                        return new Conditional(InputXml, controller, owner);
                    case "Value":
                        return new ValueOnly(InputXml);
                    default:
                        throw new TsGuiKnownException("Invalid type specified in query", InputXml.ToString());
                }
            }
            else if (InputXml.Name.ToString() == "IF")
            { return new Conditional(InputXml, controller, owner); }

            else if (InputXml.Name.ToString() == "Value")
            { return new ValueOnly(InputXml); }

            else
            { return null; }
        }

        public static OptionValueQuery GetDefaultLinkToQuery(string SourceID, MainController controller, ILinkTarget owner)
        {
            //  < Source >
            //      < Query Type = "OptionValue" >
            //          < ID Name = "TestLink1" />
            //      </ Query >
            //  </ Source >
            //  < Result >
            //      < Query Type = "OptionValue" >
            //          < ID Name = "TestLink1" />
            //      </ Query >
            //  </ Result >
            

            XElement sourcequeryx = new XElement("Query");
            sourcequeryx.Add(new XAttribute("Type", "OptionValue"));
            XElement idx = new XElement("ID");
            idx.Add(new XAttribute("Name", SourceID));
            sourcequeryx.Add(idx);

            XElement ifx = new XElement("IF");
            XElement sourcex = new XElement("Source",sourcequeryx);
            XElement resultx = new XElement("Result", sourcequeryx);
            ifx.Add(sourcex);
            ifx.Add(resultx);

            return new OptionValueQuery(sourcequeryx, controller, owner);
        }
    }
}
