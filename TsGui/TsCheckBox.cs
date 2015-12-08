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

        public void LoadXml(XElement SourceXml)
        {
            #region
            XElement x;

            x = SourceXml.Element("Variable");
            if (x != null)
            { this.name = x.Value; }

            x = SourceXml.Element("Label");
            if (x != null)
            { this.label = x.Value; }

            x = SourceXml.Element("Checked");
            if (x != null)
            { this.control.IsChecked = true; }

            x = SourceXml.Element("TrueValue");
            if (x != null)
            { this.valTrue = x.Value; }

            x = SourceXml.Element("FalseValue");
            if (x != null)
            { this.valFalse = x.Value; }


            GuiFactory.LoadHAlignment(SourceXml, ref this.hAlignment);
            GuiFactory.LoadMargins(SourceXml, this.margin);

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
