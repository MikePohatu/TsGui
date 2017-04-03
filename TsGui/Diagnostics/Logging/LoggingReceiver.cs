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

// LoggingReceiver.cs - Receives logs from the logging framework for use in testingwindow

using System;

using NLog;
using NLog.Targets;

namespace TsGui.Diagnostics.Logging
{
    [Target("LiveDataWindow")]
    public class LoggingReceiver: TargetWithLayout
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