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

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

using TsGui.Grouping;

namespace TsGui.View.GuiOptions
{
    public class TsCheckBox : GuiOptionBase, IGuiOption_2, IToggleControl
    {
        public event ToggleEvent ToggleEvent;

        private bool _value;
        private List<TsDropDownListItem> _options = new List<TsDropDownListItem>();
        private string _valTrue = "TRUE";
        private string _valFalse = "FALSE";

        //Custom stuff for control
        public List<TsDropDownListItem> Options { get { return this._options; } }
        public bool IsChecked
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.OnPropertyChanged(this, "IsChecked");
                this.ToggleEvent?.Invoke();
            }
        }
        public string CurrentValue
        {
            get
            {
                if (this.IsChecked == true) { return this._valTrue; }
                else { return this._valFalse; }
            }
        }
        public TsVariable Variable
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
        public TsCheckBox(XElement InputXml, TsColumn Parent, MainController MainController) : base(Parent,MainController)
        {
            this.UserControl.DataContext = this;           
            this.Control = new TsCheckBoxUI();
            this.Label = new TsLabelUI();
            this.LoadXml(InputXml);
        }


        //Methods
        public new void LoadXml(XElement InputXml)
        {
            #region
            XElement x;

            //load the xml for the base class stuff
            base.LoadXml(InputXml);

            this._valTrue = XmlHandler.GetStringFromXElement(InputXml, "TrueValue", this._valTrue);
            this._valFalse = XmlHandler.GetStringFromXElement(InputXml, "FalseValue", this._valFalse);

            x = InputXml.Element("Checked");
            if (x != null)
            { this.IsChecked = true; }

            x = InputXml.Element("Toggle");
            if (x != null)
            {
                Toggle t = new Toggle(this, this._controller, x);
                this._controller.AddToggleControl(this);
            }
            #endregion
        }

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        {
            this.ToggleEvent?.Invoke();
        }

        private void OnChanged(object o, RoutedEventArgs e)
        {
            
        }
    }
}
