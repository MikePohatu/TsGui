using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows;

namespace TsGui
{
    public class Toggle
    {
        private Group _group;
        private MainController _controller;
        private Dictionary<string, string> _toggles = new Dictionary<string, string>();
        bool _hiddenMode = false;
        IToggleControl _option;

        public Toggle(IToggleControl GuiOption, MainController MainController, XElement InputXml)
        {
            this._controller = MainController;
            this._option = GuiOption;
            this.LoadXml(InputXml);
            this._option.AttachToggle(this);
        }

        private void LoadXml(XElement InputXml)
        {
            XElement x;

            x = InputXml.Element("Hide");
            if (x != null)
            {
                this._hiddenMode = true;
            }

            x = InputXml.Element("GroupID");
            if (x != null)
            {
                if (!string.IsNullOrEmpty(x.Value))
                {
                    this._group = this._controller.GetGroup(x.Value);
                    //this._group.Toggle = this;
                }
                else { throw new InvalidOperationException("Invalid Toggle configured in XML: " + InputXml); }
            }
            else { throw new InvalidOperationException("No GroupID set in Toggle configured in XML: " + InputXml); }

            IEnumerable<XElement> togglesX;
            togglesX = InputXml.Elements("Activate");
            if (togglesX != null)
            {
                foreach (XElement togglex in togglesX)
                {
                    if (!string.IsNullOrEmpty(togglex.Value))
                    {
                        this._toggles.Add(togglex.Value, "ACTIVATE");
                    }
                }
            }
            togglesX = InputXml.Elements("Deactivate");
            if (togglesX != null)
            {
                foreach (XElement togglex in togglesX)
                {
                    if (!string.IsNullOrEmpty(togglex.Value))
                    {
                        this._toggles.Add(togglex.Value, "DEACTIVATE");
                    }
                }
            }
        }

        public void OnToggle(object o, RoutedEventArgs e)
        {
            string val;
            string action;
            val = (this._option.CurrentValue);
            this._toggles.TryGetValue(val, out action);

            if (string.Equals(action, "ACTIVATE")) { this.Activate(); }
            else if (string.Equals(action, "DEACTIVATE")) { this.Deactivate(); }
        }

        private void Activate()
        {
            Debug.WriteLine("Toggle Activate mode: " + this._hiddenMode);
            if (this._hiddenMode == true) { this._group.IsHidden = true; }
            else { this._group.IsEnabled = false; }
        }

        private void Deactivate()
        {
            this._group.IsHidden = false; 
            this._group.IsEnabled = true; 
        }
    }
}
