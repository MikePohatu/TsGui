using System;
using System.Xml.Linq;
using System.Windows.Controls;

namespace core
{
    public class TsList: IGuiOption
    {
        private string name;
        private string value;
        private string label;
        private ComboBox control;

        public TsVariable TsVariable
        {
            get { return new TsVariable(this.name, this.value); }
            //set { this.tsvar = value; }
        }

        public string Label { get { return this.label; } }
        public ComboBox Control { get { return this.control; } }

        public bool LoadXml(XElement pXml)
        {
            XElement x;
            bool update = false;

            x = pXml.Element("Variable");
            if (x != null)
            { this.name = pXml.Element("Variable").Value; }
            else
            { update = true; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            { this.value = pXml.Element("DefaultValue").Value; }
            else
            { update = true; }

            x = pXml.Element("Label");
            if (x != null)
            { this.label = pXml.Element("Label").Value; }
            else
            { update = true; }

            return update;
        }

        private void Build()
        {
            this.control = new ComboBox();
        }
    }
}
