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
using System.Windows;
using TsGui.Authentication;
using TsGui.Diagnostics;
using System.Xml.Linq;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions
{
    public class TsUsernameBox : TsFreeText,IUsername
    {
        private string _authid;
        public string AuthID { get { return this._authid; } }
        public string Username { get { return this._controltext; } }

        public TsUsernameBox(XElement InputXml, ParentLayoutElement parent) : base(parent)
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
                Director.Instance.AuthLibrary.AddUsernameSource(this);
            }
            else { throw new TsGuiKnownException("Missing AuthID in config", inputxml.ToString()); }
        }
    }
}