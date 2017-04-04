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

// LinkingRule.cs - holds the linking rule evaluation and type

using System.Xml.Linq;

namespace TsGui.Linking
{
    public class LinkingRule
    {
        public LinkingRuleType Type { get; set; }
        public string Content { get; set; }
        public bool IsCaseSensitive { get; set; }
        public string Message { get; set; }

        public LinkingRule() { }
        public LinkingRule(XElement InputXml) { this.LoadXml(InputXml); }
        public LinkingRule(LinkingRuleType Type, string Content)
        {
            this.Type = Type;
            this.Content = Content;
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }

            this.SetType(InputXml.Attribute("Type"));
            this.Content = InputXml.Value;
            this.IsCaseSensitive = XmlHandler.GetBoolFromXAttribute(InputXml, "CaseSensitive", false);
        }

        private void SetType(XAttribute Type)
        {
            if (Type == null) { this.Type = LinkingRuleType.StartsWith; }
            else
            {
                switch (Type.Value)
                {
                    case "StartsWith":
                        this.Type = LinkingRuleType.StartsWith;
                        break;
                    case "EndsWith":
                        this.Type = LinkingRuleType.EndsWith;
                        break;
                    case "Contains":
                        this.Type = LinkingRuleType.Contains;
                        break;
                    case "Characters":
                        this.Type = LinkingRuleType.Characters;
                        break;
                    case "RegEx":
                        this.Type = LinkingRuleType.RegEx;
                        break;
                    case "Equals":
                        this.Type = LinkingRuleType.Equals;
                        break;
                    case "GreaterThan":
                        this.Type = LinkingRuleType.GreaterThan;
                        break;
                    case "LessThan":
                        this.Type = LinkingRuleType.LessThan;
                        break;
                    case "GreaterThanOrEqualTo":
                        this.Type = LinkingRuleType.GreaterThanOrEqualTo;
                        break;
                    case "LessThanOrEqualTo":
                        this.Type = LinkingRuleType.LessThanOrEqualTo;
                        break;
                    case "IsNumeric":
                        this.Type = LinkingRuleType.IsNumeric;
                        break;
                    case "IsActive":
                        this.Type = LinkingRuleType.IsActive;
                        break;
                    case "IsInactive":
                        this.Type = LinkingRuleType.IsInactive;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
