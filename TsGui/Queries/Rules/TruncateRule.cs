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

// TruncateRule.cs - used to remove data from a string result

using System;
using System.Xml.Linq;

namespace TsGui.Queries.Rules
{
    public enum TruncateRuleType { RemoveFromStart, RemoveFromEnd, KeepFromStart, KeepFromEnd};

    public class TruncateRule: IQueryRule
    {
        private TruncateRuleType _type;
        private int _number;

        public TruncateRule(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        private void LoadXml(XElement InputXml)
        {
            XAttribute xa = InputXml.Attribute("Type");
            if (xa != null) { this.SetType(xa.Value); }

            this._number = Convert.ToInt32(InputXml.Value);
        }

        private void SetType(string Type)
        {
            switch (Type.ToUpper())
            {
                case "REMOVEFROMSTART":
                    this._type = TruncateRuleType.RemoveFromStart;
                    break;
                case "REMOVEFROMEND":
                    this._type = TruncateRuleType.RemoveFromEnd;
                    break;
                case "KEEPFROMSTART":
                    this._type = TruncateRuleType.KeepFromStart;
                    break;
                case "KEEPFROMEND":
                    this._type = TruncateRuleType.KeepFromEnd;
                    break;
                default:
                    break;
            }
        }

        public string Process(string Input)
        {
            

            switch (this._type)
            {
                case TruncateRuleType.RemoveFromStart:
                    return this.RemoveFromStart(Input);
                case TruncateRuleType.RemoveFromEnd:
                    return this.RemoveFromEnd(Input);
                case TruncateRuleType.KeepFromStart:
                    return this.KeepFromStart(Input);
                case TruncateRuleType.KeepFromEnd:
                    return this.KeepFromEnd(Input);
                default:
                    return Input;
            }
        }

        private string KeepFromEnd(string Input)
        {
            if (Input.Length <= this._number) { return Input; }
            return Input.Substring(Input.Length - this._number);
        }

        private string KeepFromStart(string Input)
        {
            if (Input.Length <= this._number) { return Input; }
            return Input.Substring(0,this._number);
        }

        private string RemoveFromEnd(string Input)
        {
            if (Input.Length <= this._number) { return string.Empty; }
            return Input.Substring(0, Input.Length - this._number);
        }

        private string RemoveFromStart(string Input)
        {
            if (Input.Length <= this._number) { return string.Empty; }
            return Input.Substring(this._number);
        }
    }
}
