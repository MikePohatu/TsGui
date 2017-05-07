using System.Windows;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

using TsGui.Linking;
using TsGui.Queries;
using TsGui.Grouping;
using TsGui.Validation;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public abstract class CollectionViewGuiOptionBase : GuiOptionBase, IGuiOption, IToggleControl, ILinkTarget
    {
        public event ToggleEvent ToggleEvent;

        protected string _validationtext;
        protected ValidationToolTipHandler _validationtooltiphandler;
        protected ValidationHandler _validationhandler;
        protected bool _nodefaultvalue;
        protected string _noselectionmessage;
        protected ListBuilder _builder;
        protected Dictionary<string, Group> _itemGroups = new Dictionary<string, Group>();
        protected bool _istoggle = false;

        public override abstract string CurrentValue { get; }
        public override abstract TsVariable Variable { get; }

        public CollectionViewGuiOptionBase(XElement InputXml, TsColumn Parent, IDirector director) : base(Parent, director)
        {
            this._builder = new ListBuilder(this, this._director);
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

        public abstract bool Validate(bool CheckSelectionMade);
        public abstract void RefreshValue();
        public abstract void RefreshAll();

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded
        public void InitialiseToggle()
        { this.ToggleEvent?.Invoke(); }

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
