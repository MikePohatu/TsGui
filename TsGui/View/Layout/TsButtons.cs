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

// TsButtons.cs - class for controlling the Next, Back, Finish, and Cancel buttons

using System.Windows;
using System.Xml.Linq;

namespace TsGui.View.Layout
{
    public class TsButtons : ViewModelBase
    {
        private string _buttonTextNext;
        private string _buttonTextFinish;
        private string _buttonTextCancel;
        private string _buttonTextBack;
        private Formatting _controlformatting;
        private Visibility _cancelvisibility = Visibility.Visible;
        private bool _cancelenabled = true;

        //Properties
        #region

        public Visibility CancelVisibility
        {
            get { return this._cancelvisibility; }
            set
            {
                this._cancelvisibility = value;
                this.OnPropertyChanged(this, "CancelVisibility");
            }
        }
        public bool CancelEnabled
        {
            get { return this._cancelenabled; }
            set
            {
                this._cancelenabled = value;
                this.OnPropertyChanged(this, "CancelEnabled");
            }
        }
        public Formatting ControlFormatting
        {
            get { return this._controlformatting; }
        }
        public string ButtonTextCancel
        {
            get { return this._buttonTextCancel; }
            set
            {
                this._buttonTextCancel = value;
                this.OnPropertyChanged(this, "ButtonTextCancel");
            }
        }
        public string ButtonTextNext
        {
            get { return this._buttonTextNext; }
            set
            {
                this._buttonTextNext = value;
                this.OnPropertyChanged(this, "ButtonTextNext");
            }
        }
        public string ButtonTextBack
        {
            get { return this._buttonTextBack; }
            set
            {
                this._buttonTextBack = value;
                this.OnPropertyChanged(this, "ButtonTextBack");
            }
        }
        public string ButtonTextFinish
        {
            get { return this._buttonTextFinish; }
            set
            {
                this._buttonTextFinish = value;
                this.OnPropertyChanged(this, "ButtonTextFinish");
            }
        }
        #endregion

        //Constructor
        public TsButtons()
        {
            this.ButtonTextBack = "Back";
            this.ButtonTextCancel = "Cancel";
            this.ButtonTextFinish = "Finish";
            this.ButtonTextNext = "Next";
            this._controlformatting = new Formatting();
            this.SetDefaults();
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml == null) { return; }

            XElement x;

            this.ButtonTextNext = XmlHandler.GetStringFromXElement(InputXml, "Next", this.ButtonTextNext);
            this.ButtonTextBack = XmlHandler.GetStringFromXElement(InputXml, "Back", this.ButtonTextBack);
            this.ButtonTextFinish = XmlHandler.GetStringFromXElement(InputXml, "Finish", this.ButtonTextFinish);
            this.ButtonTextCancel = XmlHandler.GetStringFromXElement(InputXml, "Cancel", this.ButtonTextCancel);

            x = InputXml.Element("Formatting");
            if (x != null) { this._controlformatting.LoadXml(x); }

            x = InputXml.Element("HideCancel");
            if (x != null)
            {
                if (x.Value.ToUpper() == "TRUE") { this.CancelVisibility = Visibility.Collapsed; }
                else if (x.Value.ToUpper() == "DISABLED") { this.CancelEnabled = false; }
            }
        }


        public static void Update(TsPage Page, TsPageUI Layout)
        {
            if (Page.NextActivePage != null)
            {
                Layout.buttonNext.Visibility = Visibility.Visible;
                Layout.buttonNext.IsEnabled = true;
                Layout.buttonFinish.Visibility = Visibility.Hidden;
                Layout.buttonFinish.IsEnabled = false;
            }
            else
            {
                Layout.buttonFinish.Visibility = Visibility.Visible;
                Layout.buttonFinish.IsEnabled = true;
                Layout.buttonNext.Visibility = Visibility.Hidden;
                Layout.buttonNext.IsEnabled = false;
            }

            if (Page.PreviousActivePage != null)
            {
                Layout.buttonPrev.Visibility = Visibility.Visible;
                Layout.buttonPrev.IsEnabled = true;
            }
            else
            {
                Layout.buttonPrev.Visibility = Visibility.Hidden;
                Layout.buttonPrev.IsEnabled = false;
            }
        }

        private void SetDefaults()
        {
            this._controlformatting.VerticalAlignment = VerticalAlignment.Center;
            this._controlformatting.Width = 75;
            this._controlformatting.Height = 30;
        }
    }
}

