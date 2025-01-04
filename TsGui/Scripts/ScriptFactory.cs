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
using Core.Diagnostics;
using System.Xml.Linq;

namespace TsGui.Scripts
{
    public class ScriptFactory
    {
        public static BaseScript CreateScript(XElement inputxml)
        {
            if (inputxml == null) { return null; }

            string type = XmlHandler.GetStringFromXml(inputxml, "Type", null);

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
