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

using System.Windows;
using TsGui.Authentication;
using TsGui.Diagnostics;
using System.Xml.Linq;

namespace TsGui.View.GuiOptions
{
    public class TsUsernameBox : TsFreeText,IUsername
    {
        private string _authid;
        public string AuthID { get { return this._authid; } }
        public string Username { get { return this._controltext; } }

        public TsUsernameBox(XElement InputXml, TsColumn parent, IDirector director) : base(parent, director)
        {
            this.SetDefaults();
            this.LoadXml(InputXml);
            this.RefreshValue();
        }

        private void SetDefaults()
        {
            this.LabelText = "Username:";
        }

        private new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            XAttribute x = inputxml.Attribute("AuthID");
            if (x != null)
            {
                this._authid = x.Value;
                this._director.AuthLibrary.AddUsernameSource(this);
            }
            else { throw new TsGuiKnownException("Missing AuthID in config", inputxml.ToString()); }
        }
    }
}