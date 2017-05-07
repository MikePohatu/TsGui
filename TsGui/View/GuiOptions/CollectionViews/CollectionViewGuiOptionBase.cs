using System.Windows;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

using TsGui.Linking;
using TsGui.Queries;
using TsGui.Grouping;
using TsGui.Validation;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public abstract class CollectionViewGuiOptionBase : GuiOptionBase, IGuiOption, IToggleControl, ILinkTarget, IValidationGuiOption
    {
        public event ToggleEvent ToggleEvent;

        protected ListItem _currentitem;
        protected string _validationtext;
        protected ValidationToolTipHandler _validationtooltiphandler;
        protected ValidationHandler _validationhandler;
        protected bool _nodefaultvalue;
        protected string _noselectionmessage;
        protected ListBuilder _builder;
        protected Dictionary<string, Group> _itemGroups = new Dictionary<string, Group>();
        protected bool _istoggle = false;

        //public override abstract string CurrentValue { get; }
        //public override abstract TsVariable Variable { get; }
        public override TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.CurrentValue); }
            }
        }
        public override string CurrentValue
        {
            get { return this._currentitem?.Value; }
        }
        public ListItem CurrentItem
        {
            get { return this._currentitem; }
            set { this._currentitem = value; this.OnPropertyChanged(this, "CurrentItem"); }
        }
        public bool IsValid { get { return this.Validate(); } }
        public string ValidationText
        {
            get { return this._validationtext; }
            set { this._validationtext = value; this.OnPropertyChanged(this, "ValidationText"); }
        }

        public CollectionViewGuiOptionBase(XElement InputXml, TsColumn Parent, IDirector director) : base(Parent, director)
        {
            this._setvaluequerylist = new QueryPriorityList(this, this._director);
            this._builder = new ListBuilder(this, this._director);
            this._validationhandler = new ValidationHandler(this, director);
            this._validationtooltiphandler = new ValidationToolTipHandler(this, this._director);
        }

        public void AddItemGroup(Group NewGroup)
        {
            Group g;
            this._itemGroups.TryGetValue(NewGroup.ID, out g);
            if (g == null) { this._itemGroups.Add(NewGroup.ID, NewGroup); }
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            IEnumerable<XElement> inputElements = InputXml.Elements();

            this._validationhandler.AddValidations(InputXml.Elements("Validation"));
            this._nodefaultvalue = XmlHandler.GetBoolFromXAttribute(InputXml, "NoDefaultValue", this._nodefaultvalue);
            this._noselectionmessage = XmlHandler.GetStringFromXElement(InputXml, "NoSelectionMessage", this._noselectionmessage);

            foreach (XElement x in inputElements)
            {
                //the base loadxml will create queries before this so will win
                if (x.Name == "DefaultValue")
                {
                    IQuery defquery = QueryFactory.GetQueryObject(new XElement("Value", x.Value), this._director, this);
                    this._setvaluequerylist.AddQuery(defquery);
                }

                //read in an option and add to a dictionary for later use
                else if (x.Name == "Option")
                {
                    ListItem newoption = new ListItem(x, this.ControlFormatting, this, this._director);
                    this._builder.Add(newoption);

                    IEnumerable<XElement> togglexlist = x.Elements("Toggle");
                    foreach (XElement togglex in togglexlist)
                    {
                        togglex.Add(new XElement("Enabled", newoption.Value));
                        Toggle t = new Toggle(this, this._director, togglex);
                        this._istoggle = true;
                    }
                }

                else if (x.Name == "Query")
                {
                    XElement wrapx = new XElement("wrapx");
                    wrapx.Add(x);
                    QueryPriorityList newlist = new QueryPriorityList(this, this._director);
                    newlist.LoadXml(wrapx);

                    this._builder.Add(newlist);
                }

                else if (x.Name == "Toggle")
                {
                    Toggle t = new Toggle(this, this._director, x);
                    this._istoggle = true;
                }
            }

            if (this._istoggle == true) { this._director.AddToggleControl(this); }
        }

        //public abstract bool Validate(bool CheckSelectionMade);
        public abstract void RefreshValue();
        public abstract void RefreshAll();

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        { this.ToggleEvent?.Invoke(); }

        public void ClearToolTips()
        { this._validationtooltiphandler.Clear(); }

        public void OnValidationChange()
        { this.Validate(false); }

        public bool Validate()
        { return this.Validate(true); }
        
        public bool Validate(bool CheckSelectionMade)
        {
            if (this._director.StartupFinished == false) { return true; }
            if (this.IsActive == false) { this._validationtooltiphandler.Clear(); return true; }
            if ((CheckSelectionMade == true) && (this.CurrentItem == null))
            {
                this.ValidationText = _noselectionmessage;
                this._validationtooltiphandler.ShowError();
                return false;
            }

            bool newvalid = this._validationhandler.IsValid(this.CurrentValue);

            if (newvalid == false)
            {
                string validationmessage = this._validationhandler.ValidationMessage;
                string s = "\"" + this.CurrentItem.Text + "\" is invalid" + Environment.NewLine;
                if (string.IsNullOrEmpty(validationmessage)) { s = s + _validationhandler.FailedValidationMessage; }
                else { s = s + validationmessage; }

                this.ValidationText = s;
                this._validationtooltiphandler.ShowError();
            }
            else { this._validationtooltiphandler.Clear(); }

            return newvalid;
        }

        protected void SetDefaults()
        {
            this._nodefaultvalue = false;
            this._noselectionmessage = "Please select a value";
            this.ControlFormatting.Padding = new Thickness(6, 2, 2, 3);
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        protected void OnSelectionChanged(object o, RoutedEventArgs e)
        {
            this.Validate(false);
            this.NotifyUpdate();
            this.ToggleEvent?.Invoke();
        }

        protected void OnActiveChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.ToggleEvent?.Invoke();
        }

        
    }
}
