using Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TsGui.Scripts
{
    public class ScriptFactory
    {
        public static BaseScript CreateScript(XElement inputxml)
        {
            if (inputxml == null) { return null; }

            string type = XmlHandler.GetStringFromXAttribute(inputxml, "Type", null);

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new KnownException($"Type attribute not set on script\n{inputxml}", null);
            }

            switch (type.ToLower())
            {
                case "powershell":
                    return new PoshScript(inputxml);
                case "batch":
                    return new BatchScript(inputxml);
                default:
                    throw new KnownException("Invalid type specified in script", inputxml.ToString());
            }
        }
    }
}
