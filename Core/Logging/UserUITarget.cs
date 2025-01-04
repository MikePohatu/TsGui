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

// LoggingReceiver.cs - Receives logs from the logging framework for use in testingwindow

using System;

using NLog;
using NLog.Targets;
using NLog.Config;

namespace Core.Logging
{
    [Target("UserUITarget")]
    public class UserUITarget : TargetWithLayout, ILoggingReceiver
    {
        public event NewLog NewLogMessage;

        public string LastMessage { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            //skip notifications messages as they are written to the output window separately. They need other
            //styling applied beyond the separation between alert levels
            //if (logEvent.Message.StartsWith(Notifications.AlertPrefix)) { return; }

            string trimmedmessage;
            bool ishighlighted = Log.IsHighlighted(this.Layout.Render(logEvent), out trimmedmessage);
            this.LastMessage = trimmedmessage;
            this.NewLogMessage?.Invoke(this, new EventArgs());
        }
    }
}