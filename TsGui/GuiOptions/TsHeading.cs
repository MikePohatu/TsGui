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
        private bool _bold;

        public TsHeading(XElement SourceXml): base()
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

            x = InputXml.Element("Bold");
            if (x != null)
            { this._bold = true; }

            #endregion
        }

        private void Build()
        {
            //this._control = new Label();
            this._control.Content = "";
            //this._control.Padding = this._padding;
            //this._control.VerticalAlignment = VerticalAlignment.Center;
            ;
            if (this._bold) { this._labelcontrol.FontWeight = FontWeights.Bold; }
        }
    }
}
