using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;

namespace TsGui
{
    public class TsFreeText: IGuiOption, IEditableGuiOption
    {
        //private TsVariable tsvar;
        private MainController _controller;
        private string _name;
        private string _value;
        private string _label;
        private string _invalidchars;
        private bool _caseSensValidate = false;
        private bool _isvalid = true;
        private int _height = 25;
        private int _maxlength = 0;
        private int _minlength = 0;
        private Thickness _padding = new Thickness(3, 3, 5, 3);
        private Label _labelcontrol;
        private TextBox _control;

        public TsFreeText (XElement SourceXml, MainController RootController)
        {
            this._controller = RootController;
            this.LoadXml(SourceXml);
            this.Build();
            this._control.LostFocus += this.onLostFocus;
        }

        public TsVariable Variable
        {
            get
            {
                this._value = this._control.Text;
                return new TsVariable(this._name,this._value);
            }
        }

        public Label Label { get { return this._labelcontrol; } }
        public Control Control { get { return this._control; } }
        public int Height { get { return this._height; } }
        public bool IsValid
        {
            get
            {
                this.Validate();
                return this._isvalid;
            }
        }

        public void LoadXml(XElement pXml)
        {
            #region
            XElement x;
            XAttribute attrib;

            attrib = pXml.Attribute("MaxLength");
            if (attrib != null) { this._maxlength = Convert.ToInt32(attrib.Value); }

            attrib = pXml.Attribute("MinLength");
            if (attrib != null) { this._minlength = Convert.ToInt32(attrib.Value); }

            x = pXml.Element("Invalid");
            if (x != null)
            { this._invalidchars = x.Value; }

            x = pXml.Element("Variable");
            if (x != null)
            { this._name = x.Value; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            {
                this._value = this._controller.GetValueFromList(x);
                //if required, remove invalid characters and truncate
                if (String.IsNullOrEmpty(this._invalidchars) != true) { this._value = Checker.RemoveInvalid(this._value, this._invalidchars); }
                if (this._maxlength > 0) { this._value = Checker.Truncate(this._value,this._maxlength); }
            }

            x = pXml.Element("Label");
            if (x != null)
            { this._label = x.Value; }

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


        private void Build()
        {
            this._control = new TextBox();
            this._control.MaxLines = 1;
            this._control.MaxLength = 2048;
            this._control.Height = this._height;
            this._control.Text = this._value;
            this._control.Padding = this._padding;

            this._labelcontrol = new Label();
            //this.labelcontrol.Height = "Auto";
            this._labelcontrol.Content = this._label;
            this._labelcontrol.Height = this._height;
            this._labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }


        private void onLostFocus(object sender, RoutedEventArgs e)
        {
            this.Validate();
        }

        public void Validate()
        {
            string s = "";
            bool valid = true;

            if (Checker.ValidCharacters(this._control.Text,this._invalidchars,this._caseSensValidate) != true)
            {
                s = "Invalid characters: " + this._invalidchars + Environment.NewLine;
                valid = false;
            }

            if (Checker.ValidMaxLength(this._control.Text,this._maxlength) == false)
            {
                s = s + "Maximum length: " + this._maxlength + " characters" + Environment.NewLine;
                valid = false;
            }
            else if (Checker.ValidMinLength(this._control.Text, this._minlength) == false)
            {
                s = s + "Minimum length: " + this._minlength + " characters" + Environment.NewLine;
                valid = false;
            }

            if (valid == false)
            {
                s = this._control.Text + " is invalid:" + Environment.NewLine + Environment.NewLine + s;
                MessageBox.Show(s);
            }
            this._isvalid = valid;
        }
    }
}
