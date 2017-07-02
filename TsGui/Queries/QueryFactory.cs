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

        public static IQuery GetQueryObject(XElement InputXml, IDirector director, ILinkTarget owner )
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
                    case "GuiVariable":
                        return new GuiVariableQuery(InputXml, director, owner);
                    case "EnvironmentVariable":
                        return new EnvironmentVariableQuery(InputXml, director.EnvironmentController.SccmConnector);
                    case "OptionValue":
                        return new OptionValueQuery(InputXml, director, owner);
                    case "IfElse":
                        return new IfElseQuery(InputXml, director, owner);
                    case "Combined":
                        return new CombinedQuery(InputXml, director, owner);
                    case "Value":
                        return new ValueOnlyQuery(InputXml);
                    case "LinkFalse":
                        return GetLinkTrueFalseOnlyQuery(InputXml.Value, director, owner, false);
                    case "LinkTo":
                        return GetLinkToQuery(InputXml.Value, director, owner);
                    case "LinkTrue":
                        return GetLinkTrueFalseOnlyQuery(InputXml.Value, director, owner, true);
                    default:
                        throw new TsGuiKnownException("Invalid type specified in query", InputXml.ToString());
                }
            }

            else if (InputXml.Name.ToString() == "Value")
            { return new ValueOnlyQuery(InputXml); }

            else
            { return null; }
        }

        #region
        //      <Query Type = "OptionValue">
        //          <ID Name = "TestLink1"/>
        //      </Query>
        #endregion
        public static IQuery GetLinkToQuery(string SourceID, IDirector controller, ILinkTarget owner)
        {
            XElement sourcequeryx = new XElement("Query");
            sourcequeryx.Add(new XAttribute("Type", "OptionValue"));

            XElement idx = new XElement("ID");
            idx.Add(new XAttribute("Name", SourceID));
            sourcequeryx.Add(idx);

            return new OptionValueQuery(sourcequeryx, controller, owner);
        }

        #region
        //  <Query Type="IfElse">
        //      <Source>
        //          <Query Type="OptionValue">
        //              <ID Name="TestLink1"/>
        //          </Query>
        //      </Source>
        //      <Ruleset>
        //          <Rule Type="Equals">TRUE/FALSE</Rule>
        //      </Ruleset>
        //      <Result>
        //          <Query Type = "OptionValue">
        //              <ID Name = "TestLink1"/>
        //          </Query>
        //      </Result>
        //  </Query>
        #endregion
        public static IQuery GetLinkTrueFalseOnlyQuery(string SourceID, IDirector controller, ILinkTarget owner, bool truefalse)
        {
            
            XElement sharedqueryx = new XElement("Query");
            sharedqueryx.Add(new XAttribute("Type", "OptionValue"));
            XElement idx = new XElement("ID");
            idx.Add(new XAttribute("Name", SourceID));
            sharedqueryx.Add(idx);

            XElement rulex = new XElement("Rule",truefalse.ToString());
            rulex.Add(new XAttribute("Type", "Equals"));
            XElement rulesetx = new XElement("Ruleset",rulex);

            XElement ifx = new XElement("IF");            
            XElement sourcex = new XElement("Source", sharedqueryx);
            XElement resultx = new XElement("Result", sharedqueryx);
            ifx.Add(sourcex);
            ifx.Add(rulesetx);
            ifx.Add(resultx);

            XElement ifelsex = new XElement("Query", ifx);
            ifelsex.Add(new XAttribute("Type", "IfElse"));

            return new IfElseQuery(ifelsex, controller, owner);
        }
    }
}
