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
            foreach (XElement x in InputXml.Elements("Scrtip"))
            {
                AddScript(ScriptFactory.GetScript(x));
            }
        }

        public static BaseScript GetScript(string name)
        {
            BaseScript outscript;
            if (_scripts.TryGetValue(name, out outscript)==false)
            {
                Log.Warn("Unable to find script in library: " + name);
                return null;
            }
            else
            {
                return outscript;
            }
        }

        public static void AddScript(BaseScript script)
        {
            if (_scripts.ContainsKey(script.Name))
            {
                throw new KnownException("Script with that name already exists in configuration: " + script.Name, String.Empty);
            }
            else
            {
                _scripts.Add(script.Name, script);
            }
        }
    }
}
