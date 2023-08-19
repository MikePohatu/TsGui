#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

// GuiOptionBase.cs - base parts for all GuiOptions

using System;
using System.Xml.Linq;
using TsGui.View.Layout;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using TsGui.Linking;
using TsGui.Options;
using Core.Logging;
using TsGui.Queries;
using TsGui.Grouping;
using System.Collections.Generic;
using MessageCrap;
using System.Threading.Tasks;
using TsGui.View.Layout.Events;

namespace TsGui.View.GuiOptions
{
    public abstract class GuiOptionBase : BaseLayoutElement, IOption, IToggleControl
    {
        private string _labeltext = string.Empty;
        private string _helptext = null;
        protected QueryPriorityList _querylist;


        public bool IsToggle { get; set; }
        public bool IsRendered { get; private set; } = false;
        public string ID { get; set; }
        public string Path { get; set; }
        public UserControl Control { get; set; }
        public UserControl Label { get; set; }
        public GuiOptionBaseUI UserControl { get; set; }
        public string InactiveValue { get; set; } = "TSGUI_INACTIVE";
        public string VariableName { get; set; }
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
        public virtual Control InteractiveControl { get; set; }
        public abstract string CurrentValue { get; }
        public abstract Variable Variable { get; }
        public string LiveValue
        {
            get
            {
                if (this.IsActive == true) { return this.CurrentValue; }
                else
                {
                    if (this._purgeinactive == true) { return "*PURGED*"; }
                    else { return this.InactiveValue; }
                }
            }
        }
        
        
        public GuiOptionBase(ParentLayoutElement Parent):base(Parent)
        {
            this.UserControl = new GuiOptionBaseUI();
            this.UserControl.Loaded += this.OnRendered;
        }

        public void OnRendered (object sender, EventArgs e)
        {
            this.IsRendered = true;
            this.Control.MouseEnter += OnControlGotFocus;
            this.Control.MouseLeave += OnControlLostFocus;
            this.Control.TouchEnter += OnControlGotFocus;
            this.Control.TouchLeave += OnControlLostFocus;
        }

        private void OnControlGotFocus(object sender, RoutedEventArgs e)
        {
            this.Events.InvokeLayoutEvent(LayoutTopics.ControlGotFocus, EventDirection.Tunnel);
        }

        private void OnControlLostFocus(object sender, RoutedEventArgs e)
        {
            this.Events.InvokeLayoutEvent(LayoutTopics.ControlLostFocus, EventDirection.Tunnel);
        }

        protected new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            //load legacy
            XElement x;
            x = InputXml.Element("Bold");
            if (x != null) { this.LabelStyle.FontWeight = "Bold"; }

            //path and variable can be set either as an element, or an attribute
            this.Path = XmlHandler.GetStringFromXml(InputXml, "Path", this.Path);
            this.Path = XmlHandler.GetStringFromXml(InputXml, "Path", this.Path);
            this.VariableName = XmlHandler.GetStringFromXml(InputXml, "Variable", this.VariableName);
            this.VariableName = XmlHandler.GetStringFromXml(InputXml, "Variable", this.VariableName);

            this.LabelText = XmlHandler.GetStringFromXml(InputXml, "Label", this.LabelText);
            this.HelpText = XmlHandler.GetStringFromXml(InputXml, "HelpText", this.HelpText);
            this.ShowGridLines = XmlHandler.GetBoolFromXml(InputXml, "ShowGridLines", this.Parent.ShowGridLines);
            this.InactiveValue = XmlHandler.GetStringFromXml(InputXml, "InactiveValue", this.InactiveValue);
            this.SetLayoutRightLeft();

            XAttribute xa = InputXml.Attribute("ID");
            if (xa != null) { this.ID = xa.Value; }

            x = InputXml.Element("SetValue");
            if (x != null)
            {
                this.LoadSetValueXml(x,true);
            }

            IEnumerable<XElement> xlist = InputXml.Elements("Toggle");
            if (xlist != null)
            {
                Director.Instance.AddToggleControl(this);

                foreach (XElement subx in xlist)
                {
                    new Toggle(this, subx);
                    this.IsToggle = true;
                }
            }

            if (this.IsToggle == true) { Director.Instance.AddToggleControl(this); }
        }

        //Grouping stuff
        #region
        public event ToggleEvent ToggleEvent;

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        {
            this.InvokeToggleEvent();
        }

        //This is called by the Director once everything is loaded
        public async Task InitialiseAsync()
        {
            this.UserControl.IsEnabledChanged += this.OnGroupStateChanged;
            this.UserControl.IsVisibleChanged += this.OnGroupStateChanged;
            await this.UpdateValueAsync(null);
        }

        public void InvokeToggleEvent()
        {
            this.ToggleEvent?.Invoke();
        }

        protected override void EvaluateGroups()
        {
            base.EvaluateGroups();
            this.NotifyViewUpdate();
        }

        protected void OnGroupStateChanged(object o, RoutedEventArgs e)
        {
            this.InvokeToggleEvent();
        }

        protected void OnGroupStateChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.InvokeToggleEvent();
        }
        #endregion

        protected void LoadSetValueXml(XElement inputxml, bool clearbeforeload)
        {
            XAttribute xusecurrent = inputxml.Attribute("UseCurrent");
            if (xusecurrent != null)
            {
                //default behaviour is to check if the ts variable is already set. If it is, set that as the default i.e. add a query for 
                //an environment variable to the start of the query list. 
                if (!string.Equals(xusecurrent.Value, "false", StringComparison.OrdinalIgnoreCase))
                {
                    XElement xcurrentquery = new XElement("Query", new XElement("Variable", this.VariableName));
                    xcurrentquery.Add(new XAttribute("Type", "EnvironmentVariable"));
                    inputxml.AddFirst(xcurrentquery);
                }
            }

            //_setvaluequerylist might be null e.g. on passwordboxes
            if (this._querylist != null)
            {
                if (clearbeforeload == true) { this._querylist.Clear(); }
                this._querylist.LoadXml(inputxml);
            }
            
        }

        private void SetLayoutRightLeft()
        {
            if (this.Style.LabelOnRight == false)
            {
                this.UserControl.RightPresenter.Content = this.Control;
                this.UserControl.LeftPresenter.Content = this.Label;
            }
            else
            {
                this.UserControl.RightPresenter.Content = this.Label;
                this.UserControl.LeftPresenter.Content = this.Control;
            }
        }

        protected void NotifyViewUpdate()
        {
            this.OnPropertyChanged(this, "CurrentValue");
            this.OnPropertyChanged(this, "LiveValue");
            Log.Info(this.VariableName + " variable value changed. New value: " + this.LiveValue);
        }

        public abstract Task UpdateValueAsync(Message message);
    }
}
