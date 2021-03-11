#region license
// Copyright (c) 2020 Mike Pohatu
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
using TsGui.Events;
using TsGui.Grouping;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using TsGui.Linking;
using TsGui.Authentication;
using TsGui.Validation;
using System.Windows.Input;
using System.Windows.Threading;

namespace TsGui
{
    public class Director: IDirector
    {
        private static IDirector _instance;

        public static IDirector Instance { get
            {
                if (Director._instance == null) { Director._instance = new Director(); }
                return Director._instance;
            } 
        }
        public event TsGuiWindowEventHandler WindowLoaded;
        public event TsGuiWindowMovingEventHandler WindowMoving;
        public event TsGuiWindowEventHandler WindowMouseUp;
        public event ConfigLoadFinishedEventHandler ConfigLoadFinished;

        private Debounce _movetimer;

        private Arguments _args;
        private bool _prodmode = false;
        private bool _finished = false;
        private TsButtons _buttons;
        private List<TsPage> _pages;
        private EnvironmentController _envController;
        private GroupLibrary _grouplibrary;
        private List<IToggleControl> _toggles;
        private OptionLibrary _optionlibrary;
        private AuthLibrary _authlibrary;
        private HardwareEvaluator _hardwareevaluator;
        private NoUIContainer _nouicontainer;
        private TestingWindow _testingwindow;
        private bool _livedata = false;
        private bool _debug = false;

        //properties
        public AuthLibrary AuthLibrary { get { return this._authlibrary; } }
        public GroupLibrary GroupLibrary { get { return this._grouplibrary; } }
        public TsMainWindow TsMainWindow { get; set; }
        public OptionLibrary OptionLibrary { get { return this._optionlibrary; } }
        public EnvironmentController EnvironmentController { get { return this._envController; } }
        public bool StartupFinished { get; set; }
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }
        public bool ShowGridLines { get; private set; }
        public bool UseTouchDefaults { get; private set; }

        /// <summary>
        /// The path that will be set on new IOptions by default. Can be overridden by each option
        /// </summary>
        public string DefaultPath { get; private set; }

        //constructors
        private Director()
        {
            Director._instance = this;

            //init the debounced move timer
            this._movetimer = new Debounce(new TimeSpan(0,0,0,0,500), () =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    this.WindowMoving?.Invoke(this, new EventArgs());
                }));
            });
        }

        //Wrap a generic exception handler to get some useful information in the event of a 
        //crash. 
        public void Init(MainWindow ParentWindow, Arguments Arguments)
        {
            LoggerFacade.Trace("MainController initializing");
            this._envController = new EnvironmentController();
            this._pages = new List<TsPage>();
            this._grouplibrary = new GroupLibrary();
            this._toggles = new List<IToggleControl>();
            this._optionlibrary = new OptionLibrary();
            this._authlibrary = new AuthLibrary();
            this._args = Arguments;
            this.ParentWindow = ParentWindow;
            this.ParentWindow.MouseLeftButtonUp += this.OnWindowMouseUp;
            this.ParentWindow.LocationChanged += this.OnWindowMoving;

            try { this.Startup(); }
            catch (TsGuiKnownException exc)
            {
                string msg = "Error message: " + exc.CustomMessage;
                this.CloseWithError("Application Startup Exception", msg);
                return;
            }
            catch (Exception exc)
            {
                string msg = "Error message: " + exc.Message + Environment.NewLine + exc.ToString();
                this.CloseWithError("Application Startup Exception", msg);
                return;
            }
        }

        /// <summary>
        /// Replace the default director with a new IDirector instance. This to pass in a scaffold IDirector for testing
        /// </summary>
        /// <param name="newdirector"></param>
        public static void OverrideInstance(IDirector newdirector)
        {
            Director._instance = newdirector;
        }

        public void CloseWithError(string Title, string Message)
        {
            LoggerFacade.Fatal("TsGui closing due to error: " + Title);
            LoggerFacade.Fatal("Error message: " + Message);
            MessageBox.Show(Message,Title, MessageBoxButton.OK, MessageBoxImage.Error);
            this.ParentWindow.Closing -= this.OnWindowClosing;
            this.ParentWindow.Close();
        }

        private void Startup()
        {
            LoggerFacade.Debug("*TsGui startup started");
            this.StartupFinished = false;
            

            
            XElement xconfig = this.ReadConfigFile();
            if (xconfig == null) { return; }

            //this.LoadXml(x);
            try { this.LoadXml(xconfig); }
            catch (TsGuiKnownException e)
            {
                string msg = "Error loading config file" + Environment.NewLine + e.CustomMessage + Environment.NewLine + e.Message;
                this.CloseWithError("Error loading config file", msg);
                return;
            }

            this._prodmode = this._envController.Init();
            this.PopulateHwOptions();

            //if prodmode isn't true, the envcontroller couldn't connect to sccm
            //prompt the user if they want to continue. exit if not. 
            if (this._args.TestMode == false && this._prodmode == true)
            {
                if (this._debug == true) { this._testingwindow = new TestingWindow(this); } 
            }
            else
            {
                if (this.PromptTestMode() != true) { this.Cancel(); return; }
                if ((this._debug == true) || (this._livedata == true)) { this._testingwindow = new TestingWindow(this); }
            }

            //now init everything
            this._optionlibrary.InitialiseOptions();

            //subscribe to closing event
            this.ParentWindow.Closing += this.OnWindowClosing;

            if (this._pages.Count > 0)
            {
                
                LoggerFacade.Debug("Loading pages");
                this.CurrentPage = this._pages.First();
                //update group settings to all controls
                foreach (IToggleControl t in this._toggles)
                { t.InitialiseToggle(); }

                this.ParentWindow.DataContext = this.TsMainWindow;

                // Now show and close the ghost window to make sure WinPE honours the 
                // windowstartuplocation
                LoggerFacade.Trace("Loading ghost window");
                GhostWindow ghost = new GhostWindow();
                ghost.Show();
                ghost.Close();

                this.UpdateWindow();
                this.ParentWindow.Visibility = Visibility.Visible;
                this.ParentWindow.WindowStartupLocation = this.TsMainWindow.WindowLocation.StartupLocation;
                this.StartupFinished = true;
                
                GuiTimeout.Instance?.Start(this.OnTimeoutReached);
                LoggerFacade.Info("*TsGui startup finished");
            }
            else 
            {
                //No pages, finish using only the NoUI options
                LoggerFacade.Info("*No pages configured. Finishing TsGui");
                this.Finish();
            }
        }

        //attempt to read the config.xml file, and display the right messages if it fails
        private XElement ReadConfigFile()
        {
            XElement x;
            //code to be added to make sure config file exists
            try
            {
                if (string.IsNullOrWhiteSpace(this._args.WebConfigUrl))
                {
                    x = XmlHandler.Read(this._args.ConfigFile);
                }
                else
                {
                    x = XmlHandler.ReadWeb(this._args.WebConfigUrl);
                }
                return x;
            }
            catch (System.IO.FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error reading config file", MessageBoxButton.OK, MessageBoxImage.Error);
                this.ParentWindow.Close();
                return null;
            }
            catch (TsGuiKnownException e)
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
                
                this._debug = XmlHandler.GetBoolFromXAttribute(SourceXml, "Debug", this._debug);
                this._livedata = XmlHandler.GetBoolFromXAttribute(SourceXml, "LiveData", this._livedata);
                this.DefaultPath = XmlHandler.GetStringFromXElement(SourceXml, "DefaultPath", this.DefaultPath);
                string outputtype = XmlHandler.GetStringFromXAttribute(SourceXml, "Output", "Sccm");
                if (this._args.TestMode == false) { this._envController.SetOutputType(outputtype); }
                else { this._envController.SetOutputType("Test"); }
                

                //Set show grid lines after pages and columns have been created.
                x = SourceXml.Element("ShowGridLines");
                if ((x != null) && (this._prodmode == false))
                { this.ShowGridLines = true; }

                x = SourceXml.Element("UseTouchDefaults");
                if (x != null)
                { this.UseTouchDefaults = true; }

                //turn hardware eval on or off
                x = SourceXml.Element("HardwareEval");
                if (x != null)
                { this._hardwareevaluator = new HardwareEvaluator(x); }

                //start layout import
                this.TsMainWindow = new TsMainWindow(this.ParentWindow, SourceXml);

                this._buttons = new TsButtons();
                this._buttons.LoadXml(SourceXml.Element("Buttons"));

                PageDefaults pagedef = new PageDefaults();

                x = SourceXml.Element("Heading");
                if (x != null) { pagedef.PageHeader = new TsPageHeader(this.TsMainWindow, x); }
                else { pagedef.PageHeader = new TsPageHeader(); }

                x = SourceXml.Element("LeftPane");
                if (x != null) { pagedef.LeftPane = new TsPane(x); }
                else { pagedef.LeftPane = new TsPane(); }

                x = SourceXml.Element("RightPane");
                if (x != null) { pagedef.RightPane = new TsPane(x); }
                else { pagedef.RightPane = new TsPane(); }

                pagedef.Buttons = this._buttons;
                pagedef.MainWindow = this.TsMainWindow;


                this.TsMainWindow.LoadXml(SourceXml);
                GuiTimeout.Init(SourceXml.Element("Timeout"));

                foreach (XElement xauth in SourceXml.Elements("Authentication"))
                {
                    this._authlibrary.AddAuthenticator(AuthenticationFactory.GetAuthenticator(xauth));
                }

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
                            currPage = new TsPage(this.TsMainWindow, xPage, pagedef);                                                     
                        }
                        else
                        {
                            currPage = new TsPage(this.TsMainWindow, xPage,pagedef);
                            currPage.IsFirst = true;
                        }

                        //create the new page and assign the next page/prev page links
                        currPage.PreviousPage = prevPage;
                        if (prevPage != null) { prevPage.NextPage = currPage; }

                        this._pages.Add(currPage);
                        currPage.Page.Loaded += this.OnWindowLoaded;
                        #endregion
                    }

                    //currPage.IsLast = true;
                }

                x = SourceXml.Element("NoUI");
                if (x != null)
                {
                    this._nouicontainer = new NoUIContainer(x);
                }
            }

            LoggerFacade.Info("Config load finished");
            this.ConfigLoadFinished?.Invoke(this, null);
        }

        //add options from sub classes to the main library. used to generate the final list of 
        //tsvariables
        public void AddOptionToLibary(IOption Option)
        {
            this._optionlibrary.Add(Option);
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
        
        public void AddToggleControl(IToggleControl ToogleControl)
        {
            this._toggles.Add(ToogleControl);
        }

        //Navigate to the current page, and update the datacontext of the window
        private void UpdateWindow()
        {
            LoggerFacade.Trace("UpdateWindow called");
            this.ParentWindow.ContentArea.Navigate(this.CurrentPage.Page);
            this.ParentWindow.ContentArea.DataContext = this.CurrentPage;
            this.CurrentPage.Update();
        }

        public void OnTimeoutReached()
        {
            if (GuiTimeout.Instance.IgnoreValidation == true)
            {
                foreach (IValidationGuiOption valop in this._optionlibrary.ValidationOptions)
                {
                    //validation needs disabling because LostFocus is a validation event
                    if (valop.ValidationHandler != null) { valop.ValidationHandler.Enabled = false; }
                    valop.ClearToolTips();
                }

                if (GuiTimeout.Instance.CancelOnTimeout == false) { this.Finish(); }
                else { this.Cancel(); }
            }
            else
            {
                if (ResultValidator.OptionsValid(this._optionlibrary.ValidationOptions))
                {
                    if (GuiTimeout.Instance.CancelOnTimeout == false) { this.Finish(); }
                    else { this.Cancel(); }
                }
            }
        }

        //finish and create the TS Variables
        public void Finish()
        {
            foreach (IOption option in this._optionlibrary.Options)
            {
                //first check for null option variables e.g. for headings
                if (option.Variable != null)
                {
                    //now check if the option is active or not and variables created as required
                    if (option.IsActive == true)
                    { this._envController.AddVariable(option.Variable); }
                    else
                    { this._envController.AddVariable(new Variable(option.VariableName, option.InactiveValue, option.Path)); }
                }
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
            if (_finished) { this._envController.AddVariable(new Variable("TsGui_Cancel", "FALSE", null)); }
            else { this._envController.AddVariable(new Variable("TsGui_Cancel", "TRUE", null)); }
            this._envController.Release();
        }

        /// <summary>
        /// Method to handle when content has finished rendering on the window
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnWindowLoaded(object o, RoutedEventArgs e)
        {
            this.WindowLoaded?.Invoke(o,e);
        }

        /// <summary>
        /// Method to handle when TsGui window has been moved
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnWindowMoving(object o, EventArgs e)
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

        private bool PromptTestMode()
        {
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

            if (result == MessageBoxResult.Yes) return true;
            else return false;
        }

        private void PopulateHwOptions()
        {
            if (this._hardwareevaluator != null)
            {
                LoggerFacade.Debug("Running hardware evaluator");
                foreach (Variable var in this._hardwareevaluator.GetTsVariables())
                {
                    NoUIOption newhwoption = new NoUIOption();
                    newhwoption.ImportFromTsVariable(var);
                    this._optionlibrary.Add(newhwoption);
                }
            }
        }
    }
}
