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

// TsCheckBox.cs - combobox control for user input

using System;
using System.Xml.Linq;
using System.Windows;
using System.Collections.Generic;

using TsGui.Grouping;
using TsGui.Linking;
using TsGui.Queries;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class TsTreeView : GuiOptionBase, IGuiOption, ILinkTarget
    {
        //public event ToggleEvent ToggleEvent;

        private string _val;

        //public override string CurrentValue { get { return this.ControlText; } }
        public override string CurrentValue { get { return this._val; } }
        public override TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.CurrentValue); }
            }
        }


        //Constructor
        public TsTreeView(XElement InputXml, TsColumn Parent, IDirector MainController) : base(Parent,MainController)
        {
            this.UserControl.DataContext = this;           
            this.Control = new TsTreeViewUI();
            this.Label = new TsLabelUI();
            this.SetDefaults();
            this._setvaluequerylist = new QueryPriorityList(this, this._director);          
            this.LoadXml(InputXml);
            //this.UserControl.IsEnabledChanged += this.OnGroupStateChanged;
            //this.UserControl.IsVisibleChanged += this.OnGroupStateChanged;
        }


        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            IEnumerable<XElement> inputElements = InputXml.Elements();

            //this._validationhandler.AddValidations(InputXml.Elements("Validation"));
            //this._nodefaultvalue = XmlHandler.GetBoolFromXAttribute(InputXml, "NoDefaultValue", this._nodefaultvalue);
            //this._noselectionmessage = XmlHandler.GetStringFromXElement(InputXml, "NoSelectionMessage", this._noselectionmessage);

            foreach (XElement x in inputElements)
            {
                //the base loadxml will create queries before this so will win
                if (x.Name == "DefaultValue")
                {
                    IQuery defquery = QueryFactory.GetQueryObject(new XElement("Value", x.Value), this._director, this);
                    this._setvaluequerylist.AddQuery(defquery);
                }

                //read in an option and add to a dictionary for later use
                //else if (x.Name == "Option")
                //{
                //    TsDropDownListItem newoption = new TsDropDownListItem(x, this.ControlFormatting, this, this._director);
                //    this._builder.Add(newoption);

                //    IEnumerable<XElement> togglexlist = x.Elements("Toggle");
                //    foreach (XElement togglex in togglexlist)
                //    {
                //        togglex.Add(new XElement("Enabled", newoption.Value));
                //        Toggle t = new Toggle(this, this._director, togglex);
                //        this._istoggle = true;
                //    }
                //}

                //else if (x.Name == "Query")
                //{
                //    XElement wrapx = new XElement("wrapx");
                //    wrapx.Add(x);
                //    QueryPriorityList newlist = new QueryPriorityList(this, this._director);
                //    newlist.LoadXml(wrapx);

                //    this._builder.Add(newlist);
                //}

                //else if (x.Name == "Toggle")
                //{
                //    Toggle t = new Toggle(this, this._director, x);
                //    this._istoggle = true;
                //}
            }

            //if (this._istoggle == true) { this._director.AddToggleControl(this); }
        }

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        //public void InitialiseToggle()
        //{
        //    this.ToggleEvent?.Invoke();
        //}

        public void RefreshValue()
        {

        }

        public void RefreshAll()
        {
            this.RefreshValue();
        }

        //private void OnGroupStateChanged(object o, RoutedEventArgs e)
        //{
        //    this.ToggleEvent?.Invoke();
        //}

        //private void OnGroupStateChanged(object o, DependencyPropertyChangedEventArgs e)
        //{
        //    this.ToggleEvent?.Invoke();
        //}

        private void SetDefaults()
        {
            this.ControlFormatting.Padding = new Thickness(0, 0, 0, 0);
            this.ControlFormatting.Margin = new Thickness(1, 1, 1, 1);
            this.ControlFormatting.VerticalAlignment = VerticalAlignment.Center;
        }

        private void LoadLegacyXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("HAlign");
            if (x != null)
            {
                string s = x.Value.ToUpper();
                switch (s)
                {
                    case "RIGHT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "LEFT":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "CENTER":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "STRETCH":
                        this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
