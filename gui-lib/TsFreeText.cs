using System;
using System.Xml.Linq;
using System.Windows.Controls;

namespace gui_lib
{
    public class TsFreeText: IGuiOption
    {
        //private TsVariable tsvar;
        private string name;
        private string value;
        private string label;
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
            { this.name = pXml.Element("Variable").Value; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            { this.value = pXml.Element("DefaultValue").Value; }

            x = pXml.Element("Label");
            if (x != null)
            { this.label = pXml.Element("Label").Value; }
        }

        private void Build()
        {
            this.control = new TextBox();
            this.control.MaxLines = 1;
            this.control.MaxLength = 2048;
            this.labelcontrol = new Label();
            //this.labelcontrol.Height = "Auto";
            this.labelcontrol.Content = this.label;
        }
    }
}
