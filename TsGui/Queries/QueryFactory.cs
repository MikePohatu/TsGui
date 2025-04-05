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
using System.Xml.Linq;
using Core.Diagnostics;
using TsGui.Linking;
using TsGui.Queries.ActiveDirectory;

namespace TsGui.Queries
{
    public static class QueryFactory
    {

        public static IQuery GetQueryObject(XElement InputXml, ILinkTarget linktarget)
        {
            if (InputXml == null) { return null; }

            XAttribute xtype;

            xtype = InputXml.Attribute("Type");
            if (xtype != null)
            {
                switch (xtype.Value.ToLower())
                {
                    case "wmi":
                        return new WmiQuery(InputXml, linktarget);
                    case "guivariable":
                        return new GuiVariableQuery(InputXml, linktarget);
                    case "environmentvariable":
                        return new EnvironmentVariableQuery(InputXml, linktarget);
                    case "optionvalue":
                        return new OptionValueQuery(InputXml, linktarget);
                    case "ifelse":
                        return new IfElseQuery(InputXml, linktarget);
                    case "combined":
                        return new CombinedQuery(InputXml, linktarget);
                    case "compare":
                        return new CompareQuery(InputXml, linktarget);
                    case "value":
                        return new ValueOnlyQuery(InputXml);
                    case "listvalue":
                        return new ListValueQuery(InputXml);
                    case "linkto":
                        return GetLinkToQuery(InputXml, linktarget);
                    //Set to true when source is true
                    case "linktrue":
                        return GetLinkTrueFalseOnlyQuery(InputXml, linktarget, true, false);
                    //set to false when source is false
                    case "linkfalse":
                        return GetLinkTrueFalseOnlyQuery(InputXml, linktarget, false, false);
                    //set to true when source is false
                    case "notlinkfalse":
                        return GetLinkTrueFalseOnlyQuery(InputXml, linktarget, false, true);
                    //set to false when source is true
                    case "notlinktrue":
                        return GetLinkTrueFalseOnlyQuery(InputXml, linktarget, true, true);
                    case "adgroupmembers":
                        return new ADGroupMembersQuery(InputXml, linktarget);
                    case "adou":
                        return new ADOrgUnitQuery(InputXml, linktarget);
                    case "adougroups":
                        return new ADOrgUnitGroupsQuery(InputXml, linktarget);
                    case "registry":
                        return new RegistryQuery(InputXml, linktarget);
                    case "powershell":
                        return new PoshQuery(InputXml, linktarget);
                    case "posh":
                        return new PoshQuery(InputXml, linktarget);
                    default:
                        throw new KnownException("Invalid type specified in query", InputXml.ToString());
                }
            }

            else if (InputXml.Name.ToString() == "Value")
            { return new ValueOnlyQuery(InputXml); }

            else if (InputXml.Name.ToString() == "ListValue")
            { return new ListValueQuery(InputXml); }

            else
            { return null; }
        }

        #region
        //      <Query Type = "OptionValue">
        //          <ID Name = "TestLink1"/>
        //      </Query>
        #endregion
        public static IQuery GetLinkToQuery(XElement InputXml, ILinkTarget linktarget)
        {
            string sourceid = InputXml.Value;
            XElement sourcequeryx = new XElement("Query");
            sourcequeryx.Add(new XAttribute("Type", "OptionValue"));
            bool init = XmlHandler.GetBoolFromXml(InputXml, "InitLink", true);
            sourcequeryx.Add(new XAttribute("InitLink", init.ToString()));

            XElement idx = new XElement("ID");
            idx.Add(new XAttribute("Name", sourceid));
            sourcequeryx.Add(idx);

            return new OptionValueQuery(sourcequeryx, linktarget);
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
        //          <Value>TRUE/FALSE</Value>
        //      </Result>
        //  </Query>
        #endregion
        public static IQuery GetLinkTrueFalseOnlyQuery(XElement InputXml, ILinkTarget linktarget, bool truefalse, bool invert)
        {
            string sourceid = InputXml.Value;
            XElement sourceqx = new XElement("Query");
            sourceqx.Add(new XAttribute("Type", "OptionValue"));
            XElement idx = new XElement("ID");
            idx.Add(new XAttribute("Name", sourceid));
            sourceqx.Add(idx);

            bool init = XmlHandler.GetBoolFromXml(InputXml, "InitLink", true);
            sourceqx.Add(new XAttribute("InitLink", init.ToString()));

            XElement rulex = new XElement("Rule",truefalse.ToString().ToUpper());
            rulex.Add(new XAttribute("Type", "Equals"));
            XElement rulesetx = new XElement("Ruleset",rulex);

            XElement resultqx = new XElement("Value", (truefalse ^ invert).ToString().ToUpper());

            XElement ifx = new XElement("IF");            
            XElement sourcex = new XElement("Source", sourceqx);
            XElement resultx = new XElement("Result", resultqx);
            ifx.Add(sourcex);
            ifx.Add(rulesetx);
            ifx.Add(resultx);

            XElement ifelsex = new XElement("Query", ifx);
            ifelsex.Add(new XAttribute("Type", "IfElse"));

            return new IfElseQuery(ifelsex, linktarget);
        }
    }
}
