using Core.Logging;
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using TsGui.View.Layout;

namespace TsGui.View.GuiOptions
{
    internal class TsLoggingOutput: GuiOptionBase, IGuiOption
    {
        private TsLoggingOutputUI _ui;
        private bool _verboseAllowed = false;

        public override string CurrentValue { get; } = null;
        public override IEnumerable<Variable> Variables { get; } = null;

        private Visibility _verbosevisibility = Visibility.Collapsed;
        public Visibility VerboseVisibility
        {
            get { return this._verbosevisibility; }
            set 
            {
                this._verbosevisibility = value; 
                this.OnPropertyChanged(this, "VerboseVisibility");
                this.OnPropertyChanged(this, "LogOutputHeight");
            }
        }

        private string _verboselabel = "Verbose";
        public string VerboseLabel
        {
            get { return this._verboselabel; }
            set { this._verboselabel = value; this.OnPropertyChanged(this, "VerboseLabel"); }
        }

        public bool Verbose
        {
            get { return LoggingHelpers.GetLoggingLevel("livedata") < 2; }
            set {
                if (value) { LoggingHelpers.SetLoggingLevel(0, "livedata"); }
                else { LoggingHelpers.SetLoggingLevel(2, "livedata"); }
                this.OnPropertyChanged(this, "Verbose"); 
            }
        }

        public double LogOutputHeight
        {
            get {
                
                if (this._verboseAllowed)
                {
                    var label = this._ui._verboseLabel;
                    var output = this._ui._logtextbox;
                    return this.Style.ControlStyle.Height - label.Height - this.Style.ControlStyle.Margin.Top - this.Style.ControlStyle.Margin.Bottom;
                }
                return this.Style.ControlStyle.Height - this.Style.ControlStyle.Margin.Top - this.Style.ControlStyle.Margin.Bottom;
            }
        }

        public TsLoggingOutput(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            this.Init();
            this.LoadXml(InputXml);
        }

        protected TsLoggingOutput(ParentLayoutElement Parent) : base(Parent)
        {
            this.Init();
        }

        private void Init()
        {
            this.UserControl.DataContext = this;
            this._ui = new TsLoggingOutputUI();
            this.Control = this._ui;
            this.Label = new TsLabelUI();
            this._loggingtextbox = this._ui._logtextbox;
            Director.Instance.ParentWindow.Closed += this.OnWindowClosed;
            //set defaults
            this.Style.ControlStyle.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.Style.RightCellWidth = this.Style.RightCellWidth + this.Style.LeftCellWidth;
            this.Style.LeftCellWidth = 0;
            this.Style.ControlStyle.Height = 200;


            this.SubscribeToLogs();
        }

        public override async Task UpdateLinkedValueAsync(Message message)
        {
            await Task.CompletedTask;
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this._verboseAllowed = XmlHandler.GetBoolFromXml(InputXml, "AllowVerbose", this._verboseAllowed);
            if (this._verboseAllowed) { this.VerboseVisibility = Visibility.Visible; }
            this.VerboseLabel = XmlHandler.GetStringFromXml(InputXml, "VerboseLabel", this.VerboseLabel);
        }

        #region Logging Bits
        protected TextBox _loggingtextbox;
        protected bool _pendinglogrefresh = false;

        public string Output { get { return EnvironmentController.OutputType.ToString(); } }
        private string _logs;
        public string Logs
        {
            get { return this._logs; }
            set { this._logs = value; this.OnPropertyChanged(this, "Logs"); }
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
        #endregion
    }
}
