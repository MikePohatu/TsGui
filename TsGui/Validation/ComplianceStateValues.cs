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

// ComplianceStateValues.cs

using System;

namespace TsGui.Validation
{
    public static class ComplianceStateValues
    {
        public const int Inactive = -1;
        public const int OK = 0;
        public const int Warning = 1;
        public const int Error = 2;
        public const int Invalid = 3;

        /// <summary>
        /// Convert a integer state value into the string value of that state
        /// </summary>
        /// <param name="StateValue"></param>
        /// <returns></returns>
        public static string ToString(int StateValue)
        {
            switch (StateValue)
            {
                case -1:
                    return "Inactive";
                case 0:
                    return "OK";
                case 1:
                    return "Warning";
                case 2:
                    return "Error";
                case 3:
                    return "Invalid";
                default:
                    throw new ArgumentException(StateValue + " is not a valid state value");
            }
        }
    }
}