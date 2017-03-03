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

// TsPage.cs - view model class for a page in TsGui

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows;

using TsGui.Events;
using TsGui.Grouping;
using TsGui.View.GuiOptions;
using TsGui.Validation;

namespace TsGui.View.Layout
{
    public class TsPage: BaseLayoutElement, IRootLayoutElement
    {
        public event ComplianceRetryEventHandler ComplianceRetry;

        private TsPageUI _pageui;
        private TsPage _previouspage;
        private TsPage _nextpage;
        private bool _isfirst = false;
        private TsTable _table;

        //Properties
        #region
        public TsPane LeftPane { get; set; }
        public TsPane RightPane { get; set; }
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
        public List<IGuiOption> Options { get { return this._table.Options; } }
        public TsPageUI Page { get { return this._pageui; } }
        public bool IsFirst
        {
            get { return this._isfirst; }
            set { this._isfirst = value; }
        }
        #endregion

        //Events
        #region
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
            //this._controller = Defaults.RootController;
            this.ShowGridLines = MainController.ShowGridLines;
            this._pageui = new TsPageUI(this);
            this.PageHeader = Defaults.PageHeader;
            this.LeftPane = Defaults.LeftPane;
            this.RightPane = Defaults.RightPane;

            this._pageui.Loaded += this.OnWindowLoaded;          
            this._pageui.DataContext = this;
            this._pageui.ButtonGrid.DataContext = Defaults.Buttons;

            this.LoadXml(SourceXml);
            this.Update();
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement x;

            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);

            x = InputXml.Element("Heading");
            if (x != null) { this.PageHeader = new TsPageHeader(this,this.PageHeader,x,this._controller); }

            x = InputXml.Element("LeftPane");
            if (x != null) { this.LeftPane = new TsPane(x, this._controller); }

            x = InputXml.Element("RightPane");
            if (x != null) { this.RightPane = new TsPane(x, this._controller); }

            //create the table adn bind it to the content
            this._table = new TsTable(InputXml, this, this._controller);
            this._pageui.MainTablePresenter.Content = this._table.Grid;
            this._pageui.LeftPanePresenter.Content = this.LeftPane?.PaneUI;
            this._pageui.RightPanePresenter.Content = this.RightPane?.PaneUI;
        }

        public bool OptionsValid()
        {
            if ((ResultValidator.OptionsValid(this._table.ValidationOptions)) && this.PageHeader.OptionsValid()) { return true; }
            else { return false; }
        }

        public void Cancel()
        {
            this._controller.Cancel();
        }

        public void MovePrevious()
        {
            foreach (IValidationGuiOption option in this._table.ValidationOptions)
            { option.ClearToolTips(); }

            this.ReleaseThisPage();
            this._controller.MovePrevious();
        }

        public void MoveNext()
        {
            if (this.OptionsValid() == true)
            {
                this.ReleaseThisPage();
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
            this.PreviousActivePage?.Update();
        }

        private void ReleaseThisPage()
        {
            this._pageui.HeaderPresenter.Content = null;
            this._pageui.LeftPanePresenter.Content = null;
            this._pageui.RightPanePresenter.Content = null;
        }

        //Update the prev, next, finish buttons according to the current pages 
        //place in the world
        public void Update()
        {
            this._pageui.HeaderPresenter.Content = this.PageHeader.UI;
            this._pageui.LeftPanePresenter.Content = this.LeftPane?.PaneUI;
            this._pageui.RightPanePresenter.Content = this.RightPane?.PaneUI;
            TsButtons.Update(this, this._pageui);
        }

        public void OnSurroundingPageHide(object o, GroupingEventArgs e)
        {
            if (e.GroupStateChanged == GroupStateChanged.IsHidden) { this.Update(); }
        }

        public void RaiseComplianceRetryEvent()
        {
            this.ComplianceRetry?.Invoke(this, new RoutedEventArgs());
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
