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
        public TsPasswordBox(XElement InputXml, TsColumn Parent, IDirector director): base (Parent, director)
        {
            this.Init(director);
            this.LoadXml(InputXml);
        }

        protected TsPasswordBox(TsColumn Parent, IDirector MainController) : base(Parent, MainController)
        {
            this.Init(MainController);
        }

        private void Init(IDirector MainController)
        {
            this._director = MainController;
            this._querylist = new QueryList(this._director);

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

        private new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);
            this.MaxLength = XmlHandler.GetIntFromXAttribute(inputxml, "MaxLength", this.MaxLength);
            XAttribute x = inputxml.Attribute("AuthID");
            if (x != null)
            {
                this._authid = x.Value;
                this._director.AuthLibrary.AddPasswordSource(this._authid, this);
            }  
            else { throw new TsGuiKnownException("Missing AuthID in config:", inputxml.ToString()); }      
        }
    }
}
