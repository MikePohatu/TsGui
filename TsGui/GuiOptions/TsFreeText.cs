using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Data;
using System.Diagnostics;
using System.ComponentModel;


namespace TsGui
{
    public class TsFreeText: IGuiOption, IEditableGuiOption, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //fields
        #region
        private MainController _controller;
        private string _value;
        private string _labeltext;
        private bool _caseSensValidate;
        private bool _isvalid;
        private int _height;
        private int _maxlength;
        private int _minlength;
        private Thickness _padding;
        private Label _labelcontrol;
        private TextBox _control;
        private string _help;
        private ToolTip _tooltip;
        private ToolTip _validToolTip;
        #endregion

        //properties
        #region
        public string Name { get; set; }
        public string DisallowedCharacters { get; set; }
        public Label Label { get { return this._labelcontrol; } }
        public Control Control { get { return this._control; } }
        public bool CaseSensitive
        {
            get { return this._caseSensValidate; }
            set { this._caseSensValidate = value; }
        }
        protected int MinLength
        {
            get { return this._minlength; }
            set
            {
                if (value < 0) { this._minlength = 0; }
                else { this._minlength = value; }
            }
        }
        public string Text
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.OnPropertyChanged("Text");
            }
        }
        public string HelpText
        {
            get { return this._help; }
            set
            {
                this._help = value;
                this.OnPropertyChanged("ToolTip");
            }
        }

        public ToolTip ToolTip
        {
            get { return this._tooltip; }
        }

        public string LabelText
        {
            get { return this._labeltext; }
            set
            {
                this._labeltext = value;
                this.OnPropertyChanged("LabelText");
            }
        }        
        public int MaxLength
        {
            get { return this._maxlength; }
            set {
                this._maxlength = value;
                this.OnPropertyChanged("MaxLength"); 
            }
        }        
        public TsVariable Variable
        {
            get
            {
                return new TsVariable(this.Name, this.Text);
            }
        }
        
        public int Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged("Height");
            }
        }
        public Thickness Padding
        {
            get { return this._padding; }
            set
            {
                this._padding = value;
                this.OnPropertyChanged("Padding");
            }
        }
        public bool IsValid
        {
            get
            {
                this.Validate();
                return this._isvalid;
            }
        }
        #endregion


        //constructors
        #region
        protected TsFreeText(MainController RootController)
        {
            Debug.WriteLine("TsFreeText: protected constructor called");
            this.Startup(RootController);
        }

        public TsFreeText (XElement SourceXml, MainController RootController)
        {
            this.Startup(RootController);
            this.LoadXml(SourceXml);
        }
        #endregion


        //Generic startup function to share between constructors
        private void Startup(MainController RootController)
        {
            this._caseSensValidate = false;
            this._isvalid = true;
            this._height = 25;
            this._maxlength = 0;
            this._minlength = 0;
            this._padding = new Thickness(3, 3, 5, 3);
            this._labelcontrol = new Label();
            this._control = new TextBox();
            this._validToolTip = new ToolTip();
            this._controller = RootController;

            //Subscribe to events
            this._control.MouseEnter += this.onHoverOn;
            this._control.MouseLeave += this.onHoverOff;
            this._control.TextChanged += this.onChange;

            //Setup the tooltips
            this._tooltip = WindowAlerts.SetToolTipText(this._control, this._help);
            //this._labelcontrol.ToolTip = WindowAlerts.SetToolTipText(this._control, this._help);
            TextBlock tb = this._tooltip.Content as TextBlock;

            //setup the bindings
            this._control.DataContext = this;
            tb.SetBinding(TextBlock.TextProperty, new Binding("HelpText"));

            this._control.SetBinding(TextBox.MaxLengthProperty, new Binding("MaxLength"));
            this._control.SetBinding(TextBox.HeightProperty, new Binding("Height"));
            this._control.SetBinding(TextBox.TextProperty, new Binding("Text"));
            this._control.SetBinding(TextBox.PaddingProperty, new Binding("Padding"));
            this._control.SetBinding(TextBox.ToolTipProperty, new Binding("ToolTip"));

            this._labelcontrol.DataContext = this;
            this._labelcontrol.SetBinding(Label.ContentProperty, new Binding("LabelText"));
            this._labelcontrol.SetBinding(Label.HeightProperty, new Binding("Height"));

            this._control.MaxLines = 1;
            this._labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }



        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        public void LoadXml(XElement pXml)
        {
            #region
            XElement x;
            XAttribute attrib;

            attrib = pXml.Attribute("MaxLength");
            if (attrib != null)
            { this.MaxLength = Convert.ToInt32(attrib.Value); }

            attrib = pXml.Attribute("MinLength");
            if (attrib != null)
            { this._minlength = Convert.ToInt32(attrib.Value); }

            x = pXml.Element("Disallowed");
            if (x != null)
            {
                x = x.Element("Characters");
                if (x != null) { this.DisallowedCharacters = x.Value; }               
            }

            x = pXml.Element("Variable");
            if (x != null)
            { this.Name = x.Value; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            {
                this.Text = this._controller.GetValueFromList(x);
                //if required, remove invalid characters and truncate
                if (String.IsNullOrEmpty(this.DisallowedCharacters) != true) { this.Text = Checker.RemoveInvalid(this.Text, this.DisallowedCharacters); }
                if (this._maxlength > 0) { this.Text = Checker.Truncate(this.Text, this._maxlength); }
            }

            x = pXml.Element("Label");
            if (x != null)
            { this.LabelText = x.Value; }

            x = pXml.Element("Height");
            if (x != null)
            { this.Height = Convert.ToInt32(x.Value); }

            x = pXml.Element("Padding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this.Padding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            }
            #endregion
        }

        private void onHoverOn(object sender, RoutedEventArgs e)
        {
            Control c = sender as Control;
            if (this._help != null) { WindowAlerts.ShowToolTip(c); }
            
        }

        private void onHoverOff(object sender, RoutedEventArgs e)
        {
            Control c = sender as Control;
            WindowAlerts.HideToolTip(c);
        }


        private void onChange(object sender, RoutedEventArgs e)
        {
            this.Validate();
        }

        public void Validate()
        {
            string s = "";
            bool valid = true;

            if (Checker.ValidCharacters(this._control.Text,this.DisallowedCharacters, this.CaseSensitive) != true)
            {
                s = "Invalid characters: " + this.DisallowedCharacters + Environment.NewLine;
                valid = false;
            }

            if (Checker.ValidMinLength(this._control.Text, this.MinLength) == false)
            {
                string charWord;
                if (this.MinLength == 1) { charWord = " character"; }
                else { charWord = " characters"; }

                s = s + "Minimum length: " + this.MinLength + charWord + Environment.NewLine;
                valid = false;
            }

            if (valid == false)
            {
                if ( this._control.Text.Length == 0)
                {
                    s = "Required" + Environment.NewLine + Environment.NewLine + s;
                }
                else
                {
                    s = "\"" + this._control.Text + "\" is invalid:" + Environment.NewLine + Environment.NewLine + s;
                }

                WindowAlerts.ShowUnboundToolTip(this._validToolTip,this._control,s);
            }
            else
            {
                WindowAlerts.HideToolTip(this._validToolTip);
            }

            this._isvalid = valid;
        }
    }
}
