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
namespace CustomActions
{
    public static class OutputTypes
    {
        public static string Text { get; } = "Text";
        public static string List { get; } = "List";
        public static string None { get; } = "None";
        public static string Object { get; } = "Object";

        public static string GetType(string outputtype)
        {
            if (string.IsNullOrWhiteSpace(outputtype)) { return Text; }

            switch (outputtype.Trim().ToLower())
            {
                case "text":
                    return Text;
                case "list":
                    return List;
                case "object":
                    return Object;
                case "none":
                    return None;
                default:
                    Core.Logging.Log.Error("Invalid output type set: " + outputtype);
                    return Text;
            }
        }
    }
}
