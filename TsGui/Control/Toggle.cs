using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TsGui
{
    public class Toggle
    {
        Group _group;
        string _toggleValueOn;
        string _toggleValueOff;
        bool _hiddenMode;
        TsBaseOption _option;

        public Toggle(Group Group, string ValueOn, string ValueOff, bool Hide, TsBaseOption GuiOption)
        {
            this._group = Group;
            this._toggleValueOn = ValueOn;
            this._toggleValueOff = ValueOff;
            this._hiddenMode = Hide;
            this._option = GuiOption;
        }
    }
}
