using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace TsGui
{
    public class TsCheckBox: TsBaseOption, IGuiOption, IToggleControl
    {
        public event ToggleEvent ToggleEvent;

        new private CheckBox _control;
        private HorizontalAlignment _hAlignment;
        private string _valTrue;
        private string _valFalse;

        public TsCheckBox(XElement SourceXml, MainController RootController) : base()
        {
            this._controller = RootController;

            this._control = new CheckBox();
            base._control = this._control;

            //setup the bindings
            this._control.DataContext = this;
            this._control.SetBinding(Label.IsEnabledProperty, new Binding("IsEnabled"));
            this._control.SetBinding(Label.PaddingProperty, new Binding("Padding"));
            this._control.SetBinding(Label.MarginProperty, new Binding("Margin"));

            this._visiblepadding = new Thickness(0,0,0,0);
            this.Padding = this._visiblepadding;

            this._visiblemargin = new Thickness(2, 1, 2, 1);
            this.Margin = this._visiblemargin;

            this._control.VerticalContentAlignment = VerticalAlignment.Center;
            this._hAlignment = HorizontalAlignment.Left;
          
            this._valTrue = "TRUE";
            this._valFalse = "FALSE";
            this.Height = 17;

            this.LoadXml(SourceXml);
            this.Build();            
        }

        public TsVariable Variable
        {
            get
            {
                return new TsVariable(this.VariableName, this.CurrentValue);
            }
        }

        public string CurrentValue
        {
            get
            {
                if (this._control.IsChecked == true) { return this._valTrue; }
                else { return this._valFalse; }
            }
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

            x = InputXml.Element("TrueValue");
            if (x != null)
            { this._valTrue = x.Value; }

            x = InputXml.Element("FalseValue");
            if (x != null)
            { this._valFalse = x.Value; }

            x = InputXml.Element("Toggle");
            if (x != null)
            {
                Toggle t = new Toggle(this, this._controller, x);
                this._controller.AddToggleControl(this);
            }
            GuiFactory.LoadHAlignment(InputXml, ref this._hAlignment);
            GuiFactory.LoadMargins(InputXml, this._margin);

            #endregion
        }

        private void Build()
        {
            this._control.VerticalAlignment = VerticalAlignment.Center;
            this._control.HorizontalAlignment = this._hAlignment;
        }

        //setup event subscriptions between the toggle and the control
        public void AttachToggle(Toggle Toggle)
        {
            this._control.Click += this.OnChanged;
            this.ToggleEvent += Toggle.OnToggleEvent;
        }

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        {
            this.ToggleEvent(this, new RoutedEventArgs());
        }

        private void OnChanged(object o, RoutedEventArgs e)
        {
            this.ToggleEvent(this, e);
        }
    }
}
