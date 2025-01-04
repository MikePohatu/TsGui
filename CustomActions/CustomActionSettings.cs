#region license
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
using Core.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsHelpers;

namespace CustomActions
{
    public class CustomActionSettings
    {
        private string _outputtype = OutputTypes.Text;
        /// <summary>
        /// What the action will return. Valid values: Object, List, Log, None
        /// </summary>
        public string OutputType
        {
            get { return this._outputtype; }
            set
            {
                this._outputtype = OutputTypes.GetType(value);
            }
        }

        private string _displayElement = DisplayElements.Log;
        /// <summary>
        /// How the returned data will be displayed. Valid values: Tab, Modal, Log
        /// </summary>
        public string DisplayElement
        {
            get { return this._displayElement; }
            set
            {
                this._displayElement = DisplayElements.GetType(value);             
            }
        }

        /// <summary>
        /// The count of rows per column. Used for object view
        /// </summary>
        public int MaxRowsPerColumn { get; set; } = int.MaxValue;

        /// <summary>
        /// Log output, even if the display element is not Log
        /// </summary>
        public bool LogOutput { get; set; } = false;

        /// <summary>
        /// Override the name to display in the console
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The description to display in the console
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Run the action automatically on connection
        /// </summary>
        public bool RunOnConnect { get; set; } = false;

        /// <summary>
        /// Whether to run on the client. Otherwise will be run on local computer
        /// </summary>
        public bool RunOnClient { get; set; } = true;

        /// <summary>
        /// Requires connect before run. Set to true if RunOnClient is false and you're passing client name etc
        /// </summary>
        public bool RequiresServerConnect { get; set; } = true;

        /// <summary>
        /// Log the contents of the script file/command
        /// </summary>
        public bool LogScriptContent { get; set; } = false;

        /// <summary>
        /// Which properties to filter on in a table viewer
        /// </summary>
        public List<string> FilterProperties { get; set; } = new List<string>();

        /// <summary>
        /// Create a new CustomActionSettings with the specified json text
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static CustomActionSettings Create(string json)
        {
            try
            {
                CustomActionSettings settings = JsonConvert.DeserializeObject<CustomActionSettings>(json);
                return settings;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error loading ActionSettings");
                return null;
            }
        }
    }
}
