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

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }
            this.SetType(InputXml.Name);
            this.Content = InputXml.Value;
            this.IsCaseSensitive = XmlHandler.GetBoolFromXAttribute(InputXml, "CaseSensitive", false);
            this.Message = this.Type.ToString() + " \"" + this.Content + "\"";
        }

        private void SetType(XName XName)
        {
            switch (XName.ToString())
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
                default:
                    break;
            }
        }
    }
}
