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

// TsPasswordBox.cs - Control for passwords. These can only connected to auth, will not create 
// TsVariables because of security issues

using System;
using System.Security;
using System.Windows;
using System.Xml.Linq;
using TsGui.Queries;
using TsGui.Authentication;
using TsGui.Diagnostics;

namespace TsGui.View.GuiOptions
{
    public class TsPasswordBox: GuiOptionBase, IGuiOption, IPassword
    {
        private string _authid;
        private TsPasswordBoxUI _passwordboxui;
        private int _maxlength;

        //Properties
        #region
        public string AuthID { get { return this._authid; } }
        public SecureString SecurePassword { get { return this._passwordboxui.PasswordBox.SecurePassword; } }
        public string Password { get { return this._passwordboxui.PasswordBox.Password; } }
        public override string CurrentValue { get { return null; } }
        public int MaxLength
        {
            get { return this._maxlength; }
            set { this._maxlength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public override TsVariable Variable
        {
            get { return null; }
        }
        #endregion

        //Constructor
        public TsPasswordBox(XElement InputXml, TsColumn Parent): base (Parent)
        {
            this.Init();
            this.LoadXml(InputXml);
        }

        private void Init()
        {
            this._setvaluequerylist = null;

            this._passwordboxui = new TsPasswordBoxUI();
            this.Control = this._passwordboxui;
            this.Label = new TsLabelUI();

            this.UserControl.DataContext = this;
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.ControlFormatting.Padding = new Thickness(3, 2, 3, 2);
            this.LabelText = "Password:";
        }

        public new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            this.MaxLength = XmlHandler.GetIntFromXAttribute(inputxml, "MaxLength", this.MaxLength);
            XAttribute x = inputxml.Attribute("AuthID");
            if (x != null)
            {
                this._authid = x.Value;
                Director.Instance.AuthLibrary.AddPasswordSource(this);
            }  
            else { throw new TsGuiKnownException("Missing AuthID in config:", inputxml.ToString()); }      
        }
    }
}
