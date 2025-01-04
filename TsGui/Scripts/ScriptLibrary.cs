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
using System;
using System.Collections.Generic;
using Core.Logging;
using Core.Diagnostics;
using System.Xml.Linq;

namespace TsGui.Scripts
{
    public static class ScriptLibrary
    {
        private static Dictionary<string, BaseScript> _scripts = new Dictionary<string, BaseScript>();

        public static void LoadXml(XElement InputXml)
        {
            XElement scripts = InputXml.Element("Scripts");
            if (scripts != null)
            {
                foreach (XElement x in scripts.Elements("Script"))
                {
                    AddScript(ScriptFactory.CreateScript(x));
                }
            }           
        }

        public static BaseScript GetScript(string id)
        {
            BaseScript outscript;
            var scripts = _scripts;

            if (_scripts.TryGetValue(id, out outscript)==false)
            {
                Log.Warn("Unable to find script in library: " + id);
                return null;
            }
            else
            {
                return outscript;
            }
        }

        public static void AddScript(BaseScript script)
        {
            if (script.ID == null)
            {
                throw new KnownException($"Global scripts must include an ID: {script.Name}", String.Empty);
            }

            if (_scripts.ContainsKey(script.ID))
            {
                throw new KnownException("Script with that ID already exists in configuration: " + script.ID, String.Empty);
            }
            
            _scripts.Add(script.ID, script);
        }

        public static void Reset()
        {
            _scripts.Clear();
        }
    }
}
