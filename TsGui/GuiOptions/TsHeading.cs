using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsHeading: TsBaseOption,IGuiOption
    {
        new private Label _control;
        private bool _bold;

        public TsHeading(XElement SourceXml, MainController RootController): base()
        {
            this._controller = RootController;

            this._control = new Label();
            base._control = this._control;
            
            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable { get { return null; } }

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
            if (this._bold) { this._labelcontrol.FontWeight = FontWeights.Bold; }
        }
    }
}
