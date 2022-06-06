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

// Log.cs - Facade class for the main logging framework 

using NLog;
using System;

namespace Core.Logging
{
    public static class Log
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Info(int message)
        {
            _logger.Info(message);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Warn(Exception e, string message)
        {
            _logger.Warn(e, message);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Error(Exception e, string message)
        {
            _logger.Error(e, message);
        }

        public static void Trace(string message)
        {
            _logger.Trace(message);
        }

        public static void Trace(Exception e, string message)
        {
            _logger.Trace(e, message);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public static void Fatal(Exception e, string message)
        {
            _logger.Fatal(e, message);
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        public static void Debug(Exception e, string message)
        {
            _logger.Debug(e, message);
        }

        private static string _highlightprefix = "**";
        public static string Highlight(string message)
        {
            return _highlightprefix + message;
        }

        public static bool IsHighlighted(string message, out string unhighlighted)
        {
            if (message.StartsWith(_highlightprefix))
            {
                if (message.Length == _highlightprefix.Length) { unhighlighted = string.Empty; }
                else { unhighlighted = message.Substring(_highlightprefix.Length); }
                return true;
            }
            else
            {
                unhighlighted = message;
                return false;
            }
        }
    }
}
