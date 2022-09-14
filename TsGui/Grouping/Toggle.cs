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

// Toggle.cs - class to detect changes to IToggleControl objects and apply the changes
// to the associated group.

using Core.Logging;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace TsGui.Grouping
{
    public class Toggle
    {
        private Group _group;
        private Dictionary<string, bool> _toggleValMappings = new Dictionary<string, bool>();
        private bool _hiddenMode = false;
        private bool _inverse = false;
        private IToggleControl _option;

        public Toggle(IToggleControl GuiOption, XElement InputXml)
        {
            this._option = GuiOption;
            this.LoadXml(InputXml);
            this._option.ToggleEvent += this.OnToggleEvent;
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
                    this._group = Director.Instance.GroupLibrary.GetGroupFromID(xa.Value);
                }
                else { throw new InvalidOperationException("Invalid Toggle configured in XML: " + InputXml); }
            }
            else { throw new InvalidOperationException("No Group ID set in Toggle configured in XML: " + Environment.NewLine + InputXml); }

            xa = InputXml.Attribute("Invert");
            if (xa != null)
            {
                this._inverse = Convert.ToBoolean(xa.Value);
            }

            IEnumerable<XElement> togglesX;
            togglesX = InputXml.Elements("Enabled");
            if (togglesX.Count() > 0)
            {
                foreach (XElement togglex in togglesX)
                {
                    if (!string.IsNullOrEmpty(togglex.Value))
                    {
                        this._toggleValMappings.Add(togglex.Value, true);
                    }
                }
            }
            else
            {
                this._toggleValMappings.Add("TRUE", true);
            }

            togglesX = InputXml.Elements("Disabled");
            if (togglesX.Count() > 0)
            {
                foreach (XElement togglex in togglesX)
                {
                    if (!string.IsNullOrEmpty(togglex.Value))
                    {
                        this._toggleValMappings.Add(togglex.Value, false);
                    }
                }
            }
            else
            {
                this._toggleValMappings.Add("FALSE", false);
            }
        }

        public void OnToggleEvent()
        {
            Log.Trace($"Toggle Event received by toggle | VariableName: {this._option.VariableName} | Group: {this._group?.ID}" );
            string val;
            bool newenabled;

            val = this._option.CurrentValue;
            if (val == null )
            {
                this.DisableGroup();
                return;
            }

            if (this._option.IsActive == false)
            {
                this.DisableGroup();
                return;
            }
            else
            {
                this._toggleValMappings.TryGetValue(val, out newenabled);
                if (!this._inverse)
                {
                    if (newenabled == true) { this.EnableGroup(); }
                    else { this.DisableGroup(); }
                }
                else
                {
                    if (newenabled == true) { this.DisableGroup(); }
                    else { this.EnableGroup(); }
                }
            }
        }

        private void DisableGroup()
        {
            if (this._hiddenMode == false)
            {
                this._group.State = GroupState.Disabled;
            }
            else
            {
                this._group.State = GroupState.Hidden;
            }
        }

        private void EnableGroup()
        {
            this._group.State = GroupState.Enabled;          
        }
    }
}
