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

// Toggle.cs - class to detect changes to IToggleControl objects and apply the changes
// to the associated group.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows;

namespace TsGui
{
    public class Toggle
    {
        private Group _group;
        private MainController _controller;
        private Dictionary<string, bool> _toggleValMappings = new Dictionary<string, bool>();
        bool _hiddenMode = false;
        IToggleControl _option;

        public Toggle(IToggleControl GuiOption, MainController MainController, XElement InputXml)
        {
            this._controller = MainController;
            this._option = GuiOption;
            this.LoadXml(InputXml);
            this._option.AttachToggle(this);
        }

        private void LoadXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("Hide");
            if (x != null)
            {
                this._hiddenMode = true;
            }

            XAttribute xa;
            xa = InputXml.Attribute("Group");
            if (xa != null)
            {
                if (!string.IsNullOrEmpty(xa.Value))
                {
                    this._group = this._controller.GetGroup(xa.Value);
                }
                else { throw new InvalidOperationException("Invalid Toggle configured in XML: " + InputXml); }
            }
            else { throw new InvalidOperationException("No Group ID set in Toggle configured in XML: " + Environment.NewLine + InputXml); }

            IEnumerable<XElement> togglesX;
            togglesX = InputXml.Elements("Enabled");
            if (togglesX != null)
            {
                foreach (XElement togglex in togglesX)
                {
                    if (!string.IsNullOrEmpty(togglex.Value))
                    {
                        this._toggleValMappings.Add(togglex.Value, true);
                    }
                }
            }
            togglesX = InputXml.Elements("Disabled");
            if (togglesX != null)
            {
                foreach (XElement togglex in togglesX)
                {
                    if (!string.IsNullOrEmpty(togglex.Value))
                    {
                        this._toggleValMappings.Add(togglex.Value, false);
                    }
                }
            }
        }

        public void OnToggleEvent(object o, RoutedEventArgs e)
        {
            string val;
            bool isenabled;
            val = (this._option.CurrentValue);
            this._toggleValMappings.TryGetValue(val, out isenabled);

            if (isenabled == true) { this.EnableGroup(); }
            else { this.DisableGroup(); }
        }

        private void DisableGroup()
        {
            if (this._hiddenMode == true) { this._group.IsHidden = true; }
            else { this._group.IsEnabled = false; }
        }

        private void EnableGroup()
        {
            this._group.IsHidden = false; 
            this._group.IsEnabled = true; 
        }
    }
}
