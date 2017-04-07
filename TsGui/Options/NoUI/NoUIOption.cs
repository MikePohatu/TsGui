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

using System.Xml.Linq;
using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;
using TsGui.Diagnostics.Logging;

namespace TsGui.Options.NoUI
{
    public class NoUIOption: GroupableBlindBase,IOption,ILinkingSource
    {
        public event IOptionValueChanged ValueChanged;

        private string _inactivevalue = "TSGUI_INACTIVE";
        private string _value = string.Empty;
        private bool _usecurrent = false;
        private QueryList _setvaluelist;

        //properties
        public string ID { get; set; }
        public string VariableName { get; set; }
        public string InactiveValue
        {
            get { return this._inactivevalue; }
            set { this._inactivevalue = value; }
        }
        public string CurrentValue { get { return this._value; } }
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
        public NoUIOption(NoUIContainer Parent, MainController MainController, XElement InputXml) : base(Parent,MainController)
        {
            this._setvaluelist = new QueryList(this, MainController);
            this.LoadXml(InputXml);
            this.RefreshValue();
            this.NotifyUpdate();
        }

        //public methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.VariableName = XmlHandler.GetStringFromXElement(InputXml, "Variable", this.VariableName);

            this.VariableName = XmlHandler.GetStringFromXAttribute(InputXml, "Variable", this.VariableName);
            this._value = XmlHandler.GetStringFromXAttribute(InputXml, "Value", this._value);

            this.InactiveValue = XmlHandler.GetStringFromXElement(InputXml, "InactiveValue", this.InactiveValue);
            this._usecurrent = XmlHandler.GetBoolFromXAttribute(InputXml, "UseCurrent", this._usecurrent);

            XElement x;

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

                this._setvaluelist.LoadXml(x);
                //this._value = this._controller.EnvironmentController.GetStringValueFromList(x);
            }


            XAttribute xa = InputXml.Attribute("ID");
            if (xa != null)
            {
                this.ID = xa.Value;
                this._controller.LinkingLibrary.AddSource(this);
            }
        }

        private void RefreshValue()
        { this._value = this._setvaluelist.GetResultWrangler()?.GetString(); }

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
            this.ValueChanged?.Invoke(this,new LinkingEventArgs(this.CurrentValue));
        }

        public void OnLinkedValueChanged()
        { }
    }
}
