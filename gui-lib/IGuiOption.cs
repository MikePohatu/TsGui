using System;
using System.Xml.Linq;
using System.Windows.Controls;

namespace gui_lib
{
    public interface IGuiOption: ITsGuiElement
    {
        TsVariable Variable { get; }
        Label Label { get; }
        Control Control { get; }
    }
}
