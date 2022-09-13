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
            if (_scripts.ContainsKey(script.ID))
            {
                throw new KnownException("Script with that ID already exists in configuration: " + script.ID, String.Empty);
            }
            else
            {
                _scripts.Add(script.ID, script);
            }
        }
    }
}
