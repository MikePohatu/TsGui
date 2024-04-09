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

// TsPage.cs - view model class for a page in TsGui

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using TsGui.Grouping;
using TsGui.View.GuiOptions;
using TsGui.Validation;
using Core.Logging;
using System;
using TsGui.View.Layout.Events;

namespace TsGui.View.Layout
{
    public class TsPage : ParentLayoutElement, IComplianceRoot
    {
        public event ComplianceRetryEventHandler ComplianceRetry;

        private TsPage _previouspage;
        private TsPage _nextpage;
        private TsTable _table;

        //Properties
        #region
        public TsPane LeftPane { get; set; }
        public TsPane RightPane { get; set; }
        public TsPageHeader PageHeader { get; set; }
        public string PageId { get; set; } = string.Empty;
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
        public TsPageUI Page { get; }
        public bool IsFirst { get; set; } = false;
        #endregion

        //Events
        #region
        public event RoutedEventHandler PageWindowLoaded;

        /// <summary>
        /// Method to handle when content has finished rendering on the window
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnPageLoaded(object o, RoutedEventArgs e)
        {
            this.PageWindowLoaded?.Invoke(o, e);
            foreach (IGuiOption opt in this._table.Options)
            {
                if (opt.IsEnabled && opt.InteractiveControl?.Focusable == true)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => opt.InteractiveControl.Focus()));
                    break;
                }
            }
        }
        #endregion

        //Constructors
        public TsPage(ParentLayoutElement Parent, XElement SourceXml, PageDefaults Defaults) : base(Parent)
        {
            //this._director = Defaults.RootController;
            Log.Info("New page created");
            this.ShowGridLines = Director.Instance.ShowGridLines;
            this.Page = new TsPageUI(this);
            this.PageHeader = Defaults.PageHeader;
            this.LeftPane = Defaults.LeftPane;
            this.RightPane = Defaults.RightPane;

            this.Page.Loaded += this.OnPageLoaded;

            this.GroupingStateChange += this.OnGroupingStateChange;
            this.Page.DataContext = this;
            this.Page.ButtonGrid.DataContext = Defaults.Buttons;
            this.Page.KeyDown += this.OnKeyDown;

            this.LoadXml(SourceXml);
            this.Update();
        }

        //Methods

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.LoadXml(InputXml, this);
        }

        public override void LoadXml(XElement InputXml, ParentLayoutElement parent)
        {
            XElement x;

            this.PageId = XmlHandler.GetStringFromXml(InputXml, "PageId", this.PageId);
            this.IsEnabled = XmlHandler.GetBoolFromXml(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXml(InputXml, "Hidden", this.IsHidden);

            x = InputXml.Element("Heading");
            if (x != null) { this.PageHeader = new TsPageHeader(parent, this.PageHeader, x); }

            x = InputXml.Element("LeftPane");
            if (x != null) { this.LeftPane = new TsPane(x); }

            x = InputXml.Element("RightPane");
            if (x != null) { this.RightPane = new TsPane(x); }

            //create the table adn bind it to the content
            this._table = new TsTable(InputXml, parent);
            this.Page.MainTablePresenter.Content = this._table.Grid;
            this.Page.LeftPanePresenter.Content = this.LeftPane?.PaneUI;
            this.Page.RightPanePresenter.Content = this.RightPane?.PaneUI;
        }

        public void Cancel()
        {
            Director.Instance.Cancel();
        }

        public void MovePrevious()
        {
            if (this._previouspage != null)
            {
                foreach (IValidationGuiOption option in this._table.ValidationOptions)
                { option.ClearToolTips(); }

                this.ReleaseThisPage();
                Director.Instance.MovePrevious();
            }
        }

        public bool AllOptionsValid()
        {
            if ((ResultValidator.AllOptionsValid(this._table.ValidationOptions)) && this.PageHeader.AllOptionsValid()) { return true; }
            else { return false; }
        }

        public void MoveNext()
        {
            if (this._nextpage == null)
            {
                Log.Error("Next page clicked but next page is null");
                return;
            }

            this.Events.InvokeLayoutEvent(LayoutTopics.NextPageClicked, EventDirection.Tunnel);

            if (this.AllOptionsValid() == true)
            {
                this.ReleaseThisPage();
                Director.Instance.MoveNext();
            }
        }

        public void Finish()
        {
            this.Events.InvokeLayoutEvent(LayoutTopics.FinishedClicked, EventDirection.Tunnel);
            if (this.AllOptionsValid() == true)
            {
                Director.Instance.Finish();
            }
        }

        private void UpdatePrevious()
        {
            this.PreviousActivePage?.Update();
        }

        private void ReleaseThisPage()
        {
            this.Page.HeaderPresenter.Content = null;
            this.Page.LeftPanePresenter.Content = null;
            this.Page.RightPanePresenter.Content = null;
        }

        //Update the prev, next, finish buttons according to the current pages 
        //place in the world
        public void Update()
        {
            this.Page.HeaderPresenter.Content = this.PageHeader.UI;
            this.Page.LeftPanePresenter.Content = this.LeftPane?.PaneUI;
            this.Page.RightPanePresenter.Content = this.RightPane?.PaneUI;
            TsButtons.Update(this, this.Page);
        }

        public void OnGroupingStateChange(object o, GroupingEventArgs e)
        {
            Director.Instance.CurrentPage?.Update();
        }

        public void RaiseComplianceRetryEvent()
        {
            this.ComplianceRetry?.Invoke(this, new RoutedEventArgs());
        }

        private void ConnectNextPage(TsPage NewNextPage)
        {
            this._nextpage = NewNextPage;
        }

        private void ConnectPrevPage(TsPage NewPrevPage)
        {
            this._previouspage = NewPrevPage;
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (e.SystemKey == Key.Right)
                {
                    this.MoveNext();
                }
                else if (e.SystemKey == Key.Left)
                {
                    this.MovePrevious();
                }
                e.Handled = true;
            }
        }
    }
}
