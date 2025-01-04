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

// 'Root' class from which to spin everything off. TsGui creates this
// class to do the actual work. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using System.ComponentModel;

using TsGui.View.Layout;
using TsGui.Options.NoUI;
using TsGui.Options;
using TsGui.Grouping;
using Core.Diagnostics;
using Core.Logging;
using TsGui.Authentication;
using TsGui.Validation;
using System.Windows.Input;
using System.Windows.Threading;
using TsGui.Config;
using System.Threading.Tasks;
using TsGui.Scripts;
using TsGui.View.GuiOptions;
using TsGui.Sets;

namespace TsGui
{
    public class Director: IDirector
    {
        public static IDirector Instance { get; private set; } = new Director();

        public event RoutedEventHandler PageLoaded;
        public event RoutedEventHandler WindowLoaded;
        public event EventHandler WindowMoving;
        public event EventHandler WindowMoved;
        public event RoutedEventHandler WindowMouseUp;
        public event EventHandler ConfigLoadFinished;
        public event EventHandler AppClosing;

        private Debounce _movetimer;

        private bool _finished = false;
        private bool _firstinitcomplete = false;

        //properties
        public bool StartupFinished { get; set; }
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }
        public bool ShowGridLines { get { return ConfigData.ProdMode ? false : TsGuiRootConfig.ShowGridLines; } }
        public bool UseTouchDefaults { get { return TsGuiRootConfig.UseTouchDefaults; } }

        /// <summary>
        /// The path that will be set on new IOptions by default. Can be overridden by each option
        /// </summary>
        public string DefaultPath { get; private set; }

        //constructors
        private Director() { }

        public async Task ReloadAsync()
        {
            Log.Info(Log.Highlight("Reload initiated"));

            //unsubscribe to closing event
            this.ParentWindow.Closing -= this.OnWindowClosing;
            this.ParentWindow.Close();

            #region Remove any event registrations
            if (PageLoaded != null)
            {
                foreach (Delegate d in PageLoaded.GetInvocationList())
                {
                    PageLoaded -= (RoutedEventHandler)d;
                }
            }

            if (WindowLoaded != null)
            {
                foreach (Delegate d in WindowLoaded.GetInvocationList())
                {
                    WindowLoaded -= (RoutedEventHandler)d;
                }
            }

            if (WindowMoving != null)
            {
                foreach (Delegate d in WindowMoving.GetInvocationList())
                {
                    WindowMoving -= (EventHandler)d;
                }
            }

            if (WindowMoved != null)
            {
                foreach (Delegate d in WindowMoved.GetInvocationList())
                {
                    WindowMoved -= (EventHandler)d;
                }
            }

            if (WindowMouseUp != null)
            {
                foreach (Delegate d in WindowMouseUp.GetInvocationList())
                {
                    WindowMouseUp -= (RoutedEventHandler)d;
                }
            }

            if (ConfigLoadFinished != null)
            {
                foreach (Delegate d in ConfigLoadFinished.GetInvocationList())
                {
                    ConfigLoadFinished -= (EventHandler)d;
                }
            }
            #endregion


            //remove testing keyup
            if (ConfigData.TestingWindow != null)
            {
                ConfigData.TestingWindow.Window.KeyUp -= this.OnTestingKeyUp;
            }
            if (ConfigData.ProdMode == false)
            {
                this.ParentWindow.KeyUp -= this.OnTestingKeyUp;
            }

            //Reinit
            await this.InitAsync();
        }

        /// <summary>
        /// Initial startup, initiate init code
        /// </summary>
        /// <returns></returns>
        public async Task StartupAsync()
        {
            Log.Trace("MainController initializing");

            try { await this.InitAsync(); }
            catch (KnownException e)
            {
                string msg = "Error message: " + e.CustomMessage + Environment.NewLine + e.Message;
                this.CloseWithError("Application Startup Exception", msg);
            }
            catch (Exception e)
            {
                string msg = "Error message: " + e.Message + Environment.NewLine + e.ToString();
                this.CloseWithError("Application Startup Exception", msg);
            }
        }

        /// <summary>
        /// Replace the default director with a new IDirector instance. This to pass in a scaffold IDirector for testing
        /// </summary>
        /// <param name="newdirector"></param>
        public static void OverrideInstance(IDirector newdirector)
        {
            Instance = newdirector;
        }

        /// <summary>
        /// Initialize TsGui
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync()
        {
            Log.Debug("*TsGui startup started");
            DateTime start = DateTime.Now;
            this.StartupFinished = false;

            this.ParentWindow = new MainWindow();
            App.Current.MainWindow = this.ParentWindow;

            //init the debounced move timer
            this._movetimer = new Debounce(new TimeSpan(0, 0, 0, 0, 500), () =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    this.ParentWindow.LocationChanged -= this.OnWindowMoved;
                    this.WindowMoved?.Invoke(this, new EventArgs());
                }));
            });

            ConfigData.Reset();

            this.ParentWindow.Loaded += this.OnWindowLoaded;

            //read the config file in. Don't process it yet
            XElement xconfig = await this.ReadConfigFileAsync();
            if (xconfig == null) { return; }

            //Now load the XML, catching errors
            try {
                TsGuiRootConfig.LoadXml(xconfig);
                Log.Info("Finished applying root configuration");

                //Init the envController so we know what we're writing to and attach the SCCM COM object if required
                ConfigData.ProdMode = EnvironmentController.Init();

                this.LoadXml(xconfig);
                Log.Info("Finished applying main config");
            }
            catch (KnownException e)
            {
                string msg = "Error loading config " + Environment.NewLine + e.CustomMessage + Environment.NewLine + e.Message;
                this.CloseWithError("Error loading config ", msg);
                return;
            }

            //populate hardware options if HardwareEval enabled
            await HardwareEvaluator.InitAsync(xconfig);

            //If prodmode is true and testmode is false, only show the testing window if the debug option
            //has been set
            if (Arguments.Instance.TestMode == false && ConfigData.ProdMode == true)
            {
                if (TsGuiRootConfig.Debug == true || Arguments.Instance.Debug) { ConfigData.AddTestingWindow(); }
            }
            //if prodmode isn't true, the envcontroller couldn't connect to sccm
            //prompt the user if they want to continue. exit if not. 
            else
            {
                if (this.InitTestMode() != true) 
                {
                    this.ShutDown();
                    return; 
                }
                if ((TsGuiRootConfig.Debug == true) || (TsGuiRootConfig.LiveData == true)) 
                { 
                    ConfigData.AddTestingWindow();
                    ConfigData.TestingWindow.Window.KeyUp += this.OnTestingKeyUp;
                }

                this.ParentWindow.KeyUp += this.OnTestingKeyUp;
            }

            //now send a ConfigLoadFinished event so things know they can finish setting themselves up e.g. OptionValueQuery
            this.ConfigLoadFinished?.Invoke(this, null);

            //now init all the options
            await OptionLibrary.InitialiseOptionsAsync();

            //init the groups
            GroupLibrary.Init();

            //subscribe to closing event
            this.ParentWindow.Closing += this.OnWindowClosing;

            if (ConfigData.Pages.Count > 0)
            {
                Log.Debug("Loading pages");
                this.CurrentPage = ConfigData.Pages.First();
                this.ParentWindow.DataContext = ConfigData.TsMainWindow;

                // Now show and close the ghost window to make sure WinPE honours the 
                // windowstartuplocation
                Log.Trace("Loading ghost window");
                GhostWindow ghost = new GhostWindow();
                ghost.Show();
                ghost.Close();

                this.UpdateWindow();
                this.ParentWindow.Visibility = Visibility.Visible;
                this.ParentWindow.WindowStartupLocation = ConfigData.TsMainWindow.WindowLocation.StartupLocation;
                this.StartupFinished = true;
                
                GuiTimeout.Instance?.Start(this.OnTimeoutReached);
                Log.Info($"Startup time {(DateTime.Now - start).Seconds} seconds");
                Log.Info($"*TsGui startup finished");
            }
            else 
            {
                //No pages, finish using only the NoUI options
                Log.Info("*No pages configured. Finishing TsGui");
                await this.FinishAsync();
            }

            //mark init complete. this won't change during reload
            this._firstinitcomplete = true;
        }

        public async void OnTestingKeyUp(object o, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
                { await this.ReloadAsync(); }
            }
        }

        //attempt to read the config.xml file, and display the right messages if it fails
        private async Task<XElement> ReadConfigFileAsync()
        {
            //code to be added to make sure config file exists
            try
            {
                string uri;
                if (string.IsNullOrWhiteSpace(Arguments.Instance.WebConfigUrl))
                {
                    uri = Arguments.Instance.ConfigFile; 
                }
                else
                {
                    uri = Arguments.Instance.WebConfigUrl;
                }
                return await ConfigBuilder.LoadConfigAsync(uri);
            }
            catch (System.IO.FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error reading config file", MessageBoxButton.OK, MessageBoxImage.Error);
                this.ParentWindow.Close();
                return null;
            }
            catch (KnownException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                //MessageBox.Show("Invalid config file: " + this._configpath + Environment.NewLine + Environment.NewLine + e.Message, "Error reading config file", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(e.Message, "Error reading config file", MessageBoxButton.OK, MessageBoxImage.Error);
                this.ParentWindow.Close();
                return null;
            }
        }

        private void LoadXml(XElement SourceXml)
        {
            
            XElement x;
            TsPage currPage = null;
            TsPage prevPage = null;

            IEnumerable<XElement> pagesXml;

            if (SourceXml != null)
            {
                StyleLibrary.LoadXml(SourceXml);
                AuthLibrary.LoadXml(SourceXml);
                ScriptLibrary.LoadXml(SourceXml);

                //start layout import
                ConfigData.TsMainWindow = new TsMainWindow(this.ParentWindow, SourceXml);

                ConfigData.Buttons.LoadXml(SourceXml.Element("Buttons"));

                PageDefaults pagedef = new PageDefaults();

                x = SourceXml.Element("Heading");
                if (x != null) { pagedef.PageHeader = new TsPageHeader(ConfigData.TsMainWindow, x); }
                else { pagedef.PageHeader = new TsPageHeader(); }

                x = SourceXml.Element("LeftPane");
                if (x != null) { pagedef.LeftPane = new TsPane(x); }
                else { pagedef.LeftPane = new TsPane(); }

                x = SourceXml.Element("RightPane");
                if (x != null) { pagedef.RightPane = new TsPane(x); }
                else { pagedef.RightPane = new TsPane(); }

                pagedef.Buttons = ConfigData.Buttons;
                pagedef.MainWindow = ConfigData.TsMainWindow;


                ConfigData.TsMainWindow.LoadXml(SourceXml);
                GuiTimeout.Init(SourceXml.Element("Timeout"));

                //now read in the options and add to a dictionary for later use
                pagesXml = SourceXml.Elements("Page");
                if (pagesXml != null)
                {
                    //Debug.WriteLine("pagesXml not null");
                    foreach (XElement xPage in pagesXml)
                    {
                        #region
                        //Debug.WriteLine("creating new page");
                        if (currPage != null)
                        {
                            //record the last page as the prevPage
                            prevPage = currPage;
                            currPage = new TsPage(ConfigData.TsMainWindow, xPage, pagedef);                                                     
                        }
                        else
                        {
                            currPage = new TsPage(ConfigData.TsMainWindow, xPage,pagedef);
                            currPage.IsFirst = true;
                        }

                        //create the new page and assign the next page/prev page links
                        currPage.PreviousPage = prevPage;
                        if (prevPage != null) { prevPage.NextPage = currPage; }

                        ConfigData.Pages.Add(currPage);
                        currPage.Page.Loaded += this.OnPageLoaded;
                        #endregion
                    }

                    //currPage.IsLast = true;
                }

                x = SourceXml.Element("NoUI");
                if (x != null)
                {
                    ConfigData.NouiContainer = new NoUIContainer(x);
                }

                x = SourceXml.Element("Sets");
                if (x != null)
                {
                    SetLibrary.LoadXml(x);
                }
            }
        }

        //add options from sub classes to the main library. used to generate the final list of 
        //tsvariables
        public void AddOptionToLibary(IOption Option)
        {
            OptionLibrary.Add(Option);
        }

        //move to the next page and update the next/prev/finish buttons
        public void MoveNext()
        {
            this.CurrentPage = this.CurrentPage.NextActivePage;
            this.CurrentPage.Update();         
            this.UpdateWindow();
        }

        //move to the previous page and update the next/prev/finish buttons
        public void MovePrevious()
        {
            this.CurrentPage = this.CurrentPage.PreviousActivePage;
            this.CurrentPage.Update();
            this.UpdateWindow();
        }

        //Navigate to the current page, and update the datacontext of the window
        private void UpdateWindow()
        {
            Log.Trace("UpdateWindow called");
            this.ParentWindow.ContentArea.Navigate(this.CurrentPage.Page);
            this.ParentWindow.ContentArea.DataContext = this.CurrentPage;
            this.CurrentPage.Update();
        }

        public async void OnTimeoutReached()
        {
            if (GuiTimeout.Instance.IgnoreValidation == true)
            {
                foreach (IValidationGuiOption valop in OptionLibrary.ValidationOptions)
                {
                    //validation needs disabling because LostFocus is a validation event
                    if (valop.ValidationHandler != null) { valop.ValidationHandler.Enabled = false; }
                    valop.ClearToolTips();
                }

                if (GuiTimeout.Instance.CancelOnTimeout == false) 
                { await this.FinishAsync(); }
                else { this.Cancel(); }
            }
            else
            {
                if (ResultValidator.AllOptionsValid(OptionLibrary.ValidationOptions))
                {
                    if (GuiTimeout.Instance.CancelOnTimeout == false) 
                    { 
                        await this.FinishAsync(); 
                    }
                    else { this.Cancel(); }
                }
            }
        }

        //finish and create the TS Variables
        public async Task  FinishAsync()
        {
            foreach (IOption option in OptionLibrary.Options)
            {
                //first check for null option variables e.g. for headings
                if (option.Variables != null)
                {
                    //now check if the option is active or not and variables created as required
                    if (option.IsActive == true)
                    {
                        foreach (Variable variable in option.Variables)
                        {
                            EnvironmentController.AddVariable(variable);
                        }
                    }
                    else
                    { 
                        EnvironmentController.AddVariable(new Variable(option.VariableName, option.InactiveValue, option.Path)); 
                    }
                    
                }
            }


            List<Variable> setvars = await SetLibrary.ProcessAllAsync();
            foreach (Variable variable in setvars)
            {
                EnvironmentController.AddVariable(variable);
            }

            this._finished = true;
            this.ParentWindow.Close();
        }

        public void Cancel()
        {
            this.ParentWindow.Close();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (this._finished) { EnvironmentController.AddVariable(new Variable("TsGui_Cancel", "FALSE", null)); }
            else { EnvironmentController.AddVariable(new Variable("TsGui_Cancel", "TRUE", null)); }
            EnvironmentController.Release();
            this.ShutDown();

        }

        public void CloseWithError(string Title, string Message)
        {
            Log.Fatal("TsGui closing due to error: " + Title);
            Log.Fatal("Error message: " + Message);
            MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            this.ParentWindow.Closing -= this.OnWindowClosing;
            this.ParentWindow.Close();
            this.ShutDown();
        }

        private void ShutDown()
        {
            AppClosing?.Invoke(this, new EventArgs());
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Method to handle when content has finished rendering on the page
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnPageLoaded(object o, RoutedEventArgs e)
        {
            this.PageLoaded?.Invoke(o,e);
        }

        /// <summary>
        /// Method to handle when content has finished rendering on the window
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnWindowLoaded(object o, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => {
                this.ParentWindow.MouseLeftButtonUp += this.OnWindowMouseUp;
                this.ParentWindow.TouchUp += this.OnWindowMouseUp;
                this.ParentWindow.LocationChanged += this.OnWindowMoving;
                this.WindowLoaded?.Invoke(o, e);
            }));            
        }

        /// <summary>
        /// Method to handle when TsGui window is moving
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnWindowMoving(object o, EventArgs e)
        {
            this.WindowMoving?.Invoke(this, new EventArgs());
            this.ParentWindow.LocationChanged += this.OnWindowMoved;
        }

        /// <summary>
        /// Method to handle when TsGui window is moved
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnWindowMoved(object o, EventArgs e)
        {
            this._movetimer.Start();
        }

        /// <summary>
        /// Method to handle when TsGui window left mouse is released
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnWindowMouseUp(object o, RoutedEventArgs e)
        {
            this.WindowMouseUp?.Invoke(o, e);
        }

        private bool InitTestMode()
        {
            //if first init has already run, skip this
            if (this._firstinitcomplete) { return true; }

            string msg = "Could not connect to SCCM task sequence agent." + Environment.NewLine +
                    Environment.NewLine +
                    "This is because:" + Environment.NewLine +
                    "a. The tool hasn't been run from a task sequence" + Environment.NewLine +
                    "b. There was an error connecting to the SCCM client" + Environment.NewLine +
                    Environment.NewLine +
                    "Do you wish to run in test mode?" + Environment.NewLine +
                    Environment.NewLine +
                    "Click Yes to run in test mode, click No to close Task Sequence Gui.";
            string title = "Warning";
            MessageBoxResult result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            else { return false; }
        }
    }
}
