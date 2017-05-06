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

namespace TsGui.View.GuiOptions.Trees
{
    public class TsTree : GuiOptionBase, IGuiOption, ILinkTarget
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
        public TsTree(XElement InputXml, TsColumn Parent, IDirector MainController) : base(Parent,MainController)
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
            XElement x;
            //IEnumerable<XElement> xlist;

            //load the xml for the base class stuff
            base.LoadXml(InputXml);


            //xlist = InputXml.Elements("Toggle");
            //if (xlist != null)
            //{
            //    this._director.AddToggleControl(this);

            //    foreach (XElement subx in xlist)
            //    {
            //        Toggle t = new Toggle(this, this._director, subx); 
            //    }  
            //}
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
