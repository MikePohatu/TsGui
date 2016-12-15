using System.Collections.ObjectModel;
using System.Windows;
using System;

using TsGui.View.Layout;

namespace TsGui.Diagnostics
{
    public class TestingWindow
    {
        private MainController _controller;
        private TestingWindowUI _testingwindowui;
        private ObservableCollection<IOption> _options;

        public ObservableCollection<IOption> Options { get { return this._options; } }
        public TsMainWindow TsMainWindow { get; set; }
        public double ScreenHeight { get; set; }
        public double ScreenWidth { get; set; }

        public TestingWindow(MainController Controller)
        {
            this._controller = Controller;
            this.ScreenWidth = SystemParameters.PrimaryScreenWidth;
            this.ScreenHeight = SystemParameters.PrimaryScreenHeight;
            this._testingwindowui = new TestingWindowUI();
            this._testingwindowui.DataContext = this;
            this._options = this._controller.OptionLibrary.Options;
            this.TsMainWindow = this._controller.TsMainWindow;
            this._controller.ParentWindow.Closed += this.OnParentWindowClosing;
            this._testingwindowui.Show();
        }

        public void OnParentWindowClosing(object o, EventArgs e)
        {
            this._testingwindowui.Close();
        }
    }
}
