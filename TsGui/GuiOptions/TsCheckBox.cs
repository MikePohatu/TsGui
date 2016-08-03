using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace TsGui
{
    public class TsCheckBox: TsBaseOption, IGuiOption
    {
        private Thickness _margin;
        new private CheckBox _control;
        private HorizontalAlignment _hAlignment;
        private string _valTrue;
        private string _valFalse;

        public TsCheckBox(XElement SourceXml) : base()
        {
            this._control = new CheckBox();
            base._control = this._control;

            //setup the bindings
            this._control.DataContext = this;

            this._hAlignment = HorizontalAlignment.Left;
            //this._labelcontrol = new Label();
            this._margin = new Thickness(0, 0, 0, 0);
            //this.Height = 15;
            this._padding = new Thickness(5, 0, 0, 0);           
            this._valTrue = "TRUE";
            this._valFalse = "FALSE";
            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable
        {
            get
            {
                //this.value = this.control.Text;
                if (this._control.IsChecked == true) { return new TsVariable(this.VariableName, this._valTrue); }
                else { return new TsVariable(this.VariableName, this._valFalse); }
            }
        }

        //public Label Label { get { return this._labelcontrol; } }
        //public Control Control { get { return this._control; } }
        //public int Height { get { return this.height; } }
        public void LoadXml(XElement InputXml)
        {
            #region
            XElement x;

            //load the xml for the base class stuff
            this.LoadBaseXml(InputXml);

            x = InputXml.Element("Checked");
            if (x != null)
            { this._control.IsChecked = true; }

            x = InputXml.Element("TrueValue");
            if (x != null)
            { this._valTrue = x.Value; }

            x = InputXml.Element("FalseValue");
            if (x != null)
            { this._valFalse = x.Value; }

            GuiFactory.LoadHAlignment(InputXml, ref this._hAlignment);
            GuiFactory.LoadMargins(InputXml, this._margin);

            #endregion
        }

        private void Build()
        {
            Debug.WriteLine("CheckBox HAlignment: " + this._hAlignment.ToString());
            this._control.VerticalAlignment = VerticalAlignment.Center;
            this._control.HorizontalAlignment = this._hAlignment;
        }
    }
}
