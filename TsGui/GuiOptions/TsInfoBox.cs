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

// TsInfoBox.cs - Non interactive control for displaying info from WMI. Displays a label as the control.
// Returns the value displayed for use in a variable

using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System;

namespace TsGui
{
    public class TsInfoBox : TsBaseOption, IGuiOption
    {
        new private Label _control;
        private int _maxlength;

        public string DisallowedCharacters { get; set; }
        public bool Bold { get; set; }

        public int MaxLength
        {
            get { return this._maxlength; }
            set
            {
                this._maxlength = value;
                this.OnPropertyChanged(this, "MaxLength");
            }
        }

        public TsInfoBox(XElement SourceXml, MainController RootController) : base()
        {
            this._controller = RootController;

            this._control = new Label();
            base._control = this._control;

            //setup the bindings
            this._control.DataContext = this;
            this._control.SetBinding(Label.IsEnabledProperty, new Binding("IsEnabled"));
            this._control.SetBinding(Label.HeightProperty, new Binding("Height"));
            this._control.SetBinding(Label.ContentProperty, new Binding("Value"));
            this._control.SetBinding(Label.PaddingProperty, new Binding("Padding"));
            this._control.SetBinding(Label.MarginProperty, new Binding("Margin"));

            this._control.VerticalAlignment = VerticalAlignment.Bottom;
            this._visiblepadding = new Thickness(3, 0, 0, 0);
            this.Padding = this._visiblepadding;

            this._visiblemargin = new Thickness(2, 2, 2, 2);
            this.Margin = this._visiblemargin;
            this._visibleHeight = 15;
            this.Height = _visibleHeight;

            this.LoadXml(SourceXml);
            //this.Build();
        }

        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.Value); }
            }
        }

        public void LoadXml(XElement InputXml)
        {
            #region
            XElement x;
            XAttribute attrib;

            //load the xml for the base class stuff
            this.LoadBaseXml(InputXml);

            attrib = InputXml.Attribute("MaxLength");
            if (attrib != null)
            { this.MaxLength = Convert.ToInt32(attrib.Value); }

            x = InputXml.Element("Disallowed");
            if (x != null)
            {
                x = x.Element("Characters");
                if (x != null) { this.DisallowedCharacters = x.Value; }
            }

            x = InputXml.Element("DisplayValue");
            if (x != null)
            {
                this.Value = this._controller.GetValueFromList(x);
                if (this.Value == null) { this.Value = string.Empty; }

                //if required, remove invalid characters and truncate
                if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.Value = ResultValidator.RemoveInvalid(this.Value, this.DisallowedCharacters); }
                if (this._maxlength > 0) { this.Value = ResultValidator.Truncate(this.Value, this._maxlength); }
            }
            #endregion
        }
    }
}
