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
        public const int Debug = 0;
        public const int Information = 1;
        public const int Warning = 2;
        public const int Error = 3;


        /// <summary>
        /// Convert a integer state value into the string value of that state
        /// </summary>
        /// <param name="StateValue"></param>
        /// <returns></returns>
        public static string ToString(int StateValue)
        {
            switch (StateValue)
            {
                case Debug:
                    return "Debug";
                case Information:
                    return "Information";
                case Warning:
                    return "Warning";
                case Error:
                    return "Error";
                default:
                    throw new ArgumentException(StateValue + " is not a valid state value");
            }
        }

        /// <summary>
        /// Convert a integer state value into the short string value of that state
        /// </summary>
        /// <param name="StateValue"></param>
        /// <returns></returns>
        public static string ToShortString(int StateValue)
        {
            switch (StateValue)
            {
                case Debug:
                    return "Dbg";
                case Information:
                    return "Info";
                case Warning:
                    return "Warn";
                case Error:
                    return "Err";
                default:
                    throw new ArgumentException(StateValue + " is not a valid state value");
            }
        }

        /// <summary>
        /// Convert a string logging level into the integer for that state
        /// </summary>
        /// <param name="StateValue"></param>
        /// <returns></returns>
        public static int ToLoggingLevel(string Input)
        {
            switch (Input.ToLower())
            {
                case "debug":
                    return Debug;
                case "dbg":
                    return Debug;
                case "information":
                    return Information;
                case "info":
                    return Information;
                case "warning":
                    return Warning;
                case "warn":
                    return Warning;
                case "error":
                    return Error;
                case "err":
                    return Error;
                default:
                    throw new ArgumentException(Input + " is not a valid logging level");
            }
        }
    }
}