using System;
using System.Xml.Linq;
using System.Windows.Controls;

namespace TsGui
{
    public interface IGuiOption: ITsGuiElement
    {
        TsVariable Variable { get; }
        Label Label { get; }
        Control Control { get; }
        int Height { get; }
    }
}
