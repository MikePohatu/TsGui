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

namespace TsGui.View.GuiOptions
{
    public abstract class GuiOptionBase : GroupableBase, INotifyPropertyChanged
    {
        protected MainController _controller;

        protected string _labeltext;
        protected string _helptext;
        protected bool _showgridlines;

        //standard stuff
        public TsColumn Parent { get; set; }
        public Formatting LabelFormatting { get; set; }
        public Formatting ControlFormatting { get; set; }
        public Formatting GridFormatting { get; set; }
        public string InactiveValue { get; set; }
        public string VariableName { get; set; }
        public bool PurgeInactive { get; set; }
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
        public bool ShowGridLines
        {
            get { return this._showgridlines; }
            set { this._showgridlines = value; this.OnPropertyChanged(this, "ShowGridLines"); }
        }
        
        public GuiOptionBase(TsColumn Parent)
        {
            this.Parent = Parent;
            this.LabelFormatting = new Formatting(this);
            this.ControlFormatting = new Formatting(this);
            this.GridFormatting = new Formatting(this);
            this.SetDefaults();
        }

        protected void LoadBaseXml(XElement InputXml)
        {
            XElement x;
            XElement subx;
            
            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", null);
            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", string.Empty);
            this.HelpText = XmlHandler.GetStringFromXElement(InputXml, "HelpText", null);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.Parent.ShowGridLines);
            this.InactiveValue = XmlHandler.GetStringFromXElement(InputXml, "InactiveValue", string.Empty);

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

        protected void SetDefaults()
        {
            this.ControlFormatting.Width = this.Parent.ControlWidth;
            this.LabelFormatting.Width = this.Parent.LabelWidth;
        }
    }
}
