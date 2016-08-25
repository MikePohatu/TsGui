using System;
using System.Windows;
using System.ComponentModel;
using System.Xml.Linq;

namespace TsGui
{
    public class TsButtons : INotifyPropertyChanged
    {
        private string _buttonTextNext;
        private string _buttonTextFinish;
        private string _buttonTextCancel;
        private string _buttonTextBack;

        //Constructor
        public TsButtons()
        {
            this.ButtonTextBack = "Back";
            this.ButtonTextCancel = "Cancel";
            this.ButtonTextFinish = "Finish";
            this.ButtonTextNext = "Next";
        }


        //Properties
        #region
        public string ButtonTextCancel
        {
            get { return this._buttonTextCancel; }
            set
            {
                this._buttonTextCancel = value;
                this.OnPropertyChanged(this, "ButtonTextCancel");
            }
        }
        public string ButtonTextNext
        {
            get { return this._buttonTextNext; }
            set
            {
                this._buttonTextNext = value;
                this.OnPropertyChanged(this, "ButtonTextNext");
            }
        }
        public string ButtonTextBack
        {
            get { return this._buttonTextBack; }
            set
            {
                this._buttonTextBack = value;
                this.OnPropertyChanged(this, "ButtonTextBack");
            }
        }
        public string ButtonTextFinish
        {
            get { return this._buttonTextFinish; }
            set
            {
                this._buttonTextFinish = value;
                this.OnPropertyChanged(this, "ButtonTextFinish");
            }
        }
        #endregion
        //Events and handlers
        #region
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        public void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        public void LoadXml(XElement InputXml)
        {
            XElement x;
            x = InputXml.Element("Next");
            if (x != null) { this.ButtonTextNext = x.Value; }

            x = InputXml.Element("Back");
            if (x != null) { this.ButtonTextBack = x.Value; }

            x = InputXml.Element("Finish");
            if (x != null) { this.ButtonTextFinish = x.Value; }

            x = InputXml.Element("Cancel");
            if (x != null) { this.ButtonTextCancel = x.Value; }
        }


        public static void Update(TsPage Page, PageLayout Layout)
        {
            if (Page.NextActivePage != null)
            {
                Layout.buttonNext.Visibility = Visibility.Visible;
                Layout.buttonNext.IsEnabled = true;
                Layout.buttonFinish.Visibility = Visibility.Hidden;
                Layout.buttonFinish.IsEnabled = false;
            }
            else
            {
                Layout.buttonFinish.Visibility = Visibility.Visible;
                Layout.buttonFinish.IsEnabled = true;
                Layout.buttonNext.Visibility = Visibility.Hidden;
                Layout.buttonNext.IsEnabled = false;
            }

            if (Page.PreviousActivePage != null)
            {
                Layout.buttonPrev.Visibility = Visibility.Visible;
                Layout.buttonPrev.IsEnabled = true;
            }
            else
            {
                Layout.buttonPrev.Visibility = Visibility.Hidden;
                Layout.buttonPrev.IsEnabled = false;
            }
        }
    }
}

