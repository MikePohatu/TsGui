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
using System.Windows.Data;
using System.ComponentModel;

using TsGui.View.Layout;
using TsGui.View.GuiOptions;

namespace TsGui
{
    public class TsColumn : IGroupParent, IGroupChild, INotifyPropertyChanged
    {
        private bool _enabled = true;
        private bool _purgeInactive;
        private List<Group> _groups = new List<Group>();
        private bool _hidden = false;
        //private List<IGuiOption> _options = new List<IGuiOption>();
        private List<IGuiOption_2> _options = new List<IGuiOption_2>();
        //private Grid _columngrid;
        private StackPanel _columnpanel;
        private bool _gridlines;
        private GridLength _labelwidth;
        private GridLength _controlwidth;
        private GridLength _fullwidth;
        private MainController _controller;
        //private ColumnDefinition _coldefControls;
        //private ColumnDefinition _coldefLabels;
        private TsRow _parent;

        //properties
        #region
        public TsRow Parent { get { return this._parent; } }
        public List<Group> Groups { get { return this._groups; } }
        public int GroupCount { get { return this._groups.Count; } }
        public int DisabledParentCount { get; set; }
        public int HiddenParentCount { get; set; }
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
        public GridLength LabelWidth
        {
            get { return this._labelwidth; }
            set
            {
                this._labelwidth = value;
                this.OnPropertyChanged(this, "LabelWidth");
            }
        }
        public GridLength ControlWidth
        {
            get { return this._controlwidth; }
            set
            {
                this._controlwidth = value;
                this.OnPropertyChanged(this, "ControlWidth");
            }
        }
        public GridLength Width
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
        public bool IsEnabled
        {
            get { return this._enabled; }
            set
            {
                this._enabled = value;
                this.ParentEnable?.Invoke(value);
                this.OnPropertyChanged(this, "IsEnabled");
            }
        }
        public bool IsHidden
        {
            get { return this._hidden; }
            set
            {
                this._hidden = value;
                this.ParentHide?.Invoke(value);
                this.OnPropertyChanged(this, "IsHidden");
            }
        }
        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false))
                { return true; }
                else { return false; }
            }
        }
        #endregion

        //constructor
        #region
        public TsColumn (XElement SourceXml,int PageIndex, TsRow Parent, MainController RootController)
        {

            this._controller = RootController;
            this._parent = Parent;
            this.Index = PageIndex;
            //this._columngrid = new Grid();
            this._columnpanel = new StackPanel();

            this._purgeInactive = false;
            this.DisabledParentCount = 0;
            this.HiddenParentCount = 0;

            this.LoadXml(SourceXml);
            //this.Build();
        }
        #endregion

        //Setup events and handlers
        #region
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }

        public event ParentHide ParentHide;
        public event ParentEnable ParentEnable;

        public void OnGroupStateChange()
        {
            GroupingLogic.EvaluateGroups(this);
        }

        public void OnParentHide(bool Hide)
        {           
            GroupingLogic.OnParentHide(this, Hide);
        }

        public void OnParentEnable(bool Enable)
        {
            GroupingLogic.OnParentEnable(this, Enable);
        }
        #endregion

        private void LoadXml(XElement InputXml)
        {
            IEnumerable<XElement> xlist;
            IGuiOption_2 newOption;
            bool purgeset = false;

            this.PurgeInactive = XmlHandler.GetBoolFromXAttribute(InputXml, "PurgeInactive", this.PurgeInactive);
            this.LabelWidth = XmlHandler.GetGridLengthFromXElement(InputXml, "LabelWidth", this.LabelWidth);
            this.ControlWidth = XmlHandler.GetGridLengthFromXElement(InputXml, "ControlWidth", this.ControlWidth);
            this.Width = XmlHandler.GetGridLengthFromXElement(InputXml, "Width", this.Width);
            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.Parent.ShowGridLines);

            IEnumerable<XElement> xGroups = InputXml.Elements("Group");
            if (xGroups != null)
            {
                foreach (XElement xGroup in xGroups)
                { this._groups.Add(this._controller.AddToGroup(xGroup.Value, this)); }
            }

            //now read in the options and add to a dictionary for later use
            //do this last so the event subscriptions don't get setup too early (no toggles fired 
            //until everything is loaded.
            xlist = InputXml.Elements("GuiOption");
            if (xlist != null)
            {
                foreach (XElement xOption in xlist)
                {
                    newOption = GuiFactory.CreateGuiOption_2(xOption,this,this._controller);
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
