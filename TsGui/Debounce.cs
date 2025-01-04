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
using System;
using System.Windows.Threading;

namespace TsGui
{
    public class Debounce
    {
        private DispatcherTimer _timer = new DispatcherTimer();

        public Debounce(TimeSpan timeout, Action callback)
        {
            this._timer.Stop();
            this._timer.Interval = timeout;

            this._timer.Tick += (object o, EventArgs e) =>
            {
                this._timer.Stop();
                callback();
            };
        }

        /// <summary>
        /// Reset the timer and start it
        /// </summary>
        public void Start()
        {
            this._timer.Stop();
            this._timer.Start();
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            this._timer.Stop();
        }
    }
}
