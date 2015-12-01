//'Root' class from which to spin everything off. TsGui or TsGui-Tester create this
//class to do the actual work. 

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

using System.Diagnostics;

namespace TsGui
{
    public class Controller
    {
        private string configpath;
        private int pageWidth;
        private int pageHeight;
        private int pagePadding;
        private bool testingmode = false;
        private XmlHandler handler = new XmlHandler();
        private List<Page> pages = new List<Page>();
        private List<IGuiOption> options = new List<IGuiOption>();
        private ITsVariableOutput outputconnector;

        //properties
        public MainWindow ParentWindow { get; set; }
        public Page CurrentPage { get; set; }
        public bool TestingMode
        {
            get { return this.testingmode; }
            set { this.testingmode = value; }
        }

        //constructors
        public Controller(MainWindow ParentWindow)
        {
            this.ParentWindow = ParentWindow;
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            this.configpath = exefolder + @"\Config.xml";
            Debug.WriteLine(this.configpath);                       
            this.CreateSccmObject();
        }

        public Controller(MainWindow ParentWindow, string ConfigPath)
        {
            this.ParentWindow = ParentWindow;
            this.configpath = ConfigPath;
            this.CreateSccmObject();
            this.Startup();
        }

        public void Startup()
        {
            //code to be added to make sure config file exists
            XElement x = handler.Read(this.configpath);
            this.LoadXml(x);
            this.PopulateOptions();

            //now show the first page in the list
            Page firstpage = this.pages.First();
            this.CurrentPage = firstpage;
            this.ParentWindow.MainGrid.Children.Add(this.CurrentPage.Panel);
        }

        private void CreateSccmObject()
        {
            try { this.outputconnector = new SccmConnector(); }
            catch
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
                MessageBoxResult result = MessageBox.Show(msg, title,MessageBoxButton.YesNo,MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    this.SetupTestMode();
                }
                else { this.ParentWindow.Close(); }
            }
        }

        private void SetupTestMode()
        {
            this.testingmode = true;
            this.outputconnector = new TestingConnector();
        }


        private void LoadXml(XElement SourceXml)
        {
            #region
            XElement x;
            Page currPage = null;
            Page prevPage = null;

            IEnumerable<XElement> pagesXml;
            //Debug.WriteLine("Source: " + SourceXml);

            Debug.WriteLine("Starting xml load in builder");

            if (SourceXml != null)
            {

                x = SourceXml.Element("Width");
                if (x != null)
                { this.pageWidth = Convert.ToInt32(x.Value); }

                x = SourceXml.Element("Height");
                if (x != null)
                { this.pageHeight = Convert.ToInt32(x.Value); }

                x = SourceXml.Element("Padding");
                if (x != null)
                { this.pagePadding = Convert.ToInt32(x.Value); }

                //now read in the options and add to a dictionary for later use
                pagesXml = SourceXml.Elements("Page");
                if (pagesXml != null)
                {
                    Debug.WriteLine("pagesXml not null");
                    foreach (XElement xPage in pagesXml)
                    {
                        Debug.WriteLine("creating new page");
                        if (currPage == null)
                        {
                            currPage = new Page(xPage, this.pageHeight, this.pageWidth, this.pagePadding);
                            currPage.IsFirst = true;
                        }
                        else
                        {
                            //record the last page as the prevPage
                            prevPage = currPage;
                            currPage = new Page(xPage, this.pageHeight, this.pageWidth, this.pagePadding);                 
                        }
                        
                        //create the new page and assign the next page/prev page links
                        currPage.PreviousPage = prevPage;
                        if (prevPage != null) { prevPage.NextPage = currPage; }

                        this.pages.Add(currPage);
                    }

                    currPage.IsLast = true;
                    this.ParentWindow.buttonPrev.Visibility = Visibility.Hidden;
                    this.ParentWindow.buttonPrev.IsEnabled = false;
                }
            }
            #endregion
        }


        private void PopulateOptions()
        {
            foreach (Page pg in this.pages)
            {
                this.options.AddRange(pg.Options);
            }
        }

        //move to the next page and update the next/prev/finish buttons
        public void MoveNext()
        {
            this.ParentWindow.MainGrid.Children.Remove(this.CurrentPage.Panel);
            this.CurrentPage = this.CurrentPage.NextPage;
            this.ParentWindow.MainGrid.Children.Add(this.CurrentPage.Panel);

            if (this.CurrentPage.IsLast == true)
            {
                this.ParentWindow.buttonNext.Visibility = Visibility.Hidden;
                this.ParentWindow.buttonFinish.Visibility = Visibility.Visible;
            }

            if (this.CurrentPage.IsFirst == false)
            {
                this.ParentWindow.buttonPrev.Visibility = Visibility.Visible;
                this.ParentWindow.buttonPrev.IsEnabled = true;
            }
        }

        //move to the previous page and update the next/prev/finish buttons
        public void MovePrevious()
        {
            this.ParentWindow.MainGrid.Children.Remove(this.CurrentPage.Panel);
            this.CurrentPage = this.CurrentPage.PreviousPage;
            this.ParentWindow.MainGrid.Children.Add(this.CurrentPage.Panel);

            if (this.CurrentPage.IsLast == false)
            {
                this.ParentWindow.buttonNext.Visibility = Visibility.Visible;
                this.ParentWindow.buttonFinish.Visibility = Visibility.Hidden;
            }

            if (this.CurrentPage.IsFirst == true)
            {
                this.ParentWindow.buttonPrev.Visibility = Visibility.Hidden;
                this.ParentWindow.buttonPrev.IsEnabled = false;
            }
        }

        public void Finish()
        {
            foreach (IGuiOption option in this.options)
            {
                Debug.WriteLine(option.Variable.Name + ": " + option.Variable.Value);
                this.outputconnector.AddVariable(option.Variable);
            }
            this.outputconnector.AddVariable(new TsVariable("TsGui_Cancel", "FALSE"));
            this.ParentWindow.Close();
        }

        public void Cancel()
        {
            this.outputconnector.AddVariable(new TsVariable("TsGui_Cancel", "TRUE"));
            this.ParentWindow.Close();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            this.outputconnector.Release();
        }
    }
}
