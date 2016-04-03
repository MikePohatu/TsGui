using System;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;

namespace TsGui
{
    public abstract class TsBaseOption: INotifyPropertyChanged
    {
        protected MainController _controller;
        protected string _value;
        protected string _help;
        protected int _height;
        protected ToolTip _tooltip;
        protected Label _labelcontrol;
        protected Control _control;
        protected string _labeltext;
        protected Thickness _padding;
        protected Thickness _labelpadding;

        public string VariableName { get; set; }
        public Label Label { get { return this._labelcontrol; } }
        public Control Control { get { return this._control; } }
        public string Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.OnPropertyChanged(this, "Value");
            }
        }
        public string LabelText
        {
            get { return this._labeltext; }
            set
            {
                this._labeltext = value;
                this.OnPropertyChanged(this, "LabelText");
            }
        }
        public Thickness Padding
        {
            get { return this._padding; }
            set
            {
                this._padding = value;
                this.OnPropertyChanged(this, "Padding");
            }
        }
        public int Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "Height");
            }
        }
        public string HelpText
        {
            get { return this._help; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._help = value;
                    //Setup the tooltips
                    this._tooltip = TsWindowAlerts.SetToolTipText(this._control, this._help);
                    this._labelcontrol.ToolTip = this._tooltip;

                    //setup the binding
                    TextBlock tb = this._tooltip.Content as TextBlock;
                    tb.SetBinding(TextBlock.TextProperty, new Binding("HelpText"));

                    OnPropertyChanged(this, "HelpText");
                }
            }
        }
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        //constructor
        protected TsBaseOption()
        {
            this._labelcontrol = new Label();
            this._labelcontrol.DataContext = this;
            this._labelcontrol.SetBinding(Label.ContentProperty, new Binding("LabelText"));
            this._labelcontrol.SetBinding(Label.HeightProperty, new Binding("Height"));

            this._labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            this._labelpadding = new Thickness(5, 0, 0, 0);
            this._labelcontrol.Padding = this._labelpadding;
        }

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
        }

        protected void LoadBaseXml(XElement InputXml)
        {
            //Load the XML
            #region
            XElement x;
            //XAttribute attrib;

            x = InputXml.Element("Variable");
            if (x != null)
            { this.VariableName = x.Value; }

            x = InputXml.Element("HelpText");
            if (x != null)
            { this.HelpText = x.Value; }

            x = InputXml.Element("Label");
            if (x != null)
            { this.LabelText = x.Value; }

            x = InputXml.Element("Height");
            if (x != null)
            { this.Height = Convert.ToInt32(x.Value); }

            x = InputXml.Element("Padding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this.Padding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            }
            #endregion
        }
    }
}
