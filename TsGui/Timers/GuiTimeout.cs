using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Xml.Linq;
using TsGui.Diagnostics.Logging;
using TsGui.Diagnostics;
using System.Windows;

namespace TsGui
{
    public class GuiTimeout
    {
        public delegate void TimeoutFunction();
        private static GuiTimeout _instance;
        private DateTime _afterstarttime;
        private DateTime _afterendtime;
        private bool _resetonactivity = false;
        private GuiTimeout() { }

        private TimeoutFunction _timeoutfunction;

        public bool IgnoreValidation { get; private set; } = false;
        public DispatcherTimer AfterTimer { get; private set; } = new DispatcherTimer();
        public DispatcherTimer AtTimer { get; private set; } = new DispatcherTimer();
        public TimeSpan TimeLeft { 
            get 
            { 
                if (this._afterendtime < this.TimeoutDateTime)
                {
                    if (this.AfterTimer.IsEnabled) { return (_afterendtime - DateTime.Now); }
                    else { return new TimeSpan(0, 0, 0, 0); }
                }
                else { return (this.TimeoutDateTime - DateTime.Now); }
            } 
        }

        public string AtString { get { return this.TimeoutDateTime.ToString("dd/MM/yyyy HH:mm:ss"); } }

        public TimeSpan TimeoutElapsed { get; private set; }
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
            this.IgnoreValidation = XmlHandler.GetBoolFromXElement(SourceXml, "IgnoreValidation", this.IgnoreValidation);
            this._resetonactivity = XmlHandler.GetBoolFromXElement(SourceXml, "ResetOnActivity", this.IgnoreValidation);

            XElement after = SourceXml.Element("After");
            if (after != null)
            {
                int ms = XmlHandler.GetIntFromXElement(after, "Milliseconds", 0);
                int sec = XmlHandler.GetIntFromXElement(after, "Seconds", 0);
                int minutes = XmlHandler.GetIntFromXElement(after, "Minutes", 0);
                int hours = XmlHandler.GetIntFromXElement(after, "Hours", 0);
                int days = XmlHandler.GetIntFromXElement(after, "Days", 0);
                this.TimeoutElapsed = new TimeSpan(days, hours, minutes, sec, ms); // ms + (sec * 1000) + (minutes * 60000) + (hours * 3600000) + (days * ;
            }
            
            string at = XmlHandler.GetStringFromXElement(SourceXml, "At", null);
            if (string.IsNullOrWhiteSpace(at) == false)
            {
                try
                {
                    this.TimeoutDateTime = DateTime.ParseExact(at, "yyyy-MM-dd HH:mm:ss",null);
                }
                catch (Exception e)
                {
                    throw new TsGuiKnownException("DateTime entered in <Timeount><At> is not valid", e.Message);
                }
            }
        }

        public void Start (TimeoutFunction timeoutfunction)
        {
            this._timeoutfunction = timeoutfunction;

            if (this.TimeoutElapsed != null)
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
                LoggerFacade.Info("Timeout will occur in " + this.TimeoutElapsed.TotalMilliseconds + " milliseconds");
            }
            
            if (this.TimeoutDateTime != DateTime.MaxValue)
            {
                //first check if the timeout datetime has already passed. 
                if (this.TimeoutDateTime < DateTime.Now)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.OnTimeoutReached()));
                    return;
                }
                TimeSpan timeout = this.TimeoutDateTime - DateTime.Now;
                if (timeout.TotalMilliseconds > 0)
                {
                    this.AtTimer.Interval = timeout;
                    this.AtTimer.Tick += this.OnTimeoutReached;
                    this.AtTimer.Start();
                    LoggerFacade.Info("Timeout will occur at " + this.TimeoutDateTime);
                }
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
            LoggerFacade.Info("Timeout reached");
            this.AfterTimer.Stop();
            this.AtTimer.Stop();
            this._timeoutfunction();
        }

        private void OnTimeoutReached(object sender, EventArgs e)
        {
            this.OnTimeoutReached();
        }
    }
}
