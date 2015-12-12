using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsFreeText: IGuiOption
    {
        //private TsVariable tsvar;
        private MainController _controller;
        private string _name;
        private string _value;
        private string _label;
        private string _invalidchars;
        private int _height = 25;
        private int _maxlength;
        private Thickness _padding = new Thickness(3, 3, 5, 3);
        private Label _labelcontrol;
        private TextBox _control;

        public TsFreeText (XElement SourceXml, MainController RootController)
        {
            this._controller = RootController;
            this.LoadXml(SourceXml);
            this.Build();
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

        public void LoadXml(XElement pXml)
        {
            #region
            XElement x;
            XAttribute attrib;

            attrib = pXml.Attribute("MaxLength");
            if (attrib != null) { this._maxlength = Convert.ToInt32(attrib.Value); }

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
    }
}
