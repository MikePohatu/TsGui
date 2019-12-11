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
using TsGui.Linking;

namespace TsGui.Validation.StringMatching
{
    public static class MatchingRuleFactory
    {
        public static IStringMatchingRule GetRootRuleObject(XElement InputXml, ILinkTarget owner)
        {
            IStringMatchingRule newrule = GetRuleObject(InputXml, owner);
            RuleSet testset = newrule as RuleSet;
            if (testset != null) { testset.IsChild = false; }
            return newrule;
        }

        public static IStringMatchingRule GetRuleObject(XElement InputXml, ILinkTarget owner)
        {
            string xname = InputXml?.Name.ToString();
            string type = XmlHandler.GetStringFromXAttribute(InputXml, "Type", string.Empty);

            if (xname == "Ruleset")
            { return new RuleSet(InputXml); }

            switch (type.ToLower())
            {
                case "startswith":
                    return new StartsWith(InputXml, owner);
                case "endswith":
                    return new EndsWith(InputXml, owner);
                case "contains":
                    return new Contains(InputXml, owner);
                case "characters":
                    return new Characters(InputXml, owner);
                case "regex":
                    return new RegEx(InputXml, owner);
                case "equals":
                    return new Equals(InputXml, owner);
                case "greaterthan":
                    return new GreaterThan(InputXml, owner);
                case "greaterthanorequalto":
                    return new GreaterThanOrEqualTo(InputXml, owner);
                case "lessthan":
                    return new LessThan(InputXml, owner);
                case "lessthanorequalto":
                    return new LessThanOrEqualTo(InputXml, owner);
                case "isnumeric":
                    return new IsNumeric(InputXml, owner);
                default:
                    return new StartsWith(InputXml, owner);
            }
        }
    }
}
