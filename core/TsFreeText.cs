using System;
using System.Xml.Linq;

namespace core
{
    public class TsFreeText: IGuiOption
    {
        //private TsVariable tsvar;
        private string name;
        private string value;
        private string label;

        public TsVariable TsVariable
        {
            get { return new TsVariable(this.name,this.value); }
            //set { this.tsvar = value; }
        }

        public string Label
        {
            get { return this.label; }
        }

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

        //public XElement GetXml()
        //{
        //    XElement x = new XElement("SystemSettings",
        //        new XElement("DefaultInterval", this.DefaultInterval),
        //        new XElement("MinInterval", this.MinInterval),
        //        new XElement("MaxInterval", this.MaxInterval));

        //    return x;
        //    //this.Interval = XmlConvert.ToInt32(pXml.Element("Interval").Value);
        //}
    }
}
