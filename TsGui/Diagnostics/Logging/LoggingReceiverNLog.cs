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

// LoggingReceiver.cs - Receives logs from the logging framework for use in testingwindow

using System;

using NLog;
using NLog.Targets;
using NLog.Config;

namespace TsGui.Diagnostics.Logging
{
    [Target("LiveDataWindow")]
    public class LoggingReceiverNLog: TargetWithLayout, ILoggingReceiver
    {
        public event NewLog NewLogMessage;

        public string LastMessage { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            this.LastMessage = this.Layout.Render(logEvent);
            this.NewLogMessage?.Invoke(this,new EventArgs());
        }
    }
}