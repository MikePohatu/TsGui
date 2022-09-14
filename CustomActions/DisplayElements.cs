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

namespace CustomActions
{
    public static class DisplayElements
    {
        public static string None { get; } = "None";
        public static string Log { get; } = "Log";
        public static string Tab { get; } = "Tab";
        public static string Modal { get; } = "Modal";

        public static string GetType(string outputtype)
        {
            if (string.IsNullOrWhiteSpace(outputtype)) { return Log; }
            string val = outputtype.Trim().ToLower();

            switch (outputtype.Trim().ToLower())
            {
                case "tab":
                    return Tab;
                case "modal":
                    return Modal;
                case "log":
                    return Log;
                case "none":
                    return None;
                default:
                    Core.Logging.Log.Error("Invalid display element set");
                    return Log;
            }
        }
    }
}
