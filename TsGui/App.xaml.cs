using System.Windows;
using System;
using System.Windows.Threading;

using TsGui.Diagnostics.Logging;
using TsGui.Diagnostics;

namespace TsGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private IDirector _controller;

        public Arguments Arguments;
        MainWindow _mainwindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoggingFrameworkHelpers.InitializeLogFramework();

            try { this.Arguments = new Arguments(Environment.GetCommandLineArgs()); }
            catch (Exception exc)
            {
                string msg = exc.Message + Environment.NewLine;
                MessageBox.Show(msg, "Command Line Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown(1);
                return;
            }

            this.StartTsGui();
        }

        private void StartTsGui()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
         
            LoggerFacade.Info("*TsGui started - version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            this._mainwindow = new MainWindow(this.Arguments);
            Director.Instance.Init(this._mainwindow, this.Arguments);
            //this._controller = new Director(this._mainwindow, this.Arguments);
        }


        //Exception handler methods
        #region
        public void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            if (args.Exception is System.IO.FileNotFoundException)
            {
                LoggerFacade.Error("System.IO.FileNotFoundException logged. This is a known issue with DragDrop");
            }
            else
            {
                LoggerFacade.Fatal("OnDispatcherUnhandledException:" + args.Exception.ToString());
                LoggerFacade.Fatal("OnDispatcherUnhandledException:" + args.Exception.Message);
                this.HandleException(sender, args.Exception, args.Exception.StackTrace);
            }
            
        }

        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            LoggerFacade.Fatal("OnUnhandledException:" + e.Message);
            this.HandleException(sender, e, e.StackTrace);     
        }

        private void HandleException(object sender, Exception e, string AdditionalText)
        {
            if (e is TsGuiKnownException) { this.ShowErrorMessageAndClose((TsGuiKnownException)e); }
            else
            {
                string s = "Source: " + sender.ToString() + Environment.NewLine + Environment.NewLine + "Exception: " + e.Message + Environment.NewLine;
                if (!string.IsNullOrEmpty(AdditionalText)) { s = s + Environment.NewLine + AdditionalText; }
                this.ShowErrorMessageAndClose(s);
            }

        }
        #endregion


        private void ShowErrorMessageAndClose(TsGuiKnownException e)
        {
            string msg = e.CustomMessage + Environment.NewLine + Environment.NewLine + "Full error message:" + Environment.NewLine + e.Message;
            this.ShowErrorMessageAndClose(msg);
        }

        private void ShowErrorMessageAndClose(string Message)
        {
            LoggerFacade.Fatal("Closing TsGui. Error message: " + Message);
            string msg = Message;
            Director.Instance.CloseWithError("Application Runtime Exception", msg);
        }
    }
}
