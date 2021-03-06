﻿#region license
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
using System.Windows;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Controls;
using System.Xml.Linq;
using System;

using TsGui.Linking;
using TsGui.Queries;
using TsGui.Grouping;
using TsGui.Validation;
using MessageCrap;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public abstract class CollectionViewGuiOptionBase : GuiOptionBase, ILinkTarget, IValidationGuiOption
    {        
        protected ListItem _currentitem;
        protected string _validationtext;
        protected ValidationToolTipHandler _validationtooltiphandler;
        public ValidationHandler ValidationHandler { get; protected set; }

        protected bool _nodefaultvalue;
        protected string _noselectionmessage;
        protected ListBuilder _builder;
        protected Dictionary<string, Group> _itemGroups = new Dictionary<string, Group>();
        
        //properties
        public bool Sort { get; set; } = false;
        public UserControl Icon { get; set; }
        public override Variable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new Variable(this.VariableName, this.CurrentValue, this.Path); }
            }
        }
        public override string CurrentValue
        {
            get { return this._currentitem?.Value; }
        }

        /// <summary>
        /// Don't set the CurrentItem in code. Use SetValue instead or Linking won't work properly
        /// </summary>
        public ListItem CurrentItem
        {
            get { return this._currentitem; }
            set { this.SetValue(value, null); }
        }
        public bool IsValid { get { return this.Validate(); } }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }

        //Constructor
        public CollectionViewGuiOptionBase(ParentLayoutElement Parent) : base(Parent)
        {
            this._querylist = new QueryPriorityList(this);
            this._builder = new ListBuilder(this);
        }


        //Methods
        public void AddItemGroup(Group NewGroup)
        {
            Group g;
            this._itemGroups.TryGetValue(NewGroup.ID, out g);
            if (g == null) { this._itemGroups.Add(NewGroup.ID, NewGroup); }
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this._builder.LoadXml(InputXml);

            this.ValidationHandler.AddValidations(InputXml.Elements("Validation"));
            this._nodefaultvalue = XmlHandler.GetBoolFromXAttribute(InputXml, "NoDefaultValue", this._nodefaultvalue);

            this.Sort = XmlHandler.GetBoolFromXAttribute(InputXml, "Sort", this.Sort);
            this._noselectionmessage = XmlHandler.GetStringFromXElement(InputXml, "NoSelectionMessage", this._noselectionmessage);

            foreach (XElement x in InputXml.Elements())
            {
                //the base loadxml will create queries before this so it will win
                if (x.Name == "DefaultValue")
                {
                    IQuery defquery = QueryFactory.GetQueryObject(new XElement("Value", x.Value), this);
                    this._querylist.AddQuery(defquery);
                }
            }
        }

        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

        public void OnValidationChange()
        { this.Validate(false); }

        public bool Validate()
        { return this.Validate(true); }
        
        public bool Validate(bool CheckSelectionMade)
        {
            if (Director.Instance.StartupFinished == false) { return true; }
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }
            if ((CheckSelectionMade == true) && (this.CurrentItem == null))
            {
                this.ValidationText = _noselectionmessage;
                this._validationtooltiphandler.ShowError();
                return false;
            }

            bool newvalid = this.ValidationHandler.IsValid(this.CurrentValue);

            if (newvalid == false)
            {
                string validationmessage = this.ValidationHandler.ValidationMessage;
                string s = "\"" + this.CurrentItem.Text + "\" is invalid" + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + ValidationHandler.FailedValidationMessage; }
                else { s = s + validationmessage; }

                this.ValidationText = s;
                this._validationtooltiphandler.ShowError();
            }
            else { this._validationtooltiphandler.Clear(); }

            return newvalid;
        }

        public override void UpdateValue(Message message)
        {
            this._builder.Rebuild(message);
            this.OnPropertyChanged(this, "VisibleOptions");

            //SetSelected includes the messaging call
            this.SetSelected(this._querylist.GetResultWrangler(message)?.GetString(), message);
            this.NotifyViewUpdate();
        }

        public void OnSourceValueUpdated(Message message)
        {
            this.UpdateValue(message);
        }

        protected abstract void SetSelected(string input, Message message);


        protected void SetValue(ListItem value, Message message)
        {
            this._currentitem = value; 
            this.OnPropertyChanged(this, "CurrentItem");
            LinkingHub.Instance.SendUpdateMessage(this, message);
        }

        protected void SetDefaults()
        {
            this._nodefaultvalue = false;
            this._noselectionmessage = "Please select a value";
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;

            if (Director.Instance.UseTouchDefaults)
            {
                this.ControlFormatting.Padding = new Thickness(6, 5, 2, 5);
            }
            else
            {
                this.ControlFormatting.Padding = new Thickness(6, 2, 2, 3);
            }
        }

        protected void OnSelectionChanged(object o, RoutedEventArgs e)
        {
            this.Validate(false);
            LinkingHub.Instance.SendUpdateMessage(this, null);
            this.NotifyViewUpdate();
            this.InvokeToggleEvent();
        }

        protected void OnActiveChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.Validate(false);
            this.InvokeToggleEvent();
        }
    }
}
