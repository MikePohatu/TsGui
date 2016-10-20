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

// TsColumn.cs - class for columns in the gui window

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Data;
using System.ComponentModel;

using TsGui.Grouping;
using TsGui.View.Layout;
using TsGui.View.GuiOptions;

namespace TsGui
{
    public class TsColumn : GroupableBase, IGroupParent, INotifyPropertyChanged
    {
        private bool _purgeInactive;
        private List<IGuiOption_2> _options = new List<IGuiOption_2>();
        private StackPanel _columnpanel;
        private bool _gridlines;
        private double _labelwidth = Double.NaN;
        private double _controlwidth = Double.NaN;
        private double _fullwidth = Double.NaN;
        private TsRow _parent;

        //properties
        #region
        public TsRow Parent { get { return this._parent; } }
        public bool PurgeInactive
        {
            get { return this._purgeInactive; }
            set
            {
                this._purgeInactive = value;
                foreach (IGuiOption option in this._options)
                { option.PurgeInactive = value; }
            }
        }
        public double LabelWidth
        {
            get { return this._labelwidth; }
            set
            {
                this._labelwidth = value;
                this.OnPropertyChanged(this, "LabelWidth");
            }
        }
        public double ControlWidth
        {
            get { return this._controlwidth; }
            set
            {
                this._controlwidth = value;
                this.OnPropertyChanged(this, "ControlWidth");
            }
        }
        public double Width
        {
            get { return this._fullwidth; }
            set
            {
                this._fullwidth = value;
                this.OnPropertyChanged(this, "Width");
            }
        }
        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this._gridlines = value;
                this.OnPropertyChanged(this, "ShowGridLines");
            }
        }
        public int Index { get; set; }
        public List<IGuiOption_2> Options { get { return this._options; } }
        public Panel Panel { get { return this._columnpanel; } }
        
        #endregion

        //constructor
        #region
        public TsColumn (XElement SourceXml,int PageIndex, TsRow Parent, MainController RootController):base (RootController)
        {

            this._controller = RootController;
            this._parent = Parent;
            this.Index = PageIndex;
            //this._columngrid = new Grid();
            this._columnpanel = new StackPanel();
            this._columnpanel.Name = "_columnpanel";
            this._columnpanel.DataContext = this;
            this._columnpanel.SetBinding(StackPanel.WidthProperty, new Binding("Width"));
            //this._columnpanel.HorizontalAlignment = HorizontalAlignment.Left;

            this._purgeInactive = false;
            this.DisabledParentCount = 0;
            this.HiddenParentCount = 0;

            this.LoadXml(SourceXml);
            //this.Build();
        }
        #endregion


        private void LoadXml(XElement InputXml)
        {
            IEnumerable<XElement> xlist;
            IGuiOption_2 newOption;
            bool purgeset = false;

            this.LoadGroupingXml(InputXml);

            this.PurgeInactive = XmlHandler.GetBoolFromXAttribute(InputXml, "PurgeInactive", this.PurgeInactive);
            this.LabelWidth = XmlHandler.GetDoubleFromXElement(InputXml, "LabelWidth", this.LabelWidth);
            this.ControlWidth = XmlHandler.GetDoubleFromXElement(InputXml, "ControlWidth", this.ControlWidth);
            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", this.Width);
            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.Parent.ShowGridLines);

            //now read in the options and add to a dictionary for later use
            //do this last so the event subscriptions don't get setup too early (no toggles fired 
            //until everything is loaded.
            xlist = InputXml.Elements("GuiOption");
            if (xlist != null)
            {
                foreach (XElement xOption in xlist)
                {
                    newOption = GuiFactory.CreateGuiOption_2(xOption,this,this._controller);
                    if (newOption ==null) { continue; }
                    if (purgeset == true) { newOption.PurgeInactive = this.PurgeInactive; }
                    this._options.Add(newOption);
                    this._controller.AddOptionToLibary(newOption);
                    this._columnpanel.Children.Add(newOption.Control);

                    //register for events
                    this.ParentEnable += newOption.OnParentEnable;
                    this.ParentHide += newOption.OnParentHide;
                }
            }

        }
    }
}
