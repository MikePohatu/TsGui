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

//  BlindOption.cs - used to create TsVariables but aren't shown in the gui. 

using System.Xml.Linq;
using TsGui.Grouping;

namespace TsGui.Blind
{
    public class BlindOption: GroupableBlindBase,IOption
    {
        private string _inactivevalue = "TSGUI_INACTIVE";
        private string _value;

        public string VariableName { get; set; }
        public string InactiveValue
        {
            get { return this._inactivevalue; }
            set { this._inactivevalue = value; }
        }
        public TsVariable Variable { get { return new TsVariable(this.VariableName, this._value); } }

        //constructors
        public BlindOption(MainController MainController) : base(MainController) { }
        public BlindOption(NoUIContainer Parent, MainController MainController) : base(Parent, MainController)
        {
            Parent.GroupingStateChange += this.OnParentGoupingStateChange;
            this._parent = Parent;          
        }
        public BlindOption(MainController MainController, XElement InputXml) : base(MainController)
        {
            this.LoadXml(InputXml);
        }
        public BlindOption(NoUIContainer Parent, MainController MainController, XElement InputXml) : base(MainController)
        {
            Parent.GroupingStateChange += this.OnParentGoupingStateChange;
            this._parent = Parent;
            this.LoadXml(InputXml);
        }

        //public methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", this.VariableName);
            this._value = XmlHandler.GetStringFromXElement(InputXml, "Value", this._value);

            this.VariableName = XmlHandler.GetStringFromXAttribute(InputXml, "Variable", this.VariableName);
            this._value = XmlHandler.GetStringFromXAttribute(InputXml, "Value", this._value);
        }
    }
}
