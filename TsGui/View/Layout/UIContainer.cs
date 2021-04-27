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

// UIContainer.cs - container for GuiOptions e.g. to wrap formatting

using System.Xml.Linq;

namespace TsGui.View.Layout
{
    public class UIContainer : ParentLayoutElement
    {
        //constructors
        public UIContainer(ParentLayoutElement Parent, XElement InputXml) : base(Parent)
        {
            this.LoadXml(InputXml);
        }

        //methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.Parent.LoadXml(InputXml, this);
        }

        public override void LoadXml(XElement InputXml, ParentLayoutElement parent)
        {
            this.Parent.LoadXml(InputXml, parent);
        }
    }
}
