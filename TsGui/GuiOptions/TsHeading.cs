using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsHeading: TsBaseOption,IGuiOption
    {
        //private string name;
        //private string label;
        //private int height = 25;
        //private Thickness padding = new Thickness(0, 0, 0, 0);
        //private Label labelcontrol;
        new private Label _control;
        private bool bold;

        public TsHeading(XElement SourceXml)
        {
            this._control = new Label();
            base._control = this._control;
            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable { get { return null; } }
        //public Label Label { get { return this._labelcontrol; } }
        //public Control Control { get { return this._control; } }
        //public int Height { get { return this.height; } }

        public void LoadXml(XElement InputXml)
        {
            #region
            XElement x;
            this.LoadBaseXml(InputXml);
            //x = InputXml.Element("Variable");
            //if (x != null)
            //{ this.VariableName = x.Value; }

            //x = InputXml.Element("Label");
            //if (x != null)
            //{ this._labeltext = x.Value; }

            //x = InputXml.Element("Height");
            //if (x != null)
            //{ this._height = Convert.ToInt32(x.Value); }

            x = InputXml.Element("Bold");
            if (x != null)
            { this.bold = true; }

            //x = InputXml.Element("Padding");
            //if (x != null)
            //{
            //    int padInt = Convert.ToInt32(x.Value);
            //    this._padding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            //}
            #endregion
        }

        private void Build()
        {
            this._control = new Label();
            this._control.Content = "";
            this._control.Padding = this._padding;
            this._control.VerticalAlignment = VerticalAlignment.Center;

            this._labelcontrol = new Label();
            this._labelcontrol.Height = this._height;
            this._labelcontrol.Content = this._value;
            this._labelcontrol.Height = this._height;
            this._labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            if (this.bold) { this._labelcontrol.FontWeight = FontWeights.Bold; }
        }
    }
}
