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

namespace TsGui.View.Layout
{
    public abstract class ParentLayoutElement: BaseLayoutElement
    {
        public ParentLayoutElement(ParentLayoutElement Parent) : base(Parent) { }
        public ParentLayoutElement() : base() { }

        /// <summary>
        /// Single xml parameter LoadXml is for normal loading of config
        /// </summary>
        /// <param name="InputXml"></param>
        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
        }


        /// <summary>
        /// Two parameter LoadXml is for loading additional config into an existing element i.e. from a UIContainer
        /// </summary>
        /// <param name="InputXml"></param>
        /// <param name="parent"></param>
        public abstract void LoadXml(XElement InputXml, ParentLayoutElement parent);
    }
}
