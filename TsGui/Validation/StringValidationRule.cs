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

// ValidationRule.cs - holds the rule evaluation and type

using System.Xml.Linq;

namespace TsGui.Validation
{
    public class StringValidationRule
    {
        public StringValidationRuleType Type { get; set; }
        public string Content { get; set; }
        public bool IsCaseSensitive { get; set; }
        public string Message { get; set; }

        public StringValidationRule() { }
        public StringValidationRule(XElement InputXml) { this.LoadXml(InputXml); }
        public StringValidationRule(StringValidationRuleType Type, string Content)
        {
            this.Type = Type;
            this.Content = Content;
            this.GenerateMessage();
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }

            this.SetType(InputXml.Attribute("Type"));
            this.Content = InputXml.Value;
            this.IsCaseSensitive = XmlHandler.GetBoolFromXAttribute(InputXml, "CaseSensitive", false);
            this.GenerateMessage();
        }

        private void GenerateMessage()
        {
            string s;
            if (this.Type != StringValidationRuleType.IsNumeric)
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
            if (Type == null) { this.Type = StringValidationRuleType.StartsWith; }
            else
            {
                switch (Type.Value)
                {
                    case "StartsWith":
                        this.Type = StringValidationRuleType.StartsWith;
                        break;
                    case "EndsWith":
                        this.Type = StringValidationRuleType.EndsWith;
                        break;
                    case "Contains":
                        this.Type = StringValidationRuleType.Contains;
                        break;
                    case "Characters":
                        this.Type = StringValidationRuleType.Characters;
                        break;
                    case "RegEx":
                        this.Type = StringValidationRuleType.RegEx;
                        break;
                    case "Equals":
                        this.Type = StringValidationRuleType.Equals;
                        break;
                    case "GreaterThan":
                        this.Type = StringValidationRuleType.GreaterThan;
                        break;
                    case "LessThan":
                        this.Type = StringValidationRuleType.LessThan;
                        break;
                    case "GreaterThanOrEqualTo":
                        this.Type = StringValidationRuleType.GreaterThanOrEqualTo;
                        break;
                    case "LessThanOrEqualTo":
                        this.Type = StringValidationRuleType.LessThanOrEqualTo;
                        break;
                    case "IsNumeric":
                        this.Type = StringValidationRuleType.IsNumeric;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
