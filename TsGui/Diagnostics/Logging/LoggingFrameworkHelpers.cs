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

// LoggingFrameworkHelpers.cs - class to provide helper methods for the logging framework e.g. setup

using System.Linq;
using System.Collections.Generic;
using NLog.Config;

namespace TsGui.Diagnostics.Logging
{
    public static class LoggingFrameworkHelpers
    {
        public static void InitializeLogFramework()
        {
            ConfigurationItemFactory.Default.Targets.RegisterDefinition("LiveDataWindow", typeof(LoggingReceiverNLog));
        }

        public static IEnumerable<ILoggingReceiver> GetLoggingReceivers()
        {
            return NLog.LogManager.Configuration.AllTargets.Where(t => t is ILoggingReceiver).Cast<ILoggingReceiver>();
        }
    }
}
