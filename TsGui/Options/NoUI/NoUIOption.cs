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

//  NoUIOption.cs - used to create TsVariables but aren't shown in the gui. 

using System;
using System.Xml.Linq;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;
using TsGui.Diagnostics.Logging;
using TsGui.Diagnostics;

namespace TsGui.Options.NoUI
{
    public class NoUIOption: GroupableBlindBase,IOption, ILinkTarget
    {
        public event IOptionValueChanged ValueChanged;

        private string _inactivevalue = "TSGUI_INACTIVE";
        private string _value = string.Empty;
        private bool _usecurrent = false;
        private QueryList _querylist;
        private string _id;

        //properties
        public string ID
        {
            get { return this._id; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) == true) { throw new TsGuiKnownException("Empty ID set on NoUI option", ""); }
                if (this._id != value)
                {
                    this._id = value;
                    this._director.LinkingLibrary.AddSource(this);
                }
            }
        }
        public string VariableName { get; set; }
        public string InactiveValue
        {
            get { return this._inactivevalue; }
            set { this._inactivevalue = value; }
        }
        public string CurrentValue
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.NotifyUpdate();
            }
        }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this._value); }
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
                    else { return this._inactivevalue; }
                }
            }
        }
        //constructors     
        public NoUIOption(NoUIContainer Parent, IDirector director, XElement InputXml) : base(Parent, director)
        {
            this._querylist = new QueryList(this, director);
            this.LoadXml(InputXml);
            this.RefreshValue();
            this.NotifyUpdate();
        }

        public NoUIOption(IDirector director):base (director)
        {
            this._querylist = new QueryList(this, director);
        }

        //public methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", this.VariableName);

            this.VariableName = XmlHandler.GetStringFromXAttribute(InputXml, "Variable", this.VariableName);
            

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
            }
        }

        public void RefreshValue()
        {
            this._value = this._querylist.GetResultWrangler()?.GetString();
            this.NotifyUpdate();
        }

        public void ImportFromTsVariable(TsVariable var)
        {
            this.VariableName = var.Name;
            ValueOnlyQuery newvoquery = new ValueOnlyQuery(var.Value);
            this._querylist.AddQuery(newvoquery);
            this.ID = var.Name;
            this.RefreshValue();
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

        protected override void EvaluateGroups()
        {
            base.EvaluateGroups();
            this.NotifyUpdate();
        }

        protected void NotifyUpdate()
        {
            LoggerFacade.Info(this.VariableName + " variable value changed. New value: " + this.LiveValue);
            this.OnPropertyChanged(this, "CurrentValue");
            this.OnPropertyChanged(this, "LiveValue");
            this.ValueChanged?.Invoke();
        }
    }
}
