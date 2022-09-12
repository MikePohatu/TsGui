#region license
// Copyright (c) 2021 20Road Limited
//
// This file is part of DevChecker.
//
// DevChecker is free software: you can redistribute it and/or modify
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

namespace TsGui.Scripts
{
    public class ScriptSettings
    {
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
        /// Log the contents of the script file/command
        /// </summary>
        public bool LogScriptContent { get; set; } = false;

        /// <summary>
        /// Create a new CustomActionSettings with the specified json text
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static ScriptSettings Create(string json)
        {
            try
            {
                ScriptSettings settings = JsonConvert.DeserializeObject<ScriptSettings>(json);
                if (settings == null) { settings = new ScriptSettings(); }
                return settings;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error loading ScriptSettings");
                return null;
            }
        }
    }
}
