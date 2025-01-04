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
using Core;
using Core.Logging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TsGui.Diagnostics
{
    /// <summary>
    /// View model base class for anything receiving logging messages
    /// </summary>
    public abstract class LoggingUiViewModel: ViewModelBase
    {
        protected TextBox _loggingtextbox;
        protected bool _pendinglogrefresh;

        public string Output { get { return EnvironmentController.OutputType.ToString(); } }
        private string _logs;
        public string Logs
        {
            get { return this._logs; }
            set { this._logs = value; this.OnPropertyChanged(this, "Logs"); }
        }

        public LoggingUiViewModel()
        {
            this.SubscribeToLogs();
        }

        public void OnWindowClosed(object o, EventArgs e)
        {
            this.UnsubscribeFromLogs();
        }

        public void OnNewLogMessage(UserUITarget sender, EventArgs e)
        {
            this._logs = this._logs + sender.LastMessage + Environment.NewLine;

            if (this._pendinglogrefresh == false)
            {
                this._pendinglogrefresh = true;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.RefreshLogView()));
            }
        }

        public void RefreshLogView()
        {
            if (this._pendinglogrefresh == true)
            {
                this.OnPropertyChanged(this, "Logs");
                this._pendinglogrefresh = false;
                this._loggingtextbox.ScrollToEnd();
            }
        }

        public void OnLogClearClick(object sender, RoutedEventArgs e)
        { this.Logs = string.Empty; }

        protected void SubscribeToLogs()
        {
            foreach (ILoggingReceiver receiver in LoggingHelpers.GetLoggingReceivers())
            { receiver.NewLogMessage += this.OnNewLogMessage; }
        }

        protected void UnsubscribeFromLogs()
        {
            foreach (ILoggingReceiver receiver in LoggingHelpers.GetLoggingReceivers())
            { receiver.NewLogMessage -= this.OnNewLogMessage; }
        }
    }
}
