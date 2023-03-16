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

// Conditional.cs - IF -> then processing

using MessageCrap;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Validation.StringMatching;

namespace TsGui.Queries
{
    public class Conditional
    {
        private QueryPriorityList _sourcequerylist;
        private QueryPriorityList _resultquerylist;
        private RuleSet _ruleset;

        public Conditional(XElement inputxml, ILinkTarget targetoption)
        {
            this._ruleset = new RuleSet(targetoption);
            this._sourcequerylist = new QueryPriorityList(targetoption);
            this._resultquerylist = new QueryPriorityList(targetoption);
            this.LoadXml(inputxml);
        }

        
        public void LoadXml(XElement InputXml)
        {
            XElement x;

            #region Shorthand config options - eval these first
            //<IF SourceID="LinkSource1" Equals="SomeTestValue" Result="ThisOne" />
            //<IF SourceID="LinkSource2" Equals="SomeOtherValue" Result="ThatOne" />

            XElement xresSet = new XElement("Resultset");
            foreach (XAttribute attr in InputXml.Attributes())
            {
                switch (attr.Name.ToString().ToLower())
                {
                    case "startswith":
                    case "endswith":
                    case "contains":
                    case "characters":
                    case "regex":
                    case "equals":
                    case "greaterthan":
                    case "greaterthanorequalto":
                    case "lessthan":
                    case "lessthanorequalto":
                    case "isnumeric":
                        x = new XElement("Rule",
                                new XAttribute("Type", attr.Name),
                                attr.Value
                            );
                        xresSet.Add(x);
                        break;
                    case "sourcewmi":
                        x = new XElement("Source",
                                new XElement("Query",
                                    new XAttribute("Type", "Wmi"),
                                    new XElement("Wql", attr.Value)
                                )
                            );
                        this._sourcequerylist.LoadXml(x);
                        break;
                    case "sourceid":
                        x = new XElement("Source",
                                new XElement("Query",
                                    new XAttribute("Type", "LinkTo"),
                                    attr.Value
                                )
                            );
                        this._sourcequerylist.LoadXml(x);
                        break;
                    case "andor":
                        xresSet.Add(new XAttribute("Type",attr.Value));
                        break;
                    case "result":
                        x = new XElement("Result",
                            new XElement("Value", attr.Value)
                        );
                        this._resultquerylist.LoadXml(x);
                        break;
                    default:
                        break;
                }
            }
            if (xresSet.HasElements)
            {
                this._ruleset.LoadXml(xresSet);
            }
            #endregion

            #region Full config options
            #region
            //<IF>
            //  <Source>
            //      <Query/>
            //  </Source>
            //  <Ruleset>
            //      <Rule Type="StartsWith">test</Rule>
            //  </Ruleset>
            //  <Result>
            //      <Query/>
            //  </Result>
            //</IF>
            #endregion
            x = InputXml.Element("Source");
            if (x != null) { this._sourcequerylist.LoadXml(x); }

            x = InputXml.Element("Ruleset");
            if (x != null) { this._ruleset.LoadXml(x); }

            x = InputXml.Element("Result");
            if (x != null) { this._resultquerylist.LoadXml(x); }
            #endregion
        }

        public async Task<ResultWrangler> GetResultWrangler(Message message)
        {
            return await this.ProcessQueryAsync(message);
        }

        public async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            string sourcevalue = (await this._sourcequerylist.GetResultWrangler(message))?.GetString();

            if (this._ruleset.DoesMatch(sourcevalue) == true)
            { return await this._resultquerylist.GetResultWrangler(message); }
            else { return null; }
        }
    }
}
