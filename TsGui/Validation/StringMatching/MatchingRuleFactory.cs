//    Copyright (C) 2016 Mike Pohatu

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

namespace TsGui.Validation.StringMatching
{
    public static class MatchingRuleFactory
    {
        public static IStringMatchingRule GetRootRuleObject(XElement InputXml)
        {
            IStringMatchingRule newrule = GetRuleObject(InputXml);
            RuleSet testset = newrule as RuleSet;
            if (testset != null) { testset.IsChild = false; }
            return newrule;
        }

        public static IStringMatchingRule GetRuleObject(XElement InputXml)
        {
            string xname = InputXml?.Name.ToString();
            string type = XmlHandler.GetStringFromXAttribute(InputXml, "Type", string.Empty);

            if (xname == "Ruleset")
            { return new RuleSet(InputXml); }

            switch (type.ToLower())
            {
                case "startswith":
                    return new StartsWith(InputXml);
                case "endswith":
                    return new EndsWith(InputXml);
                case "contains":
                    return new Contains(InputXml);
                case "characters":
                    return new Characters(InputXml);
                case "regex":
                    return new RegEx(InputXml);
                case "equals":
                    return new Equals(InputXml);
                case "greaterthan":
                    return new GreaterThan(InputXml);
                case "greaterthanorequalto":
                    return new GreaterThanOrEqualTo(InputXml);
                case "lessthan":
                    return new LessThan(InputXml);
                case "lessthanorequalto":
                    return new LessThanOrEqualTo(InputXml);
                case "isnumeric":
                    return new IsNumeric(InputXml);
                default:
                    return new StartsWith(InputXml);
            }
        }
    }
}
