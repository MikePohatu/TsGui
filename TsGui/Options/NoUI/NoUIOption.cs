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

//  NoUIOption.cs - used to create TsVariables but aren't shown in the gui. 

using System;
using System.Xml.Linq;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;
using TsGui.Diagnostics.Logging;
using TsGui.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using MessageCrap;

namespace TsGui.Options.NoUI
{
    public class NoUIOption: GroupableBlindBase, IOption, ILinkTarget, IToggleControl
    {
        private string _value = string.Empty;
        private bool _usecurrent = false;
        private QueryPriorityList _querylist;
        private string _id;

        //properties
        public bool IsToggle { get; set; }
        public string Path { get; set; }
        public string ID
        {
            get { return this._id; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) == true) { throw new TsGuiKnownException("Empty ID set on NoUI option", ""); }
                if (this._id != value) { this._id = value; }
            }
        }
        public string VariableName { get; set; }
        public string InactiveValue { get; set; } = "TSGUI_INACTIVE";
        public string CurrentValue
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.NotifyViewUpdate();
            }
        }
        public Variable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new Variable(this.VariableName, this._value, this.Path); }
            }
        }
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
        //constructors     
        public NoUIOption(NoUIContainer Parent, XElement InputXml) : base(Parent)
        {
            this.Path = Director.Instance.DefaultPath;
            this._querylist = new QueryPriorityList(this);
            this.LoadXml(InputXml);
            this.NotifyViewUpdate();
        }

        public NoUIOption():base ()
        {
            this.Path = Director.Instance.DefaultPath;
            this._querylist = new QueryPriorityList(this);
        }

        //public methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            //path and variable can be set either as an element, or an attribute
            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", this.VariableName);
            this.VariableName = XmlHandler.GetStringFromXAttribute(InputXml, "Variable", this.VariableName);
            this.Path = XmlHandler.GetStringFromXElement(InputXml, "Path", this.Path);
            this.Path = XmlHandler.GetStringFromXAttribute(InputXml, "Path", this.Path);

            this.InactiveValue = XmlHandler.GetStringFromXElement(InputXml, "InactiveValue", this.InactiveValue);
            this._usecurrent = XmlHandler.GetBoolFromXAttribute(InputXml, "UseCurrent", this._usecurrent);

            XElement x;
            XAttribute xa;

            xa = InputXml.Attribute("Value");
            if (xa != null)
            {
                ValueOnlyQuery newvalue = new ValueOnlyQuery(InputXml);
                newvalue.Value = xa.Value;
                this._querylist.AddQuery(newvalue);
            }


            x = InputXml.Element("SetValue");
            if (x != null)
            {
                if (this._usecurrent == true)
                {
                    //default behaviour is to NOT check if the ts variable is already set. If it is, set that as the default i.e. add a query for 
                    //an environment variable to the start of the query list. 
                    XElement xcurrentquery = new XElement("Query", new XElement("Variable", this.VariableName));
                    xcurrentquery.Add(new XAttribute("Type", "EnvironmentVariable"));
                    x.AddFirst(xcurrentquery);
                }

                this._querylist.LoadXml(x);
            }


            xa = InputXml.Attribute("ID");
            if (xa != null)
            {
                this.ID = xa.Value;
                Director.Instance.LinkingHub.AddSource(this);
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

        public void UpdateValue(Message message)
        {
            this._value = this._querylist.GetResultWrangler(message)?.GetString();

            LinkingHub.Instance.SendUpdateMessage(this, message);

            this.NotifyViewUpdate();
        }

        public void OnSourceValueUpdated(Message message)
        {
            this.UpdateValue(message);
        }

        public void ImportFromTsVariable(Variable var)
        {
            this.VariableName = var.Name;
            ValueOnlyQuery newvoquery = new ValueOnlyQuery(var.Value);
            this._querylist.AddQuery(newvoquery);
            this.ID = var.Name;
            this.Path = var.Path;
            this.UpdateValue(null);
        }

        protected void LoadSetValueXml(XElement inputxml)
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
            this._querylist.Clear();
            this._querylist.LoadXml(inputxml);
        }

        protected void NotifyViewUpdate()
        {
            LoggerFacade.Info(this.VariableName + " variable value changed. New value: " + this.LiveValue);
            this.OnPropertyChanged(this, "CurrentValue");
            this.OnPropertyChanged(this, "LiveValue");
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

        //This is called by the controller once everything is loaded
        public void Initialise()
        {
            this.UpdateValue(null);
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
    }
}
