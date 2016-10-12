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

// GuiOptionBase.cs - base class for the rest of the gui options to inherit

using System.Windows;
using System.Xml.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace TsGui.View.GuiOptions
{
    public abstract class GuiOptionBase: INotifyPropertyChanged, IGroupChild, IGroupable
    {
        //Fields
        #region
        protected int _height;
        protected int _width;

        protected int _controlwidth;
        protected Thickness _controlmargin;
        protected Thickness _controlpadding;
        protected VerticalAlignment _controlverticalalign;        

        protected string _labeltext;
        protected int _labelwidth;
        protected Thickness _labelmargin;
        protected Thickness _labelpadding;
        protected VerticalAlignment _labelverticalalign;        

        protected bool _isenabled = true;
        protected bool _ishidden = false;
        protected Visibility _visibility = Visibility.Visible;

        protected List<Group> _groups = new List<Group>();
        protected int _hiddenParents = 0;
        protected int _disabledParents = 0;
        #endregion

        //Properties
        #region 

        public List<Group> Groups { get { return this._groups; } }
        public int GroupCount { get { return this._groups.Count; } }
        public int DisabledParentCount { get; set; }
        public int HiddenParentCount { get; set; }

        public bool IsEnabled
        {
            get { return this._isenabled; }
            set { this._isenabled = value; this.OnPropertyChanged(this, "IsEnabled"); }
        }

        public bool IsHidden
        {
            get { return this._ishidden; }
            set { this.HideUnhide(value); this.OnPropertyChanged(this, "IsHidden"); }
        }

        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false)) { return true; }
                else { return false; }
            }
        }
        public Visibility Visibility
        {
            get { return this._visibility; }
            set { this._visibility = value; this.OnPropertyChanged(this, "Visibility"); }
        }
        public int Height
        {
            get { return this._height; }
            set { this._height = value; this.OnPropertyChanged(this, "Height"); }
        }

        public int Width
        {
            get { return this._width; }
            set { this._width = value; this.OnPropertyChanged(this, "Width"); }
        }

        public int LabelWidth
        {
            get { return this._labelwidth; }
            set { this._labelwidth = value; this.OnPropertyChanged(this, "LabelWidth"); }
        }

        public string LabelText
        {
            get { return this._labeltext; }
            set { this._labeltext = value; this.OnPropertyChanged(this, "LabelText"); }
        }

        public Thickness LabelMargin
        {
            get { return this._labelmargin; }
            set { this._labelmargin = value; this.OnPropertyChanged(this, "LabelMargin"); }
        }

        public Thickness LabelPadding
        {
            get { return this._labelpadding; }
            set { this._labelpadding = value; this.OnPropertyChanged(this, "LabelPadding"); }
        }

        public VerticalAlignment LabelVerticalAlign
        {
            get { return this._labelverticalalign; }
            set { this._labelverticalalign = value; this.OnPropertyChanged(this, "LabelVerticalAlign"); }
        }

        public VerticalAlignment ControlVerticalAlign
        {
            get { return this._controlverticalalign; }
            set { this._controlverticalalign = value; this.OnPropertyChanged(this, "ControlVerticalAlign"); }
        }

        public int ControlWidth
        {
            get { return this._controlwidth; }
            set { this._controlwidth = value; this.OnPropertyChanged(this, "ControlWidth"); }
        }

        public Thickness ControlPadding
        {
            get { return this._controlpadding; }
            set { this._controlpadding = value; this.OnPropertyChanged(this, "ControlPadding"); }
        }

        public Thickness ControlMargin
        {
            get { return this._controlmargin; }
            set { this._controlmargin = value; this.OnPropertyChanged(this, "ControlMargin"); }
        }
        #endregion


        //Event handling
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }

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

        protected void LoadXml(XElement InputXml)
        {
            //Load the XML
            #region
            this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", this.LabelText);
            this.Height = XmlHandler.GetIntFromXElement(InputXml, "Height", this.Height);
            this.Width = XmlHandler.GetIntFromXElement(InputXml, "Width", this.Width);
            this.ControlWidth = XmlHandler.GetIntFromXElement(InputXml, "ControlWidth", this.ControlWidth);
            this.ControlPadding = XmlHandler.GetThicknessFromXElement(InputXml, "ControlPadding", 0);
            this.LabelWidth = XmlHandler.GetIntFromXElement(InputXml, "LabelWidth", this.LabelWidth);
            this.LabelPadding = XmlHandler.GetThicknessFromXElement(InputXml, "LabelPadding", 0);

            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);

            IEnumerable<XElement> xGroups = InputXml.Elements("Group");
            if (xGroups != null)
            {
                //foreach (XElement xGroup in xGroups)
                //{ this._groups.Add(this._controller.AddToGroup(xGroup.Value, this)); }
            }
            #endregion
        }

        private void HideUnhide(bool Hidden)
        {
            this._ishidden = Hidden;
            if (Hidden == true)
            { this.Visibility = Visibility.Collapsed; }
            else
            { this.Visibility = Visibility.Visible; }
        }
    }
}
