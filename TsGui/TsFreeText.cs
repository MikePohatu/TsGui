using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsFreeText: IGuiOption
    {
        //private TsVariable tsvar;
        private string name;
        private string value;
        private string label;
        private int height = 23;
        private Thickness padding = new Thickness(3, 3, 5, 3);
        private Label labelcontrol;
        private TextBox control;

        public TsFreeText (XElement SourceXml)
        {
            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable
        {
            get { return new TsVariable(this.name,this.value); }
            //set { this.tsvar = value; }
        }

        public Label Label { get { return this.labelcontrol; } }
        public Control Control { get { return this.control; } }

        public void LoadXml(XElement pXml)
        {
            XElement x;

            x = pXml.Element("Variable");
            if (x != null)
            { this.name = x.Value; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            { this.value = x.Value; }

            x = pXml.Element("Label");
            if (x != null)
            { this.label = x.Value; }

            x = pXml.Element("Height");
            if (x != null)
            { this.height = Convert.ToInt32(x.Value); }

            x = pXml.Element("Padding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this.padding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            }
        }

        private void Build()
        {
            this.control = new TextBox();
            this.control.MaxLines = 1;
            this.control.MaxLength = 2048;
            this.control.Height = this.height;
            this.control.Text = this.value;
            this.control.Padding = this.padding;

            this.labelcontrol = new Label();
            //this.labelcontrol.Height = "Auto";
            this.labelcontrol.Content = this.label;
            this.labelcontrol.Height = this.height;
            this.labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            //this.labelcontrol.Padding = this.padding;
        }
    }
}
