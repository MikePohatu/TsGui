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
using System.Xml.Linq;
using Core.Logging;
using Core.Diagnostics;
using System.Windows;

namespace TsGui
{
    public class GuiTimeout
    {
        public delegate void TimeoutFunction();
        private static GuiTimeout _instance;
        private DateTime _afterstarttime = DateTime.MaxValue;
        private DateTime _afterendtime = DateTime.MaxValue;
        private bool _resetonactivity = false;
        private GuiTimeout() { }

        private TimeoutFunction _timeoutfunction;

        public bool IgnoreValidation { get; private set; } = false;
        public bool CancelOnTimeout { get; private set; } = false;
        public DispatcherTimer AfterTimer { get; private set; } = new DispatcherTimer();
        public DispatcherTimer AtTimer { get; private set; } = new DispatcherTimer();
        public TimeSpan TimeLeft { 
            get 
            { 
                if (this.AfterTimer.IsEnabled == false && this.AtTimer.IsEnabled == false) { return new TimeSpan(0, 0, 0, 0); }
                if (this._afterendtime < this.TimeoutDateTime)
                {
                    if (this.AfterTimer.IsEnabled) { return (_afterendtime - DateTime.Now); }
                    else { return new TimeSpan(0, 0, 0, 0); }
                }
                else { return (this.TimeoutDateTime - DateTime.Now); }
            } 
        }

        public string AtString { get { return this.TimeoutDateTime.ToString("dd/MM/yyyy HH:mm:ss"); } }

        public TimeSpan TimeoutElapsed { get; private set; } = TimeSpan.MinValue;
        public DateTime TimeoutDateTime { get; private set; } = DateTime.MaxValue;

        /// <summary>
        /// Returns the singleton GuiTimeout. Will return null if init function has not been called with valid xml
        /// </summary>
        public static GuiTimeout Instance
        {
            get
            {
                return GuiTimeout._instance;
            }
        }

        public static void Init(XElement inputXml)
        {
            if (inputXml != null)
            {
                if (GuiTimeout._instance == null)
                {
                    GuiTimeout._instance = new GuiTimeout();
                }
                GuiTimeout._instance.LoadXml(inputXml);
            }
        }

        public void LoadXml(XElement SourceXml)
        {
            this.IgnoreValidation = XmlHandler.GetBoolFromXml(SourceXml, "IgnoreValidation", this.IgnoreValidation);
            this.CancelOnTimeout = XmlHandler.GetBoolFromXml(SourceXml, "CancelOnTimeout", this.IgnoreValidation);
            this._resetonactivity = XmlHandler.GetBoolFromXml(SourceXml, "ResetOnActivity", this.IgnoreValidation);

            XElement after = SourceXml.Element("After");
            if (after != null)
            {
                int ms = XmlHandler.GetIntFromXml(after, "Milliseconds", 0);
                int sec = XmlHandler.GetIntFromXml(after, "Seconds", 0);
                int minutes = XmlHandler.GetIntFromXml(after, "Minutes", 0);
                int hours = XmlHandler.GetIntFromXml(after, "Hours", 0);
                int days = XmlHandler.GetIntFromXml(after, "Days", 0);
                this.TimeoutElapsed = new TimeSpan(days, hours, minutes, sec, ms); // ms + (sec * 1000) + (minutes * 60000) + (hours * 3600000) + (days * ;
            }
            
            string at = XmlHandler.GetStringFromXml(SourceXml, "At", null);
            if (string.IsNullOrWhiteSpace(at) == false)
            {
                try
                {
                    this.TimeoutDateTime = DateTime.ParseExact(at, "yyyy-MM-dd HH:mm:ss",null);
                }
                catch (Exception e)
                {
                    throw new KnownException("DateTime entered in <Timeount><At> is not valid", e.Message);
                }
            }
        }

        public void Start (TimeoutFunction timeoutfunction)
        {
            this._timeoutfunction = timeoutfunction;

            if (this.TimeoutElapsed != TimeSpan.MinValue)
            {
                if (this._resetonactivity)
                {
                    Director.Instance.ParentWindow.MouseDown += this.ResetElapsed;
                    Director.Instance.ParentWindow.KeyDown += this.ResetElapsed;
                }
                this.AfterTimer.Interval = TimeoutElapsed;
                this.AfterTimer.Tick += this.OnTimeoutReached;
                this._afterstarttime = DateTime.Now;
                this._afterendtime = this._afterstarttime + this.TimeoutElapsed;
                this.AfterTimer.Start();
                Log.Info("Timeout will occur in " + this.TimeoutElapsed.TotalMilliseconds + " milliseconds");
            }
            
            if (this.TimeoutDateTime != DateTime.MaxValue)
            {
                //first check if the timeout datetime has already passed. 
                if (this.TimeoutDateTime < DateTime.Now)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.OnTimeoutReached()));
                    return;
                }
                this.ResetAtTimerBlock(this, new EventArgs());
            }
        }

        private void ResetAtTimerBlock(object sender, EventArgs e)
        {
            this.AtTimer?.Stop();
            this.AtTimer = new DispatcherTimer();
            TimeSpan timeout = this.TimeoutDateTime - DateTime.Now;
            if (timeout.TotalMinutes > 10)
            {
                this.AtTimer.Interval = new TimeSpan(0, 5, 0);
                this.AtTimer.Tick += this.ResetAtTimerBlock;
                this.AtTimer.Start();
                Log.Info("Timeout will occur at " + this.TimeoutDateTime);
            }
            else if (timeout.TotalMilliseconds > 0)
            {
                this.AtTimer.Interval = timeout;
                this.AtTimer.Tick += this.OnTimeoutReached;
                this.AtTimer.Start();
                Log.Info("Timeout will occur at " + this.TimeoutDateTime);
            }
        }

        private void ResetElapsed(object sender, EventArgs e)
        {
            this.AfterTimer.Stop();
            this._afterstarttime = DateTime.Now;
            this._afterendtime = this._afterstarttime + this.TimeoutElapsed;
            this.AfterTimer.Start();
        }

        private void OnTimeoutReached()
        {
            Log.Info("Timeout reached");
            this.AfterTimer.Stop();
            this.AtTimer.Stop();
            if (this._resetonactivity)
            {
                Director.Instance.ParentWindow.MouseDown -= this.ResetElapsed;
                Director.Instance.ParentWindow.KeyDown -= this.ResetElapsed;
            }
            this._timeoutfunction();
        }

        private void OnTimeoutReached(object sender, EventArgs e)
        {
            this.OnTimeoutReached();
        }
    }
}
