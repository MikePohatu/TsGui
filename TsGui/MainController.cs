//'Root' class from which to spin everything off. TsGui creates this
//class to do the actual work. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using System.ComponentModel;

using System.Diagnostics;

namespace TsGui
{
    public class MainController
    {
        private string _configpath;
        private bool _prodmode = false;
        private bool _finished = false;
        private TsMainWindow _mainWindow;
        private XmlHandler _handler = new XmlHandler();
        private List<TsPage> _pages = new List<TsPage>();
        private EnvironmentController _envController = new EnvironmentController();
        private OptionLibrary _optionlibrary = new OptionLibrary();
        private HardwareEvaluator _chassischeck;

        //properties
        public MainWindow ParentWindow { get; set; }
        public TsPage CurrentPage { get; set; }

        //constructors
        public MainController(MainWindow ParentWindow)
        {
            this.ParentWindow = ParentWindow;
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            this._configpath = exefolder + @"Config.xml";
            this.Init();          
        }

        public MainController(MainWindow ParentWindow, string ConfigPath)
        {
            this.ParentWindow = ParentWindow;
            this._configpath = ConfigPath;
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
                //this.CurrentPage.Window.Close();
            }
        }

        public void Startup()
        {
            this._mainWindow = new TsMainWindow();
            this.ParentWindow.DataContext = this._mainWindow;
            //this.ParentWindow.ContentWrapper.DataContext = this._mainWindow;

            this._prodmode = this._envController.Init();

            //if testingmode is true, the envcontroller couldn't connect to sccm
            //prompt the user if they want to continue. exit if not. 
            if (this._prodmode != true)
            {
                if (this.PromptTestMode() != true)
                {
                    this.Cancel();
                    return;
                }               
            }
            
            XElement x = this.ReadConfigFile();
            if (x == null) { return; }

            this.LoadXml(x);

            //subscribe to closing event
            this.ParentWindow.Closing += this.OnWindowClosing;

            //now show the first page in the list
            this.CurrentPage = this._pages.First();
            this.UpdateWindow();
        }

        //attempt to read the config.xml file, and display the right messages if it fails
        private XElement ReadConfigFile()
        {
            XElement x;
            //code to be added to make sure config file exists
            try
            {
                x = _handler.Read(this._configpath);
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
                //turn hardware eval on or off
                x = SourceXml.Element("HardwareEval");
                if (x != null)
                { this._chassischeck = new HardwareEvaluator(); }

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
                            currPage = new TsPage(
                                xPage, 
                                this._mainWindow.HeadingTitle, 
                                this._mainWindow.HeadingText, 
                                this._mainWindow.Height, 
                                this._mainWindow.Width, 
                                this._mainWindow.PageMargin,
                                this._mainWindow.HeadingBgColor,
                                this._mainWindow.HeadingFontColor,
                                this);                                                     
                        }
                        else
                        {
                            currPage = new TsPage(
                                xPage,
                                this._mainWindow.HeadingTitle,
                                this._mainWindow.HeadingText,
                                this._mainWindow.Height,
                                this._mainWindow.Width,
                                this._mainWindow.PageMargin,
                                this._mainWindow.HeadingBgColor,
                                this._mainWindow.HeadingFontColor,
                                this);
                            currPage.IsFirst = true;
                        }

                        //create the new page and assign the next page/prev page links
                        currPage.PreviousPage = prevPage;
                        if (prevPage != null) { prevPage.NextPage = currPage; }

                        this._pages.Add(currPage);
                        #endregion
                    }

                    currPage.IsLast = true;
                }

                //Set show grid lines after pages and columns have been created.
                x = SourceXml.Element("ShowGridLines");
                if (x != null)
                { this.ShowGridLines(true); }
            }
        }

        //add options from sub classes to the main library. used to generate the final list of 
        //tsvariables
        public void AddOptionToLibary(IGuiOption Option)
        {
            this._optionlibrary.Add(Option);
        }

        //move to the next page and update the next/prev/finish buttons
        public void MoveNext()
        {
            this.CurrentPage = this.CurrentPage.NextPage;
            this.UpdateWindow();
        }

        //move to the previous page and update the next/prev/finish buttons
        public void MovePrevious()
        {
            this.CurrentPage = this.CurrentPage.PreviousPage;
            this.UpdateWindow();
        }
        
        //Navigate to the current page, and update the datacontext of the window
        private void UpdateWindow()
        {
            this.ParentWindow.ContentArea.Navigate(this.CurrentPage.Page);
            this.ParentWindow.ContentArea.DataContext = this.CurrentPage;
        }

        public void Finish()
        {
            foreach (IGuiOption option in this._optionlibrary.Options)
            {

                if (option.Variable != null)
                {
                    //Debug.WriteLine(option.Variable.Name + ": " + option.Variable.Value);
                    this._envController.AddVariable(option.Variable);
                }

            }

            if (this._chassischeck != null)
            {
                foreach (TsVariable var in this._chassischeck.GetTsVariables)
                {
                    //Debug.WriteLine(var.Name + ": " + var.Value);
                    this._envController.AddVariable(var);
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

        public String GetValueFromList(XElement InputXml)
        {
            return this._envController.GetValueFromList(InputXml);
        }

        public String GetEnvVar(string VariableName)
        {
            return this._envController.GetEnvVar(VariableName);
        }

        private void ShowGridLines(bool IsEnabled)
        {
            //Debug.WriteLine("TestingMode: " + this._prodmode);
            if (this._prodmode != true)
            {
                this._mainWindow.ShowGridLines = true;

                foreach (TsPage page in this._pages)
                { page.ShowGridLines = IsEnabled; }
            }
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
