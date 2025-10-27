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
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;

namespace TsGui
{
    /// <summary>
    /// Class to handle user pressing 'Defer' button, i.e. hide the window and try again later. 
    /// </summary>
    public static class DeferalHandler
    {        
        public enum DeferMode { Hide, Minimize }

        private static DispatcherTimer _deferTimer = new DispatcherTimer();
        public static int Timeout { get; private set; }
        public static DeferMode Mode { get; private set; } = DeferMode.Hide;


        public static void LoadXml(XElement InputXml)
        {
            Timeout = XmlHandler.GetIntFromXml(InputXml, "Timeout", Timeout);
            var x = InputXml.Element("Mode");
            if (x != null)
            {
                SetMode(x.Value);
            }

            var xa = InputXml.Attribute("Mode");
            if (xa != null)
            {
                SetMode(xa.Value);
            }
        }

        public static void Defer()
        {
            switch (Mode)
            {

                case DeferMode.Hide:
                    Director.Instance.ParentWindow.Hide();
                    break;
                case DeferMode.Minimize:
                    Director.Instance.ParentWindow.WindowState = WindowState.Minimized;
                    break;
                default:
                    break;
            }

            _deferTimer.Interval = new TimeSpan(0, 0, Timeout);
            _deferTimer.Tick += OnDeferalTimerFinished;
            Director.Instance.ParentWindow.Activated += OnRestoreClicked;
            _deferTimer.Start();
            Log.Info($"Deferred for {Timeout} seconds");
        }

        private static void SetMode(string mode)
        {
            if (string.IsNullOrEmpty(mode)) 
            { 
                Log.Warn("Empty deferal mode set. Skipping");
                return;
            }

            switch (mode.ToUpper())
            {
                case "HIDE":
                    Mode = DeferMode.Hide;
                    break;
                case "MINIMIZE":
                    Mode = DeferMode.Minimize;
                    break;
                default:
                    break;
            }
        }

        private static void OnDeferalTimerFinished(object sender, EventArgs e)
        {
            Log.Info($"Deferal timeout reached");
            _deferTimer.Tick -= OnDeferalTimerFinished;
            switch (Mode)
            {

                case DeferMode.Hide:
                    Director.Instance.ParentWindow.Show();
                    break;
                case DeferMode.Minimize:
                    Director.Instance.ParentWindow.WindowState = WindowState.Normal;
                    break;
                default:
                    break;
            }
        }

        private static void OnRestoreClicked(object sender, EventArgs e)
        {
            Log.Info($"Window restored, deferal cancelled");
            _deferTimer.Tick -= OnDeferalTimerFinished;
            Director.Instance.ParentWindow.Activated -= OnRestoreClicked;
        }
    }
}
