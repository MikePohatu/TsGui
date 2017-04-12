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

namespace TsGui
{
    public class Director: IDirector
    {
        public event TsGuiWindowEventHandler WindowLoaded;
        public event TsGuiWindowMovingEventHandler WindowMoving;
        public event TsGuiWindowEventHandler WindowMouseUp;
        public event ConfigLoadFinishedEventHandler ConfigLoadFinished;

        private string _configpath;
        private bool _prodmode = false;
        private bool _finished = false;
        private TsButtons _buttons = new TsButtons();
        private List<TsPage> _pages = new List<TsPage>();
        private EnvironmentController _envController;
        private LinkingLibrary _linkinglibrary = new LinkingLibrary();
        private GroupLibrary _grouplibrary = new GroupLibrary();
        private List<IToggleControl> _toggles = new List<IToggleControl>();
        private OptionLibrary _optionlibrary = new OptionLibrary();
        private HardwareEvaluator _chassischeck;
        private NoUIContainer _nouicontainer;
        private TestingWindow _testingwindow;
        private bool _livedata = false;
        private bool _debug = false;
        private bool _showtestwindow = false;

        //properties
        public LinkingLibrary LinkingLibrary { get { return this._linkinglibrary; } }
        public GroupLibrary GroupLibrary { get { return this._grouplibrary; } }
        public TsMainWindow TsMainWindow { get; set; }
        public OptionLibrary OptionLibrary { get { return this._optionlibrary; } }
        public EnvironmentController EnvironmentController { get { return this._envController; } }
        public bool StartupFinished { get; set; }
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }
        public bool ShowGridLines { get; set; }

        //constructors
        public Director(MainWindow ParentWindow, Arguments Arguments)
        {
            LoggerFacade.Trace("MainController initialized");
            this._envController = new EnvironmentController(this);
            this._configpath = Arguments.ConfigFile;
            this.ParentWindow = ParentWindow;
            this.ParentWindow.MouseLeftButtonUp += this.OnWindowMouseUp;
            this.ParentWindow.LocationChanged += this.OnWindowMoving;
            this.Init();          
        }

        //Wrap a generic exception handler to get some useful information in the event of a 
        //crash. 
        private void Init()
        {
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
            this.StartupFinished = false;
            this._prodmode = this._envController.Init();

            this.TsMainWindow = new TsMainWindow(this.ParentWindow);
            XElement x = this.ReadConfigFile();
            if (x == null) { return; }

            //this.LoadXml(x);
            try { this.LoadXml(x); }
            catch (TsGuiKnownException e)
            {
                string msg = "Error loading config file" + Environment.NewLine + e.CustomMessage + Environment.NewLine + e.Message;
                this.CloseWithError("Error loading config file", msg);
                return;
            }

            this.PopulateHwOptions();

            //if prodmode isn't true, the envcontroller couldn't connect to sccm
            //prompt the user if they want to continue. exit if not. 
            if (this._prodmode == true)
            { this._envController.HideProgressUI(); }
            else
            {
                if (this.PromptTestMode() != true) { this.Cancel(); return; }
                if (this._livedata == true) { this._showtestwindow = true; }
            }

            //subscribe to closing event
            this.ParentWindow.Closing += this.OnWindowClosing;
            this.CurrentPage = this._pages.First();

            //update group settings to all controls
            foreach (IToggleControl t in this._toggles)
            { t.InitialiseToggle(); }

            this.ParentWindow.DataContext = this.TsMainWindow;
            
            // Now show and close the ghost window to make sure WinPE honours the 
            // windowstartuplocation
            GhostWindow ghost = new GhostWindow();
            ghost.Show();
            ghost.Close();

            this.UpdateWindow();
            this.ParentWindow.Visibility = Visibility.Visible;
            this.ParentWindow.WindowStartupLocation = this.TsMainWindow.WindowLocation.StartupLocation;
            this.StartupFinished = true;
            if ((this._debug == true) || (this._showtestwindow == true)) { this._testingwindow = new TestingWindow(this); }
            LoggerFacade.Info("*TsGui startup finished");
        }

        //attempt to read the config.xml file, and display the right messages if it fails
        private XElement ReadConfigFile()
        {
            XElement x;
            //code to be added to make sure config file exists
            try
            {
                x = XmlHandler.Read(this._configpath);
                return x;
            }
            catch (System.IO.FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error reading config file", MessageBoxButton.OK, MessageBoxImage.Error);
                this.ParentWindow.Close();
                return null;
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
                this.TsMainWindow.LoadXml(SourceXml);

                this._debug = XmlHandler.GetBoolFromXAttribute(SourceXml, "Debug", this._debug);
                this._livedata = XmlHandler.GetBoolFromXAttribute(SourceXml, "LiveData", this._livedata);
                
                //Set show grid lines after pages and columns have been created.
                x = SourceXml.Element("ShowGridLines");
                if ((x != null) && (this._prodmode == false))
                { this.ShowGridLines = true; }

                //turn hardware eval on or off
                x = SourceXml.Element("HardwareEval");
                if (x != null)
                { this._chassischeck = new HardwareEvaluator(); }

                this._buttons.LoadXml(SourceXml.Element("Buttons"));
                PageDefaults pagedef = new PageDefaults();

                x = SourceXml.Element("Heading");
                if (x != null) { pagedef.PageHeader = new TsPageHeader(x, this); }
                else { pagedef.PageHeader = new TsPageHeader(this); }

                x = SourceXml.Element("LeftPane");
                if (x != null) { pagedef.LeftPane = new TsPane(x,this); }
                else { pagedef.LeftPane = new TsPane(this); }

                x = SourceXml.Element("RightPane");
                if (x != null) { pagedef.RightPane = new TsPane(x, this); }
                else { pagedef.RightPane = new TsPane(this); }

                pagedef.Buttons = this._buttons;
                pagedef.Parent = this.TsMainWindow;
                pagedef.RootController = this;

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
                            currPage = new TsPage(xPage,pagedef, this);                                                     
                        }
                        else
                        {
                            currPage = new TsPage( xPage,pagedef,this);
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
                    this._nouicontainer = new NoUIContainer(this, x);
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
            this.ParentWindow.ContentArea.Navigate(this.CurrentPage.Page);
            this.ParentWindow.ContentArea.DataContext = this.CurrentPage;
            this.CurrentPage.Update();
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
                    { this._envController.AddVariable(new TsVariable(option.VariableName,option.InactiveValue)); }
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
            if (_finished) { this._envController.AddVariable(new TsVariable("TsGui_Cancel", "FALSE")); }
            else { this._envController.AddVariable(new TsVariable("TsGui_Cancel", "TRUE")); }
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
            this.WindowMoving?.Invoke(o, e);
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
            if (this._chassischeck != null)
            {
                foreach (TsVariable var in this._chassischeck.GetTsVariables())
                {
                    NoUIOption newhwoption = new NoUIOption(this);
                    newhwoption.ImportFromTsVariable(var);
                    this._optionlibrary.Add(newhwoption);
                }
            }
        }
    }
}
