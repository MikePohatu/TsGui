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

// LoggerFacade.cs - Facade class for the main logging framework 

using NLog;

namespace TsGui.Diagnostics.Logging
{
    public static class LoggerFacade
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Trace(string message)
        {
            _logger.Trace(message);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }
    }
}
