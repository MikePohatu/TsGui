using System;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
//using System.Diagnostics;

namespace TsGui
{
    public abstract class TsBaseOption: INotifyPropertyChanged, IGroupable
    {
        protected Group _group;
        protected bool _isenabled = true;
        protected bool _ishidden = false;
        protected MainController _controller;
        //protected TsColumn _parentColumn;
        protected string _value;
        protected string _help;
        protected int _height;
        protected int _visibleHeight;
        protected ToolTip _tooltip;
        protected Label _labelcontrol;
        protected Control _control;
        protected string _labeltext;
        protected Thickness _visiblemargin;
        protected Thickness _visiblelabelmargin;
        protected Thickness _visiblepadding;
        protected Thickness _visiblelabelpadding;
        protected Thickness _margin;
        protected Thickness _labelmargin;
        protected Thickness _padding;
        protected Thickness _labelpadding;

        //properties
        #region
        public string VariableName { get; set; }
        //public Group Group { get; set; }
        public Label Label { get { return this._labelcontrol; } }
        public Control Control { get { return this._control; } }
        public string Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.OnPropertyChanged(this, "Value");
            }
        }
        public string InactiveValue { get; set; }
        public string LabelText
        {
            get { return this._labeltext; }
            set
            {
                this._labeltext = value;
                this.OnPropertyChanged(this, "LabelText");
            }
        }
        public Thickness Margin
        {
            get { return this._margin; }
            set
            {
                this._margin = value;
                this.OnPropertyChanged(this, "Margin");
            }
        }
        public Thickness LabelMargin
        {
            get { return this._labelmargin; }
            set
            {
                this._labelmargin = value;
                this.OnPropertyChanged(this, "LabelMargin");
            }
        }
        public Thickness Padding
        {
            get { return this._padding; }
            set
            {
                this._padding = value;
                this.OnPropertyChanged(this, "Padding");
            }
        }
        public Thickness LabelPadding
        {
            get { return this._labelpadding; }
            set
            {
                this._labelpadding = value;
                this.OnPropertyChanged(this, "LabelPadding");
            }
        }
        public int Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
                this.OnPropertyChanged(this, "Height");
            }
        }
        public string HelpText
        {
            get { return this._help; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._help = value;
                    this._labelcontrol.ToolTip = this._tooltip;
                    this._control.ToolTip = this._tooltip;
                    OnPropertyChanged(this, "HelpText");
                }
            }
        }
        public ToolTip ToolTip
        {
            get { return this._tooltip; }
            set
            {
                this._tooltip = value;
                OnPropertyChanged(this, "ToolTip");
            }
        }
        public bool IsEnabled
        {
            get { return this._isenabled; }
            set
            {
                //Debug.WriteLine("Enabled property set: " + value);
                //this.EnableDisable(value);
                this._isenabled = value;
                OnPropertyChanged(this, "IsEnabled");
            }
        }
        public bool IsHidden
        {
            get { return this._ishidden; }
            set
            {
                this.HideUnhide(value);
                OnPropertyChanged(this, "IsHidden");
            }
        }
        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false))
                { return true; }
                else { return false; }
            }
        }
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        //constructor
        protected TsBaseOption()
        {
            //this._parentColumn = ParentColumn;
            this._labelcontrol = new Label();
            this._labelcontrol.DataContext = this;
            this._labelcontrol.SetBinding(Label.IsEnabledProperty, new Binding("IsEnabled"));
            this._labelcontrol.SetBinding(Label.ContentProperty, new Binding("LabelText"));
            this._labelcontrol.SetBinding(Label.HeightProperty, new Binding("Height"));
            this._labelcontrol.SetBinding(Label.PaddingProperty, new Binding("LabelPadding"));
            this._labelcontrol.SetBinding(Label.MarginProperty, new Binding("LabelMargin"));

            this._labelcontrol.VerticalAlignment = VerticalAlignment.Bottom;
            this._visiblelabelpadding = new Thickness(3, 0, 0, 0);
            this.LabelPadding = this._visiblelabelpadding;

            this._visiblelabelmargin = new Thickness(2,2,2,2);
            this.LabelMargin = this._visiblelabelmargin;

            //Setup the tooltips
            this._tooltip = new ToolTip();

            //setup the binding
            TextBlock tb = new TextBlock();
            this._tooltip.Content = tb;
            tb.SetBinding(TextBlock.TextProperty, new Binding("HelpText"));

            //Set defaults
            this.InactiveValue = "TSGUI_INACTIVE";
            this._visibleHeight = 20;
            this.Height = 20;
            this._labelcontrol.HorizontalAlignment = HorizontalAlignment.Left;
        }

        protected void LoadBaseXml(XElement InputXml)
        {
            //Load the XML
            #region
            XElement x;
            //XAttribute attrib;

            x = InputXml.Element("Variable");
            if (x != null)
            { this.VariableName = x.Value; }

            x = InputXml.Element("InactiveValue");
            if (x != null)
            { this.InactiveValue = x.Value; }

            x = InputXml.Element("HelpText");
            if (x != null)
            { this.HelpText = x.Value; }

            x = InputXml.Element("Label");
            if (x != null)
            { this.LabelText = x.Value; }

            x = InputXml.Element("Height");
            if (x != null)
            {
                this._visibleHeight = Convert.ToInt32(x.Value);
                this.Height = this._visibleHeight;
            }

            x = InputXml.Element("LabelPadding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this._visiblelabelpadding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
                this.LabelPadding = this._visiblelabelpadding;
            }

            x = InputXml.Element("Enabled");
            if (x != null)
            { this.IsEnabled = Convert.ToBoolean(x.Value); }

            x = InputXml.Element("Hidden");
            if (x != null)
            { this.IsHidden = Convert.ToBoolean(x.Value); }

            x = InputXml.Element("Group");
            if (x != null)
            {
                //Debug.WriteLine("Group: " + Environment.NewLine + xGroup);
                this._group = this._controller.AddToGroup(x.Value, this);
                //this._parentColumn.ParentChanged += this.OnParentChanged;
            }
            #endregion
        }

        protected void HideUnhide(bool Hidden)
        {
            this._ishidden = Hidden;
            if (Hidden == true)
            {
                //(Grid)this._control.Parent.
                this._control.Visibility = Visibility.Collapsed;
                this._labelcontrol.Visibility = Visibility.Collapsed;
                this.Height = 0;
                this.Margin = new Thickness(0);
                this.LabelMargin = new Thickness(0);
            }
            else
            {
                this._control.Visibility = Visibility.Visible;
                this._labelcontrol.Visibility = Visibility.Visible;
                this.Height = this._visibleHeight;
                this.Padding = this._visiblepadding;
                this.Margin = this._visiblemargin;
                this.LabelMargin = this._visiblelabelmargin;
            }
        }
        
        //Only subscribed if member of a group. Registers changes to parent elements. 
        public void OnParentChanged(IGroupParent p, bool IsEnabled, bool IsHidden)
        {
            if ((IsHidden == true) || (IsEnabled == false))
            {
                this.IsEnabled = IsEnabled;
                this.IsHidden = IsHidden;
            }
            else if (this._group != null)
            {
                this.IsHidden = this._group.IsHidden;
                this.IsEnabled = this._group.IsEnabled;
            }
            else
            {
                this.IsHidden = false;
                this.IsEnabled = true;
            }
            //if (i == 2) { this.HideUnhide(true); }
            //else if (i == 1) { this.IsEnabled = false; }
            //else if (this._group != null)
            //{
            //    this.IsHidden = this._group.IsHidden;
            //    this.IsEnabled = this._group.IsEnabled;      
            //}
            //else
            //{
            //    this.IsHidden = false;
            //    this.IsEnabled = true;
            //}
        }
    }
}
