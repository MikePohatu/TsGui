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
        private string _headingTitle;
        private string _headingText;
        private int _headingHeight;
        private string _configpath;
        private int _pageWidth;
        private int _pageHeight;
        private Thickness _pageMargin = new Thickness(0,0,0,0);
        private bool _prodmode = false;
        private XmlHandler _handler = new XmlHandler();
        private List<Page> _pages = new List<Page>();
        //private List<IGuiOption> _options = new List<IGuiOption>();
        private EnvironmentController _envController = new EnvironmentController();
        private OptionLibrary _optionlibrary = new OptionLibrary();
        private HardwareEvaluator _chassischeck;

        //properties
        public string HeadingTitle { get { return this._headingTitle; } }
        public string HeadingText { get { return this._headingText; } }
        public int HeadingHeight { get { return this._headingHeight; } }

        public MainWindow ParentWindow { get; set; }
        public Page CurrentPage { get; set; }
        

        //constructors
        public MainController(MainWindow ParentWindow)
        {
            this.ParentWindow = ParentWindow;
            //this.ParentWindow.HeadingStack.DataContext = this;
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            this._configpath = exefolder + @"Config.xml";                    
            this.Startup();
        }

        public MainController(MainWindow ParentWindow, string ConfigPath)
        {

            this.ParentWindow = ParentWindow;
            this._configpath = ConfigPath;
            this.Startup();
        }

        public void Startup()
        {
            //set default values
            this._headingHeight = 50;

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

            this.ParentWindow.Closing += this.OnWindowClosing;

            this.LoadXml(x);
            //this.PopulateOptions();

            //now show the first page in the list
            Page firstpage = this._pages.First();
            this.CurrentPage = firstpage;
            this.ParentWindow.MainGrid.Children.Add(this.CurrentPage.Panel);
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
            Page currPage = null;
            Page prevPage = null;

            IEnumerable<XElement> pagesXml;

            if (SourceXml != null)
            {

                XElement headingX = SourceXml.Element("Heading");
                if (headingX != null)
                {
                    x = headingX.Element("Title");
                    if (x != null) { this._headingTitle = x.Value; }

                    x = headingX.Element("Text");
                    if (x != null) { this._headingText = x.Value; }

                    x = headingX.Element("Height");
                    if (x != null) { this._headingHeight = Convert.ToInt32(x.Value); }
                }

                x = SourceXml.Element("HardwareEval");
                if (x != null)
                { this._chassischeck = new HardwareEvaluator(); }
                 
                x = SourceXml.Element("Width");
                if (x != null)
                { this._pageWidth = Convert.ToInt32(x.Value); }

                x = SourceXml.Element("Height");
                if (x != null)
                { this._pageHeight = Convert.ToInt32(x.Value); }

                

                GuiFactory.LoadMargins(SourceXml, this._pageMargin);

                //now read in the options and add to a dictionary for later use
                pagesXml = SourceXml.Elements("Page");
                if (pagesXml != null)
                {
                    //Debug.WriteLine("pagesXml not null");
                    foreach (XElement xPage in pagesXml)
                    {
                        #region
                        //Debug.WriteLine("creating new page");
                        if (currPage == null)
                        {
                            currPage = new Page(xPage, this._pageHeight, this._pageWidth, this._pageMargin,this);
                            currPage.IsFirst = true;
                        }
                        else
                        {
                            //record the last page as the prevPage
                            prevPage = currPage;
                            currPage = new Page(xPage, this._pageHeight, this._pageWidth, this._pageMargin,this);
                        }

                        //create the new page and assign the next page/prev page links
                        currPage.PreviousPage = prevPage;
                        if (prevPage != null) { prevPage.NextPage = currPage; }

                        this._pages.Add(currPage);
                        #endregion
                    }

                    currPage.IsLast = true;

                    //if currPage is still the first page, this is a single page tsgui. 
                    //turn the next button into the finish button
                    if (currPage.IsFirst == true)
                    {
                        this.ParentWindow.buttonNext.Visibility = Visibility.Hidden;
                        this.ParentWindow.buttonFinish.Visibility = Visibility.Visible;
                    }

                }

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
            if (this.CurrentPage.OptionsValid() == true)
            {
                this.ParentWindow.MainGrid.Children.Remove(this.CurrentPage.Panel);
                this.CurrentPage = this.CurrentPage.NextPage;
                this.ParentWindow.MainGrid.Children.Add(this.CurrentPage.Panel);
                this.SetButtons();
            }           
        }


        //move to the previous page and update the next/prev/finish buttons
        public void MovePrevious()
        {
            if (this.CurrentPage.OptionsValid() == true)
            {
                this.ParentWindow.MainGrid.Children.Remove(this.CurrentPage.Panel);
                this.CurrentPage = this.CurrentPage.PreviousPage;
                this.ParentWindow.MainGrid.Children.Add(this.CurrentPage.Panel);
                this.SetButtons();
            }        
        }


        public void SetButtons()
        {
            if (this.CurrentPage.IsLast == true)
            {
                this.ParentWindow.buttonNext.Visibility = Visibility.Hidden;
                this.ParentWindow.buttonFinish.Visibility = Visibility.Visible;
            }
            else
            {
                this.ParentWindow.buttonNext.Visibility = Visibility.Visible;
                this.ParentWindow.buttonFinish.Visibility = Visibility.Hidden;
            }


            if (this.CurrentPage.IsFirst == true)
            {
                this.ParentWindow.buttonPrev.Visibility = Visibility.Hidden;
                this.ParentWindow.buttonPrev.IsEnabled = false;
            }
            else
            {
                this.ParentWindow.buttonPrev.Visibility = Visibility.Visible;
                this.ParentWindow.buttonPrev.IsEnabled = true;
            }
        }


        public void Finish()
        {
            if (this.CurrentPage.OptionsValid() == true)
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

                this._envController.AddVariable(new TsVariable("TsGui_Cancel", "FALSE"));
                this.ParentWindow.Close();
            }
        }

        public void Cancel()
        {
            this._envController.AddVariable(new TsVariable("TsGui_Cancel", "TRUE"));
            this.ParentWindow.Close();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            this._envController.Release();
        }

        public String GetValueFromList(XElement InputXml)
        {
            return this._envController.GetValueFromList(InputXml);
        }

        private void ShowGridLines(bool OnOff)
        {
            //Debug.WriteLine("TestingMode: " + this._prodmode);
            if (this._prodmode != true)
            {
                this.ParentWindow.WrapperGrid.ShowGridLines = OnOff;
                this.ParentWindow.HeadingGrid.ShowGridLines = OnOff;
                this.ParentWindow.ButtonGrid.ShowGridLines = OnOff;

                foreach (Page page in this._pages)
                { page.ShowGridlines = OnOff; }
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
