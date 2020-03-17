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

// TsCheckBox.cs - combobox control for user input

using System;
using System.Xml.Linq;
using System.Windows;
using System.Collections.Generic;

using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;
using System.Windows.Media;
using System.Windows.Controls;

namespace TsGui.View.GuiOptions
{
    public class TsCheckBox : GuiOptionBase, IGuiOption, IToggleControl, ILinkTarget
    {
        public event ToggleEvent ToggleEvent;

        private TsCheckBoxUI _checkboxui;
        private bool _ischecked;
        private string _valTrue = "TRUE";
        private string _valFalse = "FALSE";

        public bool IsChecked
        {
            get { return this._ischecked; }
            set
            {
                this._ischecked = value;
                this.OnPropertyChanged(this, "IsChecked");
                this.NotifyUpdate();
                this.ToggleEvent?.Invoke();
            }
        }
        //public override string CurrentValue { get { return this.ControlText; } }
        public override string CurrentValue
        {
            get
            {
                if (this.IsChecked == true) { return this._valTrue; }
                else { return this._valFalse; }
            }
        }
        public override TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.CurrentValue); }
            }
        }


        //Constructor
        public TsCheckBox(XElement InputXml, TsColumn Parent) : base(Parent)
        {
            this.UserControl.DataContext = this;
            TsCheckBoxUI cbui = new TsCheckBoxUI();
            this.Control = cbui;
            this._checkboxui = cbui;
            this.InteractiveControl = cbui.CheckBox;
            this.Label = new TsLabelUI();
            this.SetDefaults();
            this._setvaluequerylist = new QueryPriorityList(this);          
            this.LoadXml(InputXml);
            this.UserControl.IsEnabledChanged += this.OnGroupStateChanged;
            this.UserControl.IsVisibleChanged += this.OnGroupStateChanged;
        }

        public new void LoadXml(XElement InputXml)
        {
            XElement x;
            IEnumerable<XElement> xlist;
            this.LoadLegacyXml(InputXml);

            //load the xml for the base class stuff
            base.LoadXml(InputXml);

            this._valTrue = XmlHandler.GetStringFromXElement(InputXml, "TrueValue", this._valTrue);
            this._valFalse = XmlHandler.GetStringFromXElement(InputXml, "FalseValue", this._valFalse);

            x = InputXml.Element("Checked");
            if (x != null)
            { this.IsChecked = true; }

            xlist = InputXml.Elements("Toggle");
            if (xlist != null)
            {
                Director.Instance.AddToggleControl(this);

                foreach (XElement subx in xlist)
                {
                    Toggle t = new Toggle(this, subx); 
                }  
            }
        }

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        {
            this.ToggleEvent?.Invoke();
        }

        public void RefreshValue()
        {
            string newvalue = this._setvaluequerylist.GetResultWrangler()?.GetString();
            if (newvalue != this.CurrentValue)
            {
                if (newvalue == this._valTrue) { this.IsChecked = true; }
                else if (newvalue == this._valFalse) { this.IsChecked = false; }
            }
        }

        public void RefreshAll()
        {
            this.RefreshValue();
        }

        private void OnGroupStateChanged(object o, RoutedEventArgs e)
        {
            this.ToggleEvent?.Invoke();
        }

        private void OnGroupStateChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.ToggleEvent?.Invoke();
        }

        private void SetDefaults()
        {
            if (Director.Instance.UseTouchDefaults == true)
            {
                this.ControlFormatting.Margin = new Thickness(5, 5, 5, 5);
                this._checkboxui.CbBorder.TouchDown += this.OnBorderTouched;
                this._checkboxui.CbBorder.MouseLeftButtonDown += this.OnBorderTouched;
                this._checkboxui.CbBorder.BorderThickness = new Thickness(1);
                this._checkboxui.CbBorder.BorderBrush = Brushes.LightGray;
                this._checkboxui.CbBorder.Background = Brushes.Transparent;
            }
            else
            {
                this.ControlFormatting.Margin = new Thickness(1, 1, 1, 1);
            }
            this.ControlFormatting.Padding = new Thickness(0, 0, 0, 0);
            this.ControlFormatting.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.ControlFormatting.VerticalContentAlignment = VerticalAlignment.Center;
        }

        private void LoadLegacyXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("HAlign");
            if (x != null)
            {
                string s = x.Value.ToUpper();
                switch (s)
                {
                    case "RIGHT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "LEFT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "CENTER":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "STRETCH":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnBorderTouched(object sender, RoutedEventArgs e)
        {
            this.IsChecked = !this._ischecked;
        }
    }
}
