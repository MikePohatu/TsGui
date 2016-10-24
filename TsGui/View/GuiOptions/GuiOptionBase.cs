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
using TsGui.Grouping;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions
{
    public abstract class GuiOptionBase : BaseLayoutElement, INotifyPropertyChanged
    {
        protected string _labeltext = string.Empty;
        protected string _helptext = string.Empty;
        protected bool _purgeinactive = false;
        protected string _inactivevalue = "TSGUI_INACTIVE";

        //standard stuff
        public TsColumn Parent { get; set; }
        public string InactiveValue
        {
            get { return this._inactivevalue; }
            set { this._inactivevalue = value; }
        }
        public string VariableName { get; set; }
        public bool PurgeInactive
        {
            get { return this._purgeinactive; }
            set { this._purgeinactive = value; }
        }
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
        
        
        public GuiOptionBase(TsColumn Parent, MainController MainController):base(MainController)
        {
            this.Parent = Parent;
            this.LabelFormatting = new Formatting();
            this.ControlFormatting = new Formatting();
            this.GridFormatting = new Formatting();
            this.SetDefaults();
        }

        protected void SetDefaults()
        {
            this.GridFormatting.Width = this.Parent.Width;
            this.ControlFormatting.Width = this.Parent.ControlWidth;
            this.LabelFormatting.Width = this.Parent.LabelWidth;
            this.ShowGridLines = this.Parent.ShowGridLines;
        }

        protected void LoadXml(XElement InputXml)
        {
            XElement x;
            XElement subx;

            this.LoadGroupingXml(InputXml);

            this.PurgeInactive = XmlHandler.GetBoolFromXAttribute(InputXml, "PurgeInactive", this.PurgeInactive);
            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", this.VariableName);
            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", this.LabelText);
            this.HelpText = XmlHandler.GetStringFromXElement(InputXml, "HelpText", this.HelpText);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.Parent.ShowGridLines);
            this.InactiveValue = XmlHandler.GetStringFromXElement(InputXml, "InactiveValue", this.InactiveValue);

            x = InputXml.Element("Formatting");
            if (x != null)
            {
                subx = x.Element("Label");
                if (subx != null)
                { this.LabelFormatting.LoadXml(subx); }

                subx = x.Element("Control");
                if (subx != null)
                { this.ControlFormatting.LoadXml(subx); }

                subx = x.Element("Grid");
                if (subx != null)
                { this.GridFormatting.LoadXml(subx); }
            }
        }
    }
}
