﻿#region license
// Copyright (c) 2025 Mike Pohatu
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
        //ID is set by either LoadXml or Create via the SetID() method
        public string ID { get; private set; }
        private Group _group;
        private Dictionary<string, bool> _toggleValMappings = new Dictionary<string, bool>();
        private bool _hiddenMode = false;
        private bool _inverse = false;
        private IToggleControl _option;


        private Toggle(IToggleControl option)
        {
            this.Init(option);
        }

        public Toggle(IToggleControl option, XElement InputXml)
        {
            this.Init(option);
            this.LoadXml(InputXml);
        }

        private void Init(IToggleControl option)
        {
            this._option = option;
            ConfigData.Toggles.Add(option);
            this._option.ToggleEvent += this.OnToggleEvent;
        }
        
        private void SetGroup(Group group)
        {
            this._group = group;
            this.ID = $"var:{this._option.VariableName}_gid:{group?.ID}";
        }

        /// <summary>
        /// Create a toggle with TRUE and FALSE for enabled/disabled values
        /// </summary>
        /// <param name="GuiOption"></param>
        /// <param name="groupid"></param>
        /// <param name="hide"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static Toggle Create(IToggleControl option, string groupid, bool hide, bool invert)
        {
            var toggle = new Toggle(option);

            toggle.SetGroup(GroupLibrary.GetGroupFromID(groupid));
            toggle._hiddenMode = hide;
            toggle._inverse = invert;
            toggle._toggleValMappings.Add("TRUE", true);
            toggle._toggleValMappings.Add("FALSE", false);
            return toggle;
        }

        private void LoadXml(XElement InputXml)
        {
            this._hiddenMode = XmlHandler.GetBoolFromXml(InputXml, "Hide", this._hiddenMode);
            this._inverse = XmlHandler.GetBoolFromXml(InputXml, "Invert", false);
            string groupid = XmlHandler.GetStringFromXml(InputXml, "Group", null);

            if (string.IsNullOrWhiteSpace(groupid)) 
            { 
                throw new InvalidOperationException("No Group ID set in Toggle configured in XML: " + Environment.NewLine + InputXml); 
            }

            this.SetGroup(GroupLibrary.GetGroupFromID(groupid));

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
            Log.Trace($"Toggle Event received by toggle: " + this.ID );
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
                    if (newenabled == true) 
                    { this.DisableGroup(); }
                    else 
                    { this.EnableGroup(); }
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
