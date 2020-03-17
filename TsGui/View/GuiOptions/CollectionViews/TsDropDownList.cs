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

// TsDropDownList.cs - combobox control for user input

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Windows;

using TsGui.Grouping;
using TsGui.Validation;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class TsDropDownList : CollectionViewGuiOptionBase
    {
        private TsDropDownListUI _dropdownlistui;

        //properties
        public List<ListItem> VisibleOptions { get { return this._builder.Items.Where(x => x.IsEnabled == true).ToList(); } }
        
        //Constructor
        public TsDropDownList(XElement InputXml, TsColumn Parent) : base(Parent)
        {
            this._dropdownlistui = new TsDropDownListUI();
            this.Control = this._dropdownlistui;
            this.InteractiveControl = this._dropdownlistui.Control;
            this.Label = new TsLabelUI();
            this.ValidationHandler = new ValidationHandler(this);
            this._validationtooltiphandler = new ValidationToolTipHandler(this);
            this.UserControl.DataContext = this;

            this.SetDefaults();
            this.LoadXml(InputXml);
            this._builder.Rebuild();
            this.RegisterForItemGroupEvents();
            this.SetComboBoxDefault();

            Director.Instance.WindowLoaded += this.OnLoadReload;
            this._dropdownlistui.Control.SelectionChanged += this.OnSelectionChanged;
            this._dropdownlistui.Control.LostFocus += this.OnLostFocus;
            this.UserControl.IsEnabledChanged += this.OnActiveChanged;
            this.UserControl.IsVisibleChanged += this.OnActiveChanged;
        }

        private void SetComboBoxDefault()
        {
            ListItem newdefault = null;

            if (this._nodefaultvalue == false)
            {
                int index = 0;
                string defaultval = this._setvaluequerylist.GetResultWrangler()?.GetString();
                foreach (ListItem item in this.VisibleOptions)
                {
                    if ((item.Value == defaultval) || (index == 0))
                    {
                        newdefault = item;
                        if (index > 0) { break; }
                    }
                    index++;
                }
            }
            this.CurrentItem = newdefault;
            this.NotifyUpdate();
        }

        protected override void SetSelected(string value)
        {
            ListItem newdefault = null;
            bool changed = false;

            foreach (ListItem item in this.VisibleOptions)
            {
                if ((item.Value == value))
                {
                    newdefault = item;
                    changed = true;
                    break;
                }
            }

            if (changed == true)
            {
                this.CurrentItem = newdefault;
            }
        }

        public void OnDropDownListItemGroupEvent()
        {
            this.OnOptionsListUpdated();
        }

        //Method to work around an issue where dropdown doesn't grey the text if disabled. This opens
        //and closes the dropdown so it initialises proeprly
        public void OnLoadReload(object o, RoutedEventArgs e)
        {
            this._dropdownlistui.Control.IsDropDownOpen = true;
            this._dropdownlistui.Control.IsDropDownOpen = false;
        }

        private void OnOptionsListUpdated()
        {
            this.OnPropertyChanged(this, "VisibleOptions");
            this.SetComboBoxDefault();
        }

        
        private void RegisterForItemGroupEvents()
        {
            foreach (Group g in this._itemGroups.Values)
            {
                g.StateEvent += this.OnDropDownListItemGroupEvent;
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (this.CurrentItem == null)
            {
                SetComboBoxDefault();
            }
            else
            {
                //this makes sure the text is updated properly if the user has left it half complete
                this._dropdownlistui.Control.Text = this.CurrentItem.Text;
            }
        }
    }
}
