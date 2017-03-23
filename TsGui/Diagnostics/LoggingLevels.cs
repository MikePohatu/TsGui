//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// LoggingLevel.cs

using System;

namespace TsGui.Diagnostics
{
    public static class LoggingLevels
    {
        public const int Debug = -1;
        public const int Information = 0;
        public const int Warning = 1;
        public const int Error = 2;


        /// <summary>
        /// Convert a integer state value into the string value of that state
        /// </summary>
        /// <param name="StateValue"></param>
        /// <returns></returns>
        public static string ToString(int StateValue)
        {
            switch (StateValue)
            {
                case 0:
                    return "Information";
                case 1:
                    return "Warning";
                case 2:
                    return "Error";
                case 3:
                    return "Debug";
                default:
                    throw new ArgumentException(StateValue + " is not a valid state value");
            }
        }
    }
}