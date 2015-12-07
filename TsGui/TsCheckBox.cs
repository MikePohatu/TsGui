using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsCheckBox: IGuiOption
    {
        //private TsVariable tsvar;
        private string name;
        private string label;
        private int height = 25;
        private Thickness margin = new Thickness(0, 0, 0, 0);
        private Label labelcontrol;
        private CheckBox control;
        private HorizontalAlignment hAlignment = HorizontalAlignment.Left;
        private string valTrue = "TRUE";
        private string valFalse = "FALSE";

        public TsCheckBox(XElement SourceXml)
        {
            this.control = new CheckBox();
            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable
        {
            get
            {
                //this.value = this.control.Text;
                if (this.control.IsChecked == true) { return new TsVariable(this.name, this.valTrue); }
                else { return new TsVariable(this.name, this.valFalse); }
            }
        }

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

            x = pXml.Element("Checked");
            if (x != null)
            { this.control.IsChecked = true; }

            x = pXml.Element("Label");
            if (x != null)
            { this.label = x.Value; }

            x = pXml.Element("TrueValue");
            if (x != null)
            { this.valTrue = x.Value; }

            x = pXml.Element("FalseValue");
            if (x != null)
            { this.valFalse = x.Value; }

            x = pXml.Element("HAlign");
            if (x != null)
            {
                if ( x.Value.ToUpper() == "LEFT" )
                {
                    this.hAlignment = HorizontalAlignment.Left;
                }
                else if (x.Value.ToUpper() == "RIGHT")
                {
                    this.hAlignment = HorizontalAlignment.Right;
                }
                else if (x.Value.ToUpper() == "CENTER")
                {
                    this.hAlignment = HorizontalAlignment.Center;
                }
            }

            x = pXml.Element("Margin");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this.margin = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            }
            #endregion
        }

        private void Build()
        {          
            this.control.Margin = this.margin;
            this.control.VerticalAlignment = VerticalAlignment.Center;
            this.control.HorizontalAlignment = hAlignment;
            
            this.labelcontrol = new Label();
            //this.labelcontrol.Height = "Auto";
            this.labelcontrol.Content = this.label;
            this.labelcontrol.Height = this.height;
            this.labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
           
        }
    }
}
