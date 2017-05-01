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

using TsGui.Authentication;
using System.Xml.Linq;

namespace TsGui.View.GuiOptions
{
    public class TsUsername : TsFreeText,IUsername
    {
        private string _authid;
        public string AuthID { get { return this._authid; } }
        public string Username { get { return this.CurrentValue; } }

        public TsUsername(XElement InputXml, TsColumn parent, IDirector director) : base(parent, director)
        {
            
            this.RefreshValue();
        }

        private new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            this._authid = XmlHandler.GetStringFromXAttribute(inputxml, "AuthID", this._authid);
        }
    }
}