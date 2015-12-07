using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsHeading: IGuiOption
    {
        private string name;
        private string label;
        private int height = 25;
        private Thickness padding = new Thickness(0, 0, 0, 0);
        private Label labelcontrol;
        private Label control;
        private bool bold;

        public TsHeading(XElement SourceXml)
        {
            this.control = new Label();
            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable { get { return null; } }
        public Label Label { get { return this.labelcontrol; } }
        public Control Control { get { return this.control; } }
        public int Height { get { return this.height; } }

        public void LoadXml(XElement pXml)
        {
            #region
            XElement x;

            x = pXml.Element("Variable");
            if (x != null)
            { this.name = x.Value; }

            x = pXml.Element("Label");
            if (x != null)
            { this.label = x.Value; }

            x = pXml.Element("Height");
            if (x != null)
            { this.height = Convert.ToInt32(x.Value); }

            x = pXml.Element("Bold");
            if (x != null)
            { this.bold = true; }

            x = pXml.Element("Padding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this.padding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            }
            #endregion
        }

        private void Build()
        {
            this.control = new Label();
            this.control.Content = "";
            this.control.Padding = this.padding;
            this.control.VerticalAlignment = VerticalAlignment.Center;

            this.labelcontrol = new Label();
            this.labelcontrol.Height = this.height;
            this.labelcontrol.Content = this.label;
            this.labelcontrol.Height = this.height;
            this.labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            if (this.bold) { this.labelcontrol.FontWeight = FontWeights.Bold; }
        }
    }
}
