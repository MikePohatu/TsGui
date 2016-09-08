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
using System;

namespace TsGui
{
    public class TsColumn : IGroupParent, IGroupChild, INotifyPropertyChanged
    {
        private bool _enabled;
        private bool _purgeInactive;
        private List<Group> _groups = new List<Group>();
        private bool _hidden;
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid _columngrid;
        private bool _gridlines;
        private GridLength _labelwidth;
        private GridLength _controlwidth;
        private GridLength _fullwidth;
        private MainController _controller;
        private ColumnDefinition _coldefControls;
        private ColumnDefinition _coldefLabels;
        private TsPage _parent;

        //properties
        #region
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
                foreach (IGuiOption option in this.options)
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
        public List<IGuiOption> Options { get { return this.options; } }
        public Panel Panel { get { return this._columngrid; } }
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
        public TsColumn (XElement SourceXml,int PageIndex, TsPage ParentPage, MainController RootController)
        {

            this._controller = RootController;
            this._parent = ParentPage;
            this.Index = PageIndex;
            this._columngrid = new Grid();

            this._columngrid.Margin = new Thickness(0);
            this._coldefControls = new ColumnDefinition();
            this._coldefLabels = new ColumnDefinition();

            this._columngrid.DataContext = this;
            this._columngrid.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));

            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefControls.SetBinding(ColumnDefinition.WidthProperty, new Binding("ControlWidth"));

            //Set defaults
            this._columngrid.VerticalAlignment = VerticalAlignment.Top;

            this._columngrid.ColumnDefinitions.Add(this._coldefLabels);
            this._columngrid.ColumnDefinitions.Add(this._coldefControls);

            this._purgeInactive = false;
            this.DisabledParentCount = 0;
            this.HiddenParentCount = 0;

            this.LoadXml(SourceXml);
            this.Build();
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
            XElement x;
            IEnumerable<XElement> optionsXml;
            IGuiOption newOption;
            XAttribute xAttrib;
            bool purgeset = false;

            xAttrib = InputXml.Attribute("PurgeInactive");
            if (xAttrib != null)
            {
                purgeset = true;
                this.PurgeInactive = Convert.ToBoolean(xAttrib.Value);
            }

            IEnumerable<XElement> xGroups = InputXml.Elements("Group");
            if (xGroups != null)
            {
                foreach (XElement xGroup in xGroups)
                { this._groups.Add(this._controller.AddToGroup(xGroup.Value, this)); }
            }

            //now read in the options and add to a dictionary for later use
            optionsXml = InputXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    newOption = GuiFactory.CreateGuiOption(xOption,this._parent,this._controller);
                    if (purgeset == true) { newOption.PurgeInactive = this.PurgeInactive; }
                    this.options.Add(newOption);
                    this._controller.AddOptionToLibary(newOption);

                    //register for events
                    this.ParentEnable += newOption.OnParentEnable;
                    this.ParentHide += newOption.OnParentHide;
                }
            }

            x = InputXml.Element("LabelWidth");
            if (x != null)
            { this.LabelWidth = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("ControlWidth");
            if (x != null)
            { this.ControlWidth = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("Width");
            if (x != null)
            { this.Width = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("Enabled");
            if (x != null)
            { this.IsEnabled = Convert.ToBoolean(x.Value); }

            x = InputXml.Element("Hidden");
            if (x != null)
            { this.IsHidden = Convert.ToBoolean(x.Value); }
        }


        public void Build()
        {
            int rowindex = 0;
            double width =0;
            
            
            foreach (IGuiOption option in this.options)
            {
                //option.Control.Margin = this._margin;
                //option.Label.Margin = this._margin;

                RowDefinition coldefRow = new RowDefinition();
                coldefRow.Height = GridLength.Auto;
                //coldefRow.Height = new GridLength(option.Height + option.Margin.Top + option.Margin.Bottom) ;
                this._columngrid.RowDefinitions.Add(coldefRow);

                Grid.SetColumn(option.Label, 0);
                Grid.SetColumn(option.Control, 1);
                Grid.SetRow(option.Label, rowindex);
                Grid.SetRow(option.Control, rowindex);

                this._columngrid.Children.Add(option.Label);
                this._columngrid.Children.Add(option.Control);

                //Debug.WriteLine("Control width (" + option.Label.Content + "): " + width);
                if (width < option.Control.Width)
                {                   
                    width = option.Control.Width;
                }

                rowindex++;
            }
        }
    }
}
