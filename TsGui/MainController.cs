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
using TsGui.View.GuiOptions;

using TsGui.Grouping;

namespace TsGui
{
    public class MainController
    {
        public event WindowLoadedHandler WindowLoaded;

        private string _configpath;
        private bool _prodmode = false;
        private bool _finished = false;
        private TsButtons _buttons = new TsButtons();
        private TsMainWindow _mainWindow;
        private List<TsPage> _pages = new List<TsPage>();
        private EnvironmentController _envController = new EnvironmentController();
        private Dictionary<string, Group> _groups = new Dictionary<string, Group>();
        private List<IToggleControl> _toggles = new List<IToggleControl>();
        private List<IEditableGuiOption> _editables = new List<IEditableGuiOption>();
        private OptionLibrary _optionlibrary = new OptionLibrary();
        private HardwareEvaluator _chassischeck;

        //properties
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }
        public bool ShowGridLines { get; set; }

        //constructors
        public MainController(MainWindow ParentWindow, Arguments Arguments)
        {
            this._configpath = Arguments.ConfigFile;
            this.ParentWindow = ParentWindow;
            this.Init();          
        }

        

        //Wrap a generic exception handler to get some useful information in the event of a 
        //crash. 
        private void Init()
        {
            try { this.Startup(); }
            catch (Exception exc)
            {
                string msg = "Error message: " + exc.Message + Environment.NewLine + exc.ToString();
                MessageBox.Show(msg, "Application Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                this.ParentWindow.Closing -= this.OnWindowClosing;
                this.ParentWindow.Close();
            }
        }

        public void Startup()
        {
            this._mainWindow = new TsMainWindow();
            this.ParentWindow.DataContext = this._mainWindow;

            this._prodmode = this._envController.Init();

            //if prodmode isn't true, the envcontroller couldn't connect to sccm
            //prompt the user if they want to continue. exit if not. 
            if (this._prodmode != true)
            {
                if (this.PromptTestMode() != true)
                {
                    this.Cancel();
                    return;
                }               
            }

            //this.ParentWindow.Loaded += this.OnWindowLoaded;

            XElement x = this.ReadConfigFile();
            if (x == null) { return; }

            this.LoadXml(x);

            //subscribe to closing event
            this.ParentWindow.Closing += this.OnWindowClosing;

            //now show the first page in the list
            this.CurrentPage = this._pages.First();

            //update group settings to all controls
            foreach (IToggleControl t in this._toggles)
            { t.InitialiseToggle(); }

            foreach (IEditableGuiOption g in this._editables) { g.Validate(); }

            this.UpdateWindow();
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

            catch
            {
                MessageBox.Show("Invalid config file: " + this._configpath, "Error reading config file", MessageBoxButton.OK, MessageBoxImage.Error);
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

            this._mainWindow.LoadXml(SourceXml);

            if (SourceXml != null)
            {
                //Set show grid lines after pages and columns have been created.
                x = SourceXml.Element("ShowGridLines");
                if ((x != null) && (this._prodmode = false))
                { this.ShowGridLines = true; }

                //turn hardware eval on or off
                x = SourceXml.Element("HardwareEval");
                if (x != null)
                { this._chassischeck = new HardwareEvaluator(); }

                this._buttons.LoadXml(SourceXml.Element("Buttons"));

                PageDefaults pagedef = new PageDefaults();
                pagedef.HeadingTitle = this._mainWindow.HeadingTitle;
                pagedef.HeadingText = this._mainWindow.HeadingText;
                pagedef.HeadingBgColor = this._mainWindow.HeadingBgColor;
                pagedef.HeadingFontColor = this._mainWindow.HeadingFontColor;
                pagedef.Buttons = this._buttons;
                pagedef.Parent = this._mainWindow;
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
            }
        }

        //add options from sub classes to the main library. used to generate the final list of 
        //tsvariables
        public void AddOptionToLibary(IGuiOption_2 Option)
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

        private Group CreateGroup(string ID)
        {
            Group group;
            group = new Group(ID);
            this._groups.Add(ID, group);
            return group;
        }

        /// <summary>
        /// Add a groupable element to a group
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Element"></param>
        /// <returns></returns>
        public Group AddToGroup (string ID, IGroupableUIElement Element)
        {
            Group group = this.GetGroupFromID(ID);
            group.Add(Element);
            
            return group;
        }

        /// <summary>
        /// Get a group object, create if doesn't exist
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Group GetGroupFromID(string ID)
        {
            Group group;
            this._groups.TryGetValue(ID, out group);
            if (group == null) { group = this.CreateGroup(ID); }

            return group;
        }

        /// <summary>
        /// Return the Group object from a specified ID. Creates a new one if it doesn't already exist
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Group GetGroup(string ID)
        {
            Group group;
            this._groups.TryGetValue(ID, out group);
            if (group == null)
            {
                group = this.CreateGroup(ID);
            }
            return group;
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
            foreach (IGuiOption_2 option in this._optionlibrary.Options)
            {
                //first check for null option variables e.g. for headings
                if (option.Variable != null)
                {
                    //now check if the option is active or not and variables created as required
                    if ((option.IsEnabled == true) && (option.IsHidden == false))
                    { this._envController.AddVariable(option.Variable); }
                    else
                    { this._envController.AddVariable(new TsVariable(option.VariableName,option.InactiveValue)); }
                }
            }

            if (this._chassischeck != null)
            {
                foreach (TsVariable var in this._chassischeck.GetTsVariables)
                { this._envController.AddVariable(var); }
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
            this.WindowLoaded?.Invoke();
        }

        public String GetValueFromList(XElement InputXml)
        {
            return this._envController.GetValueFromList(InputXml);
        }

        public Dictionary<string, string> GetDictionaryFromList(XElement InputXml)
        {
            return this._envController.GetDictionaryFromList(InputXml);
        }

        public List<KeyValuePair<string, string>> GetKeyValueListFromList(XElement InputXml)
        {
            return this._envController.GetKeyValueListFromList(InputXml);
        }
            

        public String GetEnvVar(string VariableName)
        {
            return this._envController.GetEnvVar(VariableName);
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
            string title = "Error";
            MessageBoxResult result = MessageBox.Show(msg, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes) return true;
            else return false;
        }
    }
}
