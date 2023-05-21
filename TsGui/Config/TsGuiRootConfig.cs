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

using System.Xml.Linq;

namespace TsGui.Config
{
    public static class TsGuiRootConfig
    {
        /// <summary>
        /// Turn the GridLines in the UI on/off during testing (default = false)
        /// </summary>
        public static bool ShowGridLines { get; private set; } = false;

        /// <summary>
        /// Show the Live Data window during testing (default = true)
        /// </summary>
        public static bool LiveData { get; private set; } = false;

        /// <summary>
        /// Show the Live Data window during testing and production (default = false)
        /// </summary>
        public static bool Debug { get; private set; } = false;

        /// <summary>
        /// Run the Hardware Evaluator (default = true)
        /// </summary>
        public static bool HardwareEval { get; private set; } = true;

        /// <summary>
        /// Set touch defaults in UI, gives more space and height for controls (default = false)
        /// </summary>
        public static bool UseTouchDefaults { get; private set; } = false;

        /// <summary>
        /// Default path value for GuiOptions (default = null)
        /// </summary>
        public static string DefaultPath { get; private set; } = null;

        /// <summary>
        /// Purge inactive/disabled controls i.e. don't create values (default = false)
        /// </summary>
        /// <param name="inputxml"></param>
        public static bool PurgeInactive { get; private set; } = false;

        public static string OutputType { get; private set; } = "Sccm";

        public static void LoadXml(XElement inputxml)
        {
            Debug = XmlHandler.GetBoolFromXml(inputxml, "Debug", Debug);
            LiveData = XmlHandler.GetBoolFromXml(inputxml, "LiveData", LiveData);

            XElement x;
            //Set show grid lines after pages and columns have been created.
            x = inputxml.Element("ShowGridLines");
            if (x != null)
            { ShowGridLines = true; }

            x = inputxml.Element("UseTouchDefaults");
            if (x != null)
            { UseTouchDefaults = true; }

            //turn hardware eval on or off
            x = inputxml.Element("HardwareEval");
            if (x == null)
            { HardwareEval = false; }

            DefaultPath = XmlHandler.GetStringFromXml(inputxml, "DefaultPath", DefaultPath);
            PurgeInactive = XmlHandler.GetBoolFromXml(inputxml, "PurgeInactive", PurgeInactive);
            OutputType = XmlHandler.GetStringFromXml(inputxml, "Output", OutputType);

        }
    }
}
