using System;
using System.Xml.Linq;
using System.Windows.Controls;

namespace core
{
    public class TsFreeText: IGuiOption
    {
        //private TsVariable tsvar;
        private string name;
        private string value;
        private string label;
        private TextBox control;

        public TsFreeText (XElement SourceXml)
        {
            this.LoadXml(SourceXml);
        }

        public TsVariable Variable
        {
            get { return new TsVariable(this.name,this.value); }
            //set { this.tsvar = value; }
        }

        public string Label { get { return this.label; } }
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
        }
    }
}
