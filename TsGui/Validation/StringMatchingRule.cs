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

// StringMatchingRule.cs - holds the rule evaluation and type

using System.Xml.Linq;

namespace TsGui.Validation
{
    public class StringMatchingRule
    {
        public StringMatchingRuleType Type { get; set; }
        public string Content { get; set; }
        public bool IsCaseSensitive { get; set; }
        public string Message { get; set; }

        public StringMatchingRule() { }
        public StringMatchingRule(XElement InputXml) { this.LoadXml(InputXml); }
        public StringMatchingRule(StringMatchingRuleType Type, string Content)
        {
            this.Type = Type;
            this.Content = Content;
            this.GenerateMessage();
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }

            this.Content = InputXml.Value;
            this.SetType(InputXml.Attribute("Type"));
            this.IsCaseSensitive = XmlHandler.GetBoolFromXAttribute(InputXml, "CaseSensitive", false);
            this.GenerateMessage();
        }

        private void GenerateMessage()
        {
            string s;
            if (this.Type != StringMatchingRuleType.IsNumeric)
            {
                s = this.Type.ToString() + ": \"" + this.Content + "\"";
                if (this.IsCaseSensitive == true) { s = s + " (case sensitive)"; }
            }
            else
            { s = this.Type.ToString(); }
            
            this.Message = s;
        }

        private void SetType(XAttribute Type)
        {
            if (Type == null) { this.Type = StringMatchingRuleType.StartsWith; }
            else
            {
                switch (Type.Value)
                {
                    case "StartsWith":
                        this.Type = StringMatchingRuleType.StartsWith;
                        break;
                    case "EndsWith":
                        this.Type = StringMatchingRuleType.EndsWith;
                        break;
                    case "Contains":
                        this.Type = StringMatchingRuleType.Contains;
                        break;
                    case "Characters":
                        this.Type = StringMatchingRuleType.Characters;
                        break;
                    case "RegEx":
                        this.Type = StringMatchingRuleType.RegEx;
                        break;
                    case "Equals":
                        this.Type = StringMatchingRuleType.Equals;
                        break;
                    case "GreaterThan":
                        this.Type = StringMatchingRuleType.GreaterThan;
                        break;
                    case "LessThan":
                        this.Type = StringMatchingRuleType.LessThan;
                        break;
                    case "GreaterThanOrEqualTo":
                        this.Type = StringMatchingRuleType.GreaterThanOrEqualTo;
                        break;
                    case "LessThanOrEqualTo":
                        this.Type = StringMatchingRuleType.LessThanOrEqualTo;
                        break;
                    case "IsNumeric":
                        this.Type = StringMatchingRuleType.IsNumeric;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
