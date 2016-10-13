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

// TsRow.cs - class for rows in the gui window

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

namespace TsGui.View.Layout
{
    public class TsRow : IGroupParent, IGroupChild, INotifyPropertyChanged
    {
        private bool _enabled;
        private bool _purgeInactive;
        private List<Group> _groups = new List<Group>();
        private bool _hidden;
        private Grid _columngrid;
        private bool _gridlines;
        private MainController _controller;
        private List<TsColumn> _columns = new List<TsColumn>();
        private TsPage _parent;
        private List<IGuiOption> _options = new List<IGuiOption>();
        private double _height;

        //properties
        #region
        public List<IGuiOption> Options { get { return this._options; } }
        public TsPage Parent { get { return this._parent; } }
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
                foreach (TsColumn column in this._columns)
                { column.PurgeInactive = value; }
            }
        }
        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set { this._gridlines = value; this.OnPropertyChanged(this, "ShowGridLines"); }
        }

        public double Height
        {
            get { return this._height; }
            set { this._height = value; this.OnPropertyChanged(this, "Height"); }
        }
        public int Index { get; set; }
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
        public TsRow (XElement SourceXml,int PageIndex, TsPage ParentPage, MainController RootController)
        {

            this._controller = RootController;
            this._parent = ParentPage;
            this.Index = PageIndex;
            this._columngrid = new Grid();

            this._columngrid.DataContext = this;
            this._columngrid.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));

            //Set defaults
            this.IsEnabled = true;
            this.IsHidden = false;
            this.Height = this.Parent.Height;

            this._columngrid.VerticalAlignment = VerticalAlignment.Top;

            this._purgeInactive = false;
            this.DisabledParentCount = 0;
            this.HiddenParentCount = 0;

            this.LoadXml(SourceXml);
            //this.PopulateOptions();
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
            IEnumerable<XElement> xlist;
            int colIndex = 0;
            bool purgeset = false;

            this.PurgeInactive = XmlHandler.GetBoolFromXAttribute(InputXml, "PurgeInactive", this.PurgeInactive);
            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);

            IEnumerable<XElement> xGroups = InputXml.Elements("Group");
            if (xGroups != null)
            {
                foreach (XElement xGroup in xGroups)
                { this._groups.Add(this._controller.AddToGroup(xGroup.Value, this)); }
            }

            xlist = InputXml.Elements("Column");
            if (xlist != null)
            {
                foreach (XElement xColumn in xlist)
                {
                    TsColumn c = new TsColumn(xColumn, colIndex, this, this._controller);
                    if (purgeset == true) { c.PurgeInactive = this.PurgeInactive; }

                    this._columns.Add(c);
                    this._options.AddRange(c.Options);
                    //Debug.WriteLine("TsRow- Registering column");
                    this.ParentHide += c.OnParentHide;
                    this.ParentEnable += c.OnParentEnable;

                    colIndex++;
                }
            }
        }

        public void Build()
        {
            int index = 0;
            
            
            foreach (TsColumn column in this._columns)
            {
                //option.Control.Margin = this._margin;
                //option.Label.Margin = this._margin;

                ColumnDefinition coldef = new ColumnDefinition();
                coldef.Width = GridLength.Auto;
                //coldefRow.Height = new GridLength(option.Height + option.Margin.Top + option.Margin.Bottom) ;
                this._columngrid.ColumnDefinitions.Add(coldef);

                Grid.SetColumn(column.Panel, index);

                this._columngrid.Children.Add(column.Panel);

                index++;
            }
        }
    }
}
