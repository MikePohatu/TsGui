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
        public static IStringMatchingRule GetRootRuleObject(XElement InputXml, ILinkTarget linktarget)
        {
            IStringMatchingRule newrule = GetRuleObject(InputXml, linktarget);
            RuleSet testset = newrule as RuleSet;
            if (testset != null) { testset.IsChild = false; }
            return newrule;
        }

        public static IStringMatchingRule GetRuleObject(XElement InputXml, ILinkTarget linktarget)
        {
            string xname = InputXml?.Name.ToString();
            string type = XmlHandler.GetStringFromXAttribute(InputXml, "Type", string.Empty);

            if (xname == "Ruleset")
            { return new RuleSet(InputXml); }

            switch (type.ToLower())
            {
                case "startswith":
                    return new StartsWith(InputXml, linktarget);
                case "endswith":
                    return new EndsWith(InputXml, linktarget);
                case "contains":
                    return new Contains(InputXml, linktarget);
                case "characters":
                    return new Characters(InputXml, linktarget);
                case "regex":
                    return new RegEx(InputXml, linktarget);
                case "equals":
                    return new Equals(InputXml, linktarget);
                case "greaterthan":
                    return new GreaterThan(InputXml, linktarget);
                case "greaterthanorequalto":
                    return new GreaterThanOrEqualTo(InputXml, linktarget);
                case "lessthan":
                    return new LessThan(InputXml, linktarget);
                case "lessthanorequalto":
                    return new LessThanOrEqualTo(InputXml, linktarget);
                case "isnumeric":
                    return new IsNumeric(InputXml, linktarget);
                default:
                    return new StartsWith(InputXml, linktarget);
            }
        }
    }
}
