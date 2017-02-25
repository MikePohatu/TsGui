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

// TsPage.cs - view model class for PageLayout

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

using TsGui.Grouping;
using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsPage: BaseLayoutElement
    {
        private List<TsRow> _rows = new List<TsRow>();
        private List<IGuiOption> _options = new List<IGuiOption>();
        private List<IValidationGuiOption> _validationoptions = new List<IValidationGuiOption>();
        private Grid _pagepanel;
        private PageLayout _pagelayout;
        private TsPage _previouspage;
        private TsPage _nextpage;
        private bool _isfirst = false;
        private TsMainWindow _parent;

        //Properties
        #region
        public TsTable Table { get; set; }
        public TsPageHeader PageHeader { get; set; }
        public TsPage NextActivePage
        {
            get
            {
                if ((this.NextPage == null) || (this.NextPage.IsHidden == false)) { return this.NextPage; }
                else { return this.NextPage.NextActivePage; }
            }
        }
        public TsPage PreviousActivePage
        {
            get
            {
                if ((this.PreviousPage == null) || (this.PreviousPage.IsHidden == false)) { return this.PreviousPage; }
                else { return this.PreviousPage.PreviousActivePage; }
            }
        }

        public TsPage PreviousPage
        {
            get { return this._previouspage; }
            set { this.ConnectPrevPage(value); }
        }
        public TsPage NextPage
        {
            get { return this._nextpage; }
            set { this.ConnectNextPage(value); }
        }        
        public List<IGuiOption> Options { get { return this._options; } }
        public PageLayout Page { get { return this._pagelayout; } }
        public bool IsFirst
        {
            get { return this._isfirst; }
            set { this._isfirst = value; }
        }
        #endregion

        //Events
        #region
        //Setup the INotifyPropertyChanged interface 

        public event TsGuiWindowEventHandler PageWindowLoaded;

        /// <summary>
        /// Method to handle when content has finished rendering on the window
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnWindowLoaded(object o, RoutedEventArgs e)
        {
            this.PageWindowLoaded?.Invoke(o,e);
        }
        #endregion

        //Constructors
        public TsPage(XElement SourceXml, PageDefaults Defaults, MainController MainController):base (MainController)
        { 
            this._parent = Defaults.Parent;
            this._controller = Defaults.RootController;
            this._pagelayout = new PageLayout(this);
            this._pagelayout.Loaded += this.OnWindowLoaded;
            this._pagepanel = this._pagelayout.MainGrid;
            this.PageHeader = Defaults.PageHeader;
            this.ShowGridLines = MainController.ShowGridLines;         

            this._pagelayout.DataContext = this;
            this._pagepanel.SetBinding(Grid.IsEnabledProperty, new Binding("IsEnabled"));
            this._pagelayout.ButtonGrid.DataContext = Defaults.Buttons;

            this.LoadXml(SourceXml);
            this.PopulateOptions();
            this.Build();
        }


        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            IEnumerable<XElement> xlist;
            XElement x;
            int index;

            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this._parent.ShowGridLines);

            x = InputXml.Element("Heading");
            if (x == null) { this.PageHeader = new TsPageHeader(this.PageHeader,x,this._controller); }

            //now read in the options and add to a dictionary for later use
            int i = 0;
            xlist = InputXml.Elements("Row");
            if (xlist != null)
            {
                index = 0;
                foreach (XElement xrow in xlist)
                {
                    this.CreateRow(xrow, index);
                    index++;
                    i++;
                }
            }

            //legacy support i.e. no row in config.xml. create a new row and add the columns 
            //to it
            if (0 == i)
            {

                xlist = InputXml.Elements("Column");
                x = new XElement("Row");

                foreach (XElement xColumn in xlist)
                {
                    x.Add(xColumn);
                }
                if (x.Elements() != null) { this.CreateRow(x, 0); }
            }

        }

        private void CreateRow(XElement InputXml, int Index)
        {
            TsRow r = new TsRow(InputXml, Index, this, this._controller);

            this._rows.Add(r);
        }

        //build the gui controls.
        public void Build()
        {
            int index = 0;

            this._pagepanel.VerticalAlignment = VerticalAlignment.Top;
            this._pagepanel.HorizontalAlignment = HorizontalAlignment.Left;

            foreach (TsRow row in this._rows)
            {
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = GridLength.Auto;

                this._pagepanel.RowDefinitions.Add(rowdef);

                Grid.SetRow(row.Panel, index);
                this._pagepanel.Children.Add(row.Panel);
                index++;
            }

            this.Update();
        }

        //get all the options from the sub columns. this is parsed up the chain to generate the final
        //list of ts variables to set at the end. 
        private void PopulateOptions()
        {
            foreach (TsRow row in this._rows)
            {
                this._options.AddRange(row.Options);
            }

            foreach (IGuiOption option in this._options)
            {
                if (option is IValidationGuiOption) { this._validationoptions.Add((IValidationGuiOption)option); }
            }
        }

        public bool OptionsValid()
        {
            foreach (IValidationGuiOption option in this._validationoptions)
            {
                if (option.IsActive == true)
                {
                    if (option.IsValid == false)
                    { return false; }
                }
            }
            return true;
        }

        public void Cancel()
        {
            this._controller.Cancel();
        }

        public void MovePrevious()
        {
            foreach (IValidationGuiOption option in this._validationoptions)
            { option.ClearToolTips(); }

            this._controller.MovePrevious();
        }

        public void MoveNext()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.MoveNext();
            }
        }

        public void Finish()
        {
            if (this.OptionsValid() == true)
            {
                this._controller.Finish();
            }
        }

        private void UpdatePrevious()
        {
            TsPage tempPage;

            tempPage = this.PreviousActivePage;
            if (tempPage != null) { tempPage.Update(); }
        }

        //Update the prev, next, finish buttons according to the current pages 
        //place in the world
        public void Update()
        {
            TsButtons.Update(this, this._pagelayout);
        }

        public void OnSurroundingPageHide(object o, GroupingEventArgs e)
        {
            if (e.GroupStateChanged == GroupStateChanged.IsHidden) { this.Update(); }
        }
        
        private void ConnectNextPage(TsPage NewNextPage)
        {
            if (this.NextPage != null) { this.NextPage.GroupingStateChange -= this.OnSurroundingPageHide; }
            this._nextpage = NewNextPage;
            if (this._nextpage != null) { this._nextpage.GroupingStateChange += this.OnSurroundingPageHide; }
        }

        private void ConnectPrevPage(TsPage NewPrevPage)
        {           
            if (this.PreviousPage != null) { this.PreviousPage.GroupingStateChange -= this.OnSurroundingPageHide; }
            this._previouspage = NewPrevPage;
            if (this._previouspage != null) { this._previouspage.GroupingStateChange += this.OnSurroundingPageHide; }
        }

    }
}
