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

        private MainController _controller;
        //private string _value;
        private bool _caseSensValidate = false;
        private bool _isvalid = true;
        private int _height = 25;
        private int _maxlength = 0;
        private int _minlength = 0;
        private Thickness _padding = new Thickness(3, 3, 5, 3);
        private Label _labelcontrol = new Label();
        private TextBox _control = new TextBox();

        //properties
        protected string Name { get; set; }
        protected bool CaseSensitive
        {
            get { return this._caseSensValidate; }
            set { this._caseSensValidate = value; }
        }
        protected int MinLength
        {
            get { return this._minlength; }
            set { this._minlength = value; }
        }

        public string Text { get; set; }
        public string DisallowedCharacters { get; set; }   
        public Label Label { get { return this._labelcontrol; } }
        public string LabelText { get; set; }        
        public int MaxLength
        {
            get { return this._maxlength; }
            set { this._maxlength = value; }
        }        
        public TsVariable Variable
        {
            get
            {
                //this.Text = this._control.Text;
                return new TsVariable(this.Name, this.Text);
            }
        }
        public Control Control { get { return this._control; } }
        public int Height { get { return this._height; } }
        public Thickness Padding { get { return this._padding; } }
        public bool IsValid
        {
            get
            {
                this.Validate();
                return this._isvalid;
            }
        }

        //constructors
        protected TsFreeText(MainController RootController)
        {
            Debug.WriteLine("TsFreeText: protected constructor called");
            this.Startup(RootController);
        }

        public TsFreeText (XElement SourceXml, MainController RootController)
        {
            this.Startup(RootController);
            this.LoadXml(SourceXml);
            this.Build();
        }

        //Generic startup function to share between constructors
        private void Startup(MainController RootController)
        {
            this._controller = RootController;
            this._control.TextChanged += this.onChange;

            //setup the bindings
            this._control.DataContext = this;
            this._control.SetBinding(TextBox.MaxLengthProperty, new Binding("MaxLength"));
            this._control.SetBinding(TextBox.HeightProperty, new Binding("Height"));
            this._control.SetBinding(TextBox.TextProperty, new Binding("Text"));
            this._control.SetBinding(TextBox.PaddingProperty, new Binding("Padding"));

            this._labelcontrol.DataContext = this;
            this._labelcontrol.SetBinding(Label.ContentProperty, new Binding("LabelText"));
            this._labelcontrol.SetBinding(Label.HeightProperty, new Binding("Height"));
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
            { this._height = Convert.ToInt32(x.Value); }

            x = pXml.Element("Padding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this._padding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            }
            #endregion
        }


        protected void Build()
        {
            this._control.MaxLines = 1;
            this._labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

            this.OnPropertyChanged("MaxLength");
            this.OnPropertyChanged("Height");
            this.OnPropertyChanged("Text");
            this.OnPropertyChanged("Padding");
            this.OnPropertyChanged("LabelText");
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
                
                this.ShowToolTip(s);
            }
            else
            {
                this.HideToolTip();
            }

            this._isvalid = valid;
        }

        private void ShowToolTip(string Message)
        {
            ToolTip tt = this._control.ToolTip as ToolTip;
            if (tt == null)
            {
                tt = new ToolTip();
                tt.Placement = PlacementMode.Right;
                tt.HorizontalOffset = 5;
                tt.PlacementTarget = this._control;
                tt.Content = new TextBlock();
                this._control.ToolTip = tt;
            }

            TextBlock tb = tt.Content as TextBlock;
            
            tb.Text = Message;
            tt.Content = tb;
            tt.StaysOpen = true;
            tt.IsOpen = true;
        }

        private void HideToolTip()
        {
            if (this._control.ToolTip != null)
            {
                ToolTip tt = this._control.ToolTip as ToolTip;
                tt.StaysOpen = false;
                tt.IsOpen = false;
            }
        }
    }
}
