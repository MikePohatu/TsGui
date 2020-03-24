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
