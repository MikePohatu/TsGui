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
using NLog;
using NLog.Config;
using System.Collections.Generic;
using System.Linq;

namespace Core.Logging
{

    public static class LoggingHelpers
    {
        private static string _logtargetName = "UserUI";
        public static void AddLoggingHandler(NewLog handler)
        {
            if (handler == null) { return; }

            var receivers = NLog.LogManager.Configuration.AllTargets.Where(t => t is UserUITarget).Cast<UserUITarget>();
            foreach (UserUITarget receiver in receivers)
            { receiver.NewLogMessage += handler; }
        }

        public static void InitLogging(string targetName)
        {
            _logtargetName = targetName;
            LogManager.Setup().SetupExtensions(ext => ext.RegisterTarget<UserUITarget>(targetName));
        }

        public static void InitLogging()
        {

            LogManager.Setup().SetupExtensions(ext => ext.RegisterTarget<UserUITarget>(_logtargetName));
        }

        /// <summary>
        /// 0=Trace, 1=Debug, 2=Info, 3=Warn, 4=Error, 5=Fatal
        /// </summary>
        /// <param name="level"></param>
        /// <param name="target">NLog target name from config</param>
        public static void SetLoggingLevel(int level, string target)
        {
            LogLevel loglevel = ConvertToLogLevel(level);
            LoggingRule rule = LogManager.Configuration.FindRuleByName(target);
            rule?.SetLoggingLevels(loglevel, LogLevel.Fatal);
            LogManager.ReconfigExistingLoggers();
        }

        public static int GetLoggingLevel(string target)
        {
            LoggingRule rule = LogManager.Configuration.FindRuleByName(target);
            return ConvertToInt(rule.Levels[0]);
        }

        public static int ConvertToInt(LogLevel level)
        {
            if (level == LogLevel.Trace) { return 0; }
            if (level == LogLevel.Debug) { return 1; }
            if (level == LogLevel.Info) { return 2; }
            if (level == LogLevel.Warn) { return 3; }
            if (level == LogLevel.Error) { return 4; }
            return 5;
        }

        public static LogLevel ConvertToLogLevel(int level)
        {
            if (level == 0) { return LogLevel.Trace; }
            if (level == 1) { return LogLevel.Debug; }
            if (level == 2) { return LogLevel.Info; }
            if (level == 3) { return LogLevel.Warn; }
            if (level == 4) { return LogLevel.Error; }
            return LogLevel.Fatal;
        }


        public static IEnumerable<ILoggingReceiver> GetLoggingReceivers()
        {
            var targets = NLog.LogManager.Configuration.AllTargets;
            var receivers =  targets.Where(t => t is ILoggingReceiver).Cast<ILoggingReceiver>();
            return receivers;
        }
    }
}
