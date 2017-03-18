//    Copyright (C) 2017 Mike Pohatu

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

// Conditional.cs - IF rule for option linking

using System.Xml.Linq;

namespace TsGui.Linking
{
    public class Conditional
    {
        public string SourceID { get; set; }
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }

        public void LoadXml(XElement InputXml)
        {
            //<IF ID="AppParent" Value="TRUE">TRUE</IF>
            this.SourceID = XmlHandler.GetStringFromXAttribute(InputXml, "ID", this.SourceID);
            this.SourceValue = XmlHandler.GetStringFromXAttribute(InputXml, "Value", this.SourceID);
        }
    }
}
