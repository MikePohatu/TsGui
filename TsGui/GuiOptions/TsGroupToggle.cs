using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;

namespace TsGui
{
    class TsGroupToggle : TsBaseOption
    {
        private Group _group;
        private Thickness _margin;
        new private CheckBox _control;
        private HorizontalAlignment _hAlignment;

        public TsGroupToggle(XElement SourceXml) : base()
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

            this.LoadXml(SourceXml);
        }

        public void LoadXml(XElement InputXml)
        {
            #region
            XElement x;

            //load the xml for the base class stuff
            this.LoadBaseXml(InputXml);

            x = InputXml.Element("Checked");
            if (x != null)
            { this._control.IsChecked = true; }

            GuiFactory.LoadHAlignment(InputXml, ref this._hAlignment);
            GuiFactory.LoadMargins(InputXml, this._margin);

            #endregion
        }
    }
}
