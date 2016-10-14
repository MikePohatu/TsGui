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

// GuiOptionBase.cs - base parts for all GuiOptions


using System.ComponentModel;
using System.Xml.Linq;

namespace TsGui.View.GuiOptions
{
    public abstract class GuiOptionBase : INotifyPropertyChanged
    {
        protected MainController _controller;

        protected string _labeltext;
        protected string _helptext;

        //standard stuff
        public string LabelText
        {
            get { return this._labeltext; }
            set { this._labeltext = value; this.OnPropertyChanged(this, "LabelText"); }
        }
        public string HelpText
        {
            get { return this._helptext; }
            set { this._helptext = value; this.OnPropertyChanged(this, "HelpText"); }
        }
        public Formatting LabelFormatting { get; set; }
        public Formatting ControlFormatting { get; set; }
        public Formatting GridFormatting { get; set; }
        public string VariableName { get; set; }

        public GuiOptionBase()
        {
            this.LabelFormatting = new Formatting();
            this.ControlFormatting = new Formatting();
            this.GridFormatting = new Formatting();
        }

        protected void LoadBaseXml(XElement InputXml)
        {
            XElement x;
            
            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", null);
            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", string.Empty);
            this.HelpText = XmlHandler.GetStringFromXElement(InputXml, "HelpText", null);

            x = InputXml.Element("Formatting");
            if (x != null)
            {
                x = x.Element("Label");
                if (x != null)
                { this.LabelFormatting.LoadXml(x); }

                x = x.Element("Control");
                if (x != null)
                { this.ControlFormatting.LoadXml(x); }

                x = x.Element("Grid");
                if (x != null)
                { this.GridFormatting.LoadXml(x); }
            }
        }

        //Event handling
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        public void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
        #endregion

    }
}
